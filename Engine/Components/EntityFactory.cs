using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Calcifer.Engine.Graphics;
using Calcifer.Engine.Graphics.Animation;
using Calcifer.Engine.Physics;
using Calcifer.Engine.Scenery;
using Calcifer.Engine.Scripting;
using Calcifer.Utilities;
using ComponentKit;
using ComponentKit.Model;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using OpenTK;

namespace Calcifer.Engine.Components
{
    public delegate IEnumerable<IComponent> MultiConstructorDelegate(ParameterCollection param);
    public delegate IComponent ConstructorDelegate(ParameterCollection param);

    public static class EntityFactory
    {

        private static DefinitionCollection definitions;
        private static Dictionary<string, Delegate> constructors;

        public static void AddConstructor(string componentType, ConstructorDelegate constructor)
        {
            constructors.Add(componentType, constructor);
        }

        public static void AddMultiConstructor(string componentType, MultiConstructorDelegate constructor)
        {
            constructors.Add(componentType, constructor);
        }

        public static void Define(EntityDefinition definition)
        {
            if (!definitions.Contains(definition.Name))
                definitions.Add(definition);
        }

        public static IEntityRecord Create(string name, string className)
        {
            var def = definitions[className];
            return Create(name, def);
        }

        public static IEntityRecord Create(EntityInstance instance)
        {
            var def = definitions[instance.ClassName];
            var newdef = new EntityDefinition(instance.Name);
            foreach (var p in def.Parameters)
                newdef.Parameters.Add(p);
            foreach (var delta in instance.Deltas)
            {
                var key = delta.Component + ":" + delta.Parameter;
                if (delta.Type == DeltaType.Add)
                {
                    if (newdef.Parameters.Contains(key))
                        newdef.Parameters[key].Value = delta.Value;
                    else
                        newdef.Parameters.Add(new Parameter(delta.Component, delta.Parameter, delta.Value));
                }
                else if (newdef.Parameters.Contains(key)) newdef.Parameters.Remove(key);
            }
            return Create(instance.Name, newdef);
        }
        
        private static IEntityRecord Create(string name, EntityDefinition definition)
        {
            var e = Entity.Create(name);
            foreach (var c in definition.Parameters.GetComponents().
                SelectMany(type => BuildComponent(type, definition.Parameters)))
                e.Add(c);
            return e;
        }

        private static IEnumerable<IComponent> BuildComponent(string type, ParameterCollection param)
        {
            if (!constructors.ContainsKey(type))
                throw new Exception("Unknown component type: " + type);
            var ctr = constructors[type];
            var cd = ctr as ConstructorDelegate;
            if (cd != null)
                yield return cd(param);
            else
            {
                var mcd = ctr as MultiConstructorDelegate;
                if (mcd != null)
                {
                    var result = mcd(param);
                    foreach (var c in result) yield return c;
                }
            }
        }

        static EntityFactory()
        {
            definitions = new DefinitionCollection();
            constructors = new Dictionary<string, Delegate>();
            AddMultiConstructor("physics", BuildPhysicsComponent);
            AddConstructor("render", BuildRenderComponent);
            AddConstructor("health", BuildHealthComponent);
            AddConstructor("animation", BuildAnimationComponent);
            AddConstructor("luaScript", BuildLuaComponent);
            AddConstructor("luaStorage", BuildStorageComponent);
            AddConstructor("transform", BuildTransformComponent);
            AddConstructor("crate", p => new CrateComponent());
            AddConstructor("projectile", p => new ProjectileComponent());
            AddConstructor("movable", p => new WaypointMovableComponent());
            AddConstructor("sensor", p => new SensorComponent());
            AddConstructor("motion", p => new MotionComponent());
            AddConstructor("player", p => new PlayerStateComponent());
        }

        private static IComponent BuildRenderComponent(ParameterCollection param)
        {
            var mesh = ResourceFactory.LoadAsset<MeshData>(param["render", "meshData"]);
            return new RenderComponent(mesh);
        }

        private static IComponent BuildHealthComponent(ParameterCollection param)
        {
            return new HealthComponent(int.Parse(param["health", "hp"] ?? "100"));
        }

        private static IComponent BuildAnimationComponent(ParameterCollection param)
        {
            var restPose = ResourceFactory.LoadAsset<AnimationData>(param["animation", "restPose"]).Frames[0];
            var type = param["animation", "controllerType"];
            restPose.CalculateWorld();
            var c = new BlendAnimationController(restPose);
            if (param["animation", "animations"] != null)
                foreach (var animName in param["animation", "animations"].Split(';'))
                    c.AddAnimation(ResourceFactory.LoadAsset<AnimationData>(animName));
            return c;
        }

        private static IEnumerable<IComponent> BuildPhysicsComponent(ParameterCollection param)
        {
            var type = param["physics", "type"];
            RigidBody body;
            switch (type)
            {
                case "trimesh":
                    var physData = ResourceFactory.LoadAsset<PhysicsData>(param["physics", "physData"]);
                    var tris = physData.Shapes[0].Triangles.Select(t => new TriangleVertexIndices(t.X, t.Y, t.Z)).ToList();
                    var verts = physData.Shapes[0].Vertices.Select(v => v.Position.ToJVector()).ToList();
                    var materials = physData.Shapes.Select(g => new Tuple<int, int, string>(g.Offset, g.Count, g.Material.Name)).ToList();
                    var octree = new Octree(verts, tris);
                    yield return new TerrainComponent(materials, octree);
                    Shape shape = new TriangleMeshShape(octree);
                    body = new RigidBody(shape) { Material = { Restitution = 0f, KineticFriction = 0f } };
                    break;
                case "hull":
                    physData = ResourceFactory.LoadAsset<PhysicsData>(param["physics", "physData"]);
                    shape = new MinkowskiSumShape(physData.Shapes.Select(
                        g => new ConvexHullShape(g.Vertices.Select(v => v.Position.ToJVector()).ToList())
                        ));
                    body = new RigidBody(shape);
                    break;
                case "sphere":
                    shape = new SphereShape(float.Parse(param["physics", "radius"], CultureInfo.InvariantCulture));
                    body = new RigidBody(shape);
                    break;
                case "box":
                    var d = param["physics", "size"].ConvertToVector();
                    var offset = (param["physics", "offset"] ?? "0;0;0").ConvertToVector();
                    shape = new BoxShape(2.0f * d.ToJVector());
                    body = new RigidBody(shape) { Position = offset.ToJVector() };
                    break;
                case "capsule":
                    var height = float.Parse(param["physics", "height"], CultureInfo.InvariantCulture);
                    var radius = float.Parse(param["physics", "radius"], CultureInfo.InvariantCulture);
                    shape = new CapsuleShape(height, radius);
                    body = new RigidBody(shape)
                    {
                        Position = JVector.Backward * (0.5f * height + radius),
                        Orientation = JMatrix.CreateRotationX(MathHelper.PiOver2)
                    };
                    break;
                default:
                    throw new Exception("Unknown shape: " + type);
            }
            bool isStatic = Convert.ToBoolean(param["physics", "static"] ?? "false");
            body.IsStatic = isStatic;
            yield return new PhysicsComponent(body);
        }

        private static IComponent BuildLuaComponent(ParameterCollection param)
        {
            return new LuaComponent(param["luaScript", "source"] ?? File.ReadAllText(param["luaScript", "sourceRef"]));
        }

        private static IComponent BuildStorageComponent(ParameterCollection param)
        {
            return new WaypointComponent(param["luaStorage", "nodes"].Split(';').Select(Entity.Find));
        }

        private static IComponent BuildTransformComponent(ParameterCollection param)
        {
            var r = param["transform", "rotation"].ConvertToVector();
            return new TransformComponent
            {
                Translation = param["transform", "translation"].ConvertToVector(),
                Rotation = Quaternion.FromAxisAngle(Vector3.UnitX, r.X) *
                           Quaternion.FromAxisAngle(Vector3.UnitY, r.Y) *
                           Quaternion.FromAxisAngle(Vector3.UnitZ, r.Z),
                Scale = param["transform", "scale"].ConvertToVector()
            };
        }
    }
}
