using System.Collections.Generic;
using Calcifer.Engine.Components;
using Calcifer.Engine.Graphics.Animation;
using Calcifer.Engine.Graphics.Primitives;
using Calcifer.Engine.Scripting;
using Calcifer.Utilities;
using Calcifer.Utilities.Logging;
using ComponentKit.Model;
using System;
using LuaSharp;
using OpenTK;
using OpenTK.Input;

namespace Calcifer.Engine.Scripting
{
    public class LuaService : Component
    {
        private Lua lua;
        private LuaComponent currentScript;
        private HashSet<string> collisions;
        private Random rand;
        private int lights;
        public LuaService()
        {
            lua = new Lua();
            rand = new Random();
            collisions = new HashSet<string>();
            InitializeCore();
            InitializeKeyboard();
            InitializeProperties();
            InitializeNavigation();
            InitializeNodes();
            InitializePhysics();
            InitializeAnimation();
            InitializeHealth();
            InitializeSound();
        }

        public void ExecuteScript(LuaComponent script)
        {
            currentScript = script;
            lua["this"] = currentScript.Record.Name;
            try
            {
                lua.DoString(script.Source);
            }
            catch (LuaException ex)
            {
                Log.WriteLine(LogLevel.Error, ex.Message);
            }
        }
        private void InitializeCore()
        {
            lua["log"] = new Action<string>(s => Log.WriteLine(LogLevel.Info, s));
            lua["lighting"] = new Action<int>(i => lights = i);
            lua["create_valid_object_name"] = new Func<string>(() => Guid.NewGuid().ToString());
            lua["location"] = new Func<string>(() => "There ain't no way I'm tellin' ya that, punk");
            lua["get_name"] = new Func<string>(() => currentScript.Record.Name);
            lua["append_object"] = new Action<string, string, string>(AddObject);
            lua["remove_object"] = new Action<string>(name => Record.Registry.Drop(Entity.Find(name)));
            lua["has_waited"] = new Func<string, bool>(name => !currentScript.IsWaiting);
            lua["wait"] = new Action<string, int>((name, count) => currentScript.Wait((float)count / 60f));
        }
        private void InitializeKeyboard()
        {
            lua["key_f1"] = new Func<bool>(() => Keyboard.GetState().IsKeyDown(Key.F1));
            lua["key_f2"] = new Func<bool>(() => Keyboard.GetState().IsKeyDown(Key.F2));
            lua["key_f3"] = new Func<bool>(() => Keyboard.GetState().IsKeyDown(Key.F3));
            lua["key_f4"] = new Func<bool>(() => Keyboard.GetState().IsKeyDown(Key.F4));
            lua["key_space"] = new Func<bool>(() => Keyboard.GetState().IsKeyDown(Key.Space));
            lua["key_right"] = new Func<bool>(() => Keyboard.GetState().IsKeyDown(Key.Right));
            lua["key_left"] = new Func<bool>(() => Keyboard.GetState().IsKeyDown(Key.Left));
            lua["key_up"] = new Func<bool>(() => Keyboard.GetState().IsKeyDown(Key.Up));
            lua["key_down"] = new Func<bool>(() => Keyboard.GetState().IsKeyDown(Key.Down));
            lua["key_shift"] = new Func<bool>(() => Keyboard.GetState().IsKeyDown(Key.ShiftLeft) || Keyboard.GetState().IsKeyDown(Key.ShiftRight));
            lua["key_control"] = new Func<bool>(() => Keyboard.GetState().IsKeyDown(Key.ControlLeft) || Keyboard.GetState().IsKeyDown(Key.ControlRight));
        }
        private void InitializeProperties()
        {
            lua["set_can_push"] = new Action<string, bool>((name, value) => Get<LuaStorageComponent>(name).CanPush = value);
            lua["get_can_push"] =  new Func<string, bool>(name => Get<LuaStorageComponent>(name).CanPush);
            lua["can_climb"] = new Func<string, bool>(name => Get<LuaStorageComponent>(name).CanClimb);
            lua["can_get_over"] = new Func<string, bool>(name => Get<LuaStorageComponent>(name).CanGetOver);
            lua["can_walk"] = new Func<string, bool>(name => Get<LuaStorageComponent>(name).CanWalk);
            lua["can_hit_wall"] = new Func<string, bool>(name => Get<LuaStorageComponent>(name).CanHitWall);
        }
        private void InitializeNavigation()
        {
            lua["get_pos_x"] = new Func<string, float>(name => Get<TransformComponent>(name).Translation.X);
            lua["get_pos_y"] = new Func<string, float>(name => Get<TransformComponent>(name).Translation.Z);
            lua["get_pos_z"] = new Func<string, float>(name => Get<TransformComponent>(name).Translation.Y);
            lua["get_rot_x"] = new Func<string, float>(name => Get<TransformComponent>(name).Rotation.ToPitchYawRoll().X * 180f / 3.14159274f);
            lua["get_rot_y"] = new Func<string, float>(name => Get<TransformComponent>(name).Rotation.ToPitchYawRoll().Z * 180f / 3.14159274f);
            lua["get_rot_z"] = new Func<string, float>(name => Get<TransformComponent>(name).Rotation.ToPitchYawRoll().Y * 180f / 3.14159274f);
            lua["angle"] = new Func<string, string, double>(GetAngle);
            lua["distance"] = new Func<string, string, double>((name1, name2) => (Get<TransformComponent>(name1).Translation - Get<TransformComponent>(name2).Translation).Length);
            lua["set_pos"] = new Action<string, float, float, float>((name, x, y, z) => Get<TransformComponent>(name).Translation = new Vector3(x, y, z));
            lua["move_step_local"] = new Action<string, float, float, float>((name, x, y, z) => { });
            lua["move_step"] = new Action<string, float, float, float>((name, x, y, z) => { });
            lua["rotate_step"] =new Action<string, float, float, float>((name, x, y, z) => { });
            lua["jump"] = new Action(() => { });
        }
        private double GetAngle(string name1, string name2)
        {
            Quaternion rotation = Get<TransformComponent>(name1).Rotation;
            Quaternion rotation2 = Get<TransformComponent>(name2).Rotation;
            return 2.0 * Math.Acos(rotation.X * rotation2.X + rotation.Y * rotation2.Y + rotation.Z * rotation2.Z + rotation.W * rotation2.W);
        }
        private bool IsNear(string name1, string name2)
        {
            ScalableTransform transform = Get<TransformComponent>(name1).Transform;
            ScalableTransform transform2 = Get<TransformComponent>(name2).Transform;
            double angle = GetAngle(name1, name2);
            float length = (transform.Translation - transform2.Translation).Length;
            return angle < 0.1f * Math.PI && length < 0.5f;
        }
        private void InitializeNodes()
        {
            lua["get_node"] = new Func<string, int>(name => Get<LuaStorageComponent>(name).CurrentNode);
            lua["set_node"] = new Action<string, int>((name, id) => Get<LuaStorageComponent>(name).CurrentNode = id);
            lua["move_to_node"] = new Action<string, int>((name, id) => { });
            lua["is_at_node"] = new Func<string, bool>(name => IsNear(name, Get<LuaStorageComponent>(name).Nodes[Get<LuaStorageComponent>(name).CurrentNode]));
        }
        private void InitializePhysics()
        {
            lua["collision_between"] = new Func<string, string, bool>((name1, name2) => false);
            lua["set_gravity"] = new Action<string, float>((name, value) => { });
        }
        private void InitializeAnimation()
        {
            lua["set_anim"] = new Action<string, string>((name, anim) => GetAnimationController(name).Start(anim, false));
            lua["get_anim"] = new Func<string, string>(name => Get<AnimationComponent>(name).Name);
            lua["get_frame"] = new Func<string, float>(name =>
                                                          {
                                                              var animationController = GetAnimationController(name);
                                                              return animationController.Time*animationController.Speed;
                                                          });
            lua["get_frame_ratio"] = new Func<string, float>(name => GetAnimationController(name).Speed);
            lua["is_anim_finished"] = new Func<string, bool>(name => Math.Abs(GetAnimationController(name).Time - GetAnimationController(name).Length) < 0.01);
        }
        private BlendAnimationController GetAnimationController(string name)
        {
            var blendAnimationController = Get<AnimationComponent>(name) as BlendAnimationController;
            if (blendAnimationController != null)
            {
                return blendAnimationController;
            }
            throw new LuaException("this AnimationComponent isn't BlendAnimationController");
        }
        private void InitializeHealth()
        {
            lua["get_health"] = new Func<string, int>(name => GetHealthComponent(name).Health);
            lua["set_health"] = new Action<string, int>((name, value) => GetHealthComponent(name).Health = value);
            lua["get_wounded"] = new Func<string, bool>(name => GetHealthComponent(name).IsWounded);
            lua["set_wounded"] = new Action<string, bool>((name, value) => Log.WriteLine(LogLevel.Error, "IsWounded property is read-only. Use set_health instead"));
        }
        private void InitializeSound()
        {
            lua["get_sound"] = new Func<string, string>(name => name);
            lua["play_sound"] = new Action<string, string>((owner, sound) => PlaySound(owner, sound, false, 1f));
            lua["play_sound_loop"] = new Action<string, string>((owner, sound) => PlaySound(owner, sound, true, 1f));
            lua["play_sound_random_pitch"] = new Action<string, string>((owner, sound) => PlaySound(owner, sound, false, 0.95f + 0.1f * (float)rand.NextDouble()));
        }
        private T Get<T>(string name) where T : Component
        {
            var t = Entity.Find(name).GetComponent<T>();
			if (t != null)
			{
				return t;
			}
			throw new LuaException(typeof(T) + " not present in " + name);
		}
        private HealthComponent GetHealthComponent(string name)
        {
            var healthComponent = Entity.Find(name).GetComponent<HealthComponent>();
            if (healthComponent != null)
                return healthComponent;
            throw new LuaException("Immortal object! HealthComponent not present in " + name);
        }
        private void PlaySound(string owner, string sound, bool looped, float pitch)
        {
            Log.WriteLine(LogLevel.Warning, "Sound playing not yet implemented.");
        }
        private void AddObject(string map, string nameInMap, string name)
        {
            Log.WriteLine(LogLevel.Warning, "Entity creation not yet implemented.");
        }
    }
}
