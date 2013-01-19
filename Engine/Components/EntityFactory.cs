using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Calcifer.Engine.Content;
using Calcifer.Engine.Content.Pipeline;
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
    public class EntityFactory
    {
        private ContentManager content;
        private AssetCollection assets;

        public EntityFactory(ContentManager content)
        {
            assets = new AssetCollection();
            this.content = content;
        }

        private DefinitionCollection definitions = new DefinitionCollection();

        public void Define(EntityDefinition definition)
        {
            if (!definitions.Contains(definition.Name))
                definitions.Add(definition);
        }

        public IEntityRecord Create(string name, string className)
        {
            var def = definitions[className];
            return Create(name, def);
        }

        public IEntityRecord Create(EntityInstance instance)
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
        
        private IEntityRecord Create(string name, EntityDefinition definition)
        {
            var e = Entity.Create(name);
            foreach (var type in definition.Parameters.GetComponents())
                BuildComponent(type, e, definition);
            return e;
        }

        private void BuildComponent(string type, IEntityRecord e, EntityDefinition def)
        {
            switch (type)
            {
                case "transform":
                    BuildTransformComponent(def, e);
                    break;
                case "mesh":
                    BuildMeshComponent(def, e);
                    break;
                case "animation":
                    BuildAnimationComponent(def, e);
                    break;
                case "physics":
                    BuildPhysicsComponent(def, e);
                    break;
                case "luaScript":
                    BuildLuaComponent(def, e);
                    break;
                case "luaStorage":
                    BuildStorageComponent(def, e);
                    break;
                case "health":
                    BuildHealthComponent(def, e);
                    break;
                case "render":
                    e.Add(new RenderComponent());
                    break;
                case "sensor":
                    e.Add(new SensorComponent());
                    break;
                case "crate":
                    e.Add(new CrateComponent());
                    break;
                case "motion":
                    e.Add(new MotionComponent());
                    break;
                case "movable":
                    e.Add(new WaypointMovableComponent());
                    break;
                case "projectile":
                    e.Add(new ProjectileComponent());
                    break;
                case "player":
                    e.Add(new PlayerStateComponent());
                    break;
                default:
                    throw new Exception("Unknown component type: " + type);
            }
        }

        private void BuildHealthComponent(EntityDefinition def, IEntityRecord entityRecord)
        {
            entityRecord.Add(new HealthComponent(int.Parse(def["health:hp"] ?? "100")));
        }

        private void BuildMeshComponent(EntityDefinition def, IEntityRecord entityRecord)
        {
            entityRecord.Add(LoadAsset<MeshData>(assets[def["mesh:meshData"]]));
        }

        private void BuildAnimationComponent(EntityDefinition def, IEntityRecord entityRecord)
        {
            var restPose = LoadAsset<AnimationData>(assets[def["animation:restPose"]]).Frames[0];
            var type = def["animation:controllerType"];
            restPose.CalculateWorld();
            var c = new BlendAnimationController(restPose);
            if (def["animation:animations"] != null)
                foreach (var animName in def["animation:animations"].Split(';'))
                    c.AddAnimation(LoadAsset<AnimationData>(assets[animName]));
            entityRecord.Add(c);
        }

        private void BuildPhysicsComponent(EntityDefinition def, IEntityRecord entityRecord)
        {
            var type = def["physics:type"];
            RigidBody body;
            switch (type)
            {
                case "trimesh":
                    var physData = LoadAsset<PhysicsData>(assets[def["physics:physData"]]);
                    var tris = physData.Shapes[0].Triangles.Select(t => new TriangleVertexIndices(t.X, t.Y, t.Z)).ToList();
                    var verts = physData.Shapes[0].Vertices.Select(v => v.Position.ToJVector()).ToList();
                    var materials = physData.Shapes.Select(g => new Tuple<int, int, string>(g.Offset, g.Count, g.Material.Name)).ToList();
                    var octree = new Octree(verts, tris);
                    entityRecord.Add(new TerrainComponent(materials, octree));
                    Shape shape = new TriangleMeshShape(octree);
                    body = new RigidBody(shape) { Material = { Restitution = 0f, KineticFriction = 0f } };
                    break;
                case "hull":
                    physData = LoadAsset<PhysicsData>(assets[def["physics:physData"]]);
                    shape = new MinkowskiSumShape(physData.Shapes.Select(
                        g => new ConvexHullShape(g.Vertices.Select(v => v.Position.ToJVector()).ToList())
                        ));
                    body = new RigidBody(shape);
                    break;
                case "sphere":
                    shape = new SphereShape(float.Parse(def["physics:radius"], CultureInfo.InvariantCulture));
                    body = new RigidBody(shape);
                    break;
                case "box":
                    var d = def["physics:size"].ConvertToVector();
                    var offset = (def["physics:offset"] ?? "0;0;0").ConvertToVector();
                    shape = new BoxShape(2.0f * d.ToJVector());
                    body = new RigidBody(shape) { Position = offset.ToJVector() };
                    break;
                case "capsule":
                    var height = float.Parse(def["physics:height"], CultureInfo.InvariantCulture);
                    var radius = float.Parse(def["physics:radius"], CultureInfo.InvariantCulture);
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
            bool isStatic = Convert.ToBoolean(def["physics:static"] ?? "false");
            body.IsStatic = isStatic;
            entityRecord.Add(new PhysicsComponent(body));
        }

        private void BuildLuaComponent(EntityDefinition def, IEntityRecord entityRecord)
        {
            entityRecord.Add(new LuaComponent(def["luaScript:source"] ?? File.ReadAllText(def["luaScript:sourceRef"])));
        }

        private void BuildStorageComponent(EntityDefinition def, IEntityRecord entityRecord)
        {
            entityRecord.Add(new WaypointComponent(def["luaStorage:nodes"].Split(';').Select(Entity.Find)));
        }

        private void BuildTransformComponent(EntityDefinition def, IEntityRecord entityRecord)
        {
            var r = def["transform:rotation"].ConvertToVector();
            entityRecord.Add(new TransformComponent
            {
                Translation = def["transform:translation"].ConvertToVector(),
                Rotation = Quaternion.FromAxisAngle(Vector3.UnitX, r.X) *
                           Quaternion.FromAxisAngle(Vector3.UnitY, r.Y) *
                           Quaternion.FromAxisAngle(Vector3.UnitZ, r.Z),
                Scale = def["transform:scale"].ConvertToVector()
            });
        }

        private T LoadAsset<T>(AssetReference r) where T : class, IResource
        {
            return r.Composite ? content.Load<CompositeResource>(r.Source).OfType<T>().First() : content.Load<T>(r.Source);
        }

        public void AddAsset(AssetReference asset)
        {
            assets.Add(asset);
        }
    }
}
