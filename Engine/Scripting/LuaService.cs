using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Calcifer.Engine.Components;
using Calcifer.Engine.Graphics.Animation;
using Calcifer.Utilities;
using Calcifer.Utilities.Logging;
using ComponentKit;
using ComponentKit.Model;
using System;
using LuaInterface.Exceptions;
using OpenTK;
using OpenTK.Input;
using LuaInterface;

namespace Calcifer.Engine.Scripting
{
    public class LuaService
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
            InitializeText();
            EntityRegistry.Current.SetTrigger(c => c is LuaComponent, ComponentSync);
        }

        private void ComponentSync(object sender, ComponentSyncEventArgs e)
        {
            foreach (var c in e.Components.OfType<LuaComponent>())
                c.Service = c.IsOutOfSync ? null : this;
        }

        private bool halt;
	    private HashSet<LuaComponent> blacklist = new HashSet<LuaComponent>(); 
        public void ExecuteScript(LuaComponent script)
        {
            if (halt)
                return;
	        if (blacklist.Contains(script))
				return;
			currentScript = script;
            lua["this"] = currentScript.Record.Name;
	        var watch = new Stopwatch();
            try
            {
	            var name = currentScript.Record.Name;
				watch.Start();
                lua.DoString(script.Source);
	            watch.Stop();
	            var elapsed = watch.Elapsed.TotalMilliseconds;
				if (elapsed > 15)
				{
					Log.WriteLine(LogLevel.Warning, "{0} script: took {1}", name, elapsed);
					//blacklist.Add(script);
				}
            }
            catch (LuaException ex)
            {
                Log.WriteLine(LogLevel.Error, ex.Message);
                halt = true;
            }
        }
        private void InitializeCore()
        {
			//lua.RegisterFunction("log", this, new Action<string>(s => Log.WriteLine(LogLevel.Info, s)).Method);
			lua.RegisterFunction("log", this, new Action<string>(s => { }).Method);
            lua.RegisterFunction("lighting", this, new Action<int>(i => lights = i).Method);
            lua.RegisterFunction("create_valid_object_name", this, new Func<string>(() => Guid.NewGuid().ToString()).Method);
            lua.RegisterFunction("location", this, new Func<string>(() => "There ain't no way I'm tellin' ya that, punk").Method);
            lua.RegisterFunction("get_name", this, new Func<string>(() => currentScript.Record.Name).Method);
            lua.RegisterFunction("append_object", this, new Action<string, string, string>(AddObject).Method);
            lua.RegisterFunction("remove_object", this, new Action<string>(name =>
                                                                               {
                                                                                   var r = currentScript.Record.Registry;
                                                                                   r.Drop(Entity.Find(name));
                                                                                   r.Synchronize();
                                                                               }).Method);
            lua.RegisterFunction("has_waited", this, new Func<string, bool>(s => !currentScript.IsWaiting).Method);
            lua.RegisterFunction("wait", this, new Action<string, int>((name, count) => currentScript.Wait(count / 60f)).Method);
        }
        
        private void InitializeKeyboard()
        {
            lua.RegisterFunction("key_f1", this, new Func<bool>(() => Keyboard.GetState().IsKeyDown(Key.F1)).Method);
            lua.RegisterFunction("key_f2", this, new Func<bool>(() => Keyboard.GetState().IsKeyDown(Key.F2)).Method);
            lua.RegisterFunction("key_f3", this, new Func<bool>(() => Keyboard.GetState().IsKeyDown(Key.F3)).Method);
            lua.RegisterFunction("key_f4", this, new Func<bool>(() => Keyboard.GetState().IsKeyDown(Key.F4)).Method);
            lua.RegisterFunction("key_space", this, new Func<bool>(() => Keyboard.GetState().IsKeyDown(Key.Space)).Method);
            lua.RegisterFunction("key_right", this, new Func<bool>(() => Keyboard.GetState().IsKeyDown(Key.Right)).Method);
            lua.RegisterFunction("key_left", this, new Func<bool>(() => Keyboard.GetState().IsKeyDown(Key.Left)).Method);
            lua.RegisterFunction("key_up", this, new Func<bool>(() => Keyboard.GetState().IsKeyDown(Key.Up)).Method);
            lua.RegisterFunction("key_down", this, new Func<bool>(() => Keyboard.GetState().IsKeyDown(Key.Down)).Method);
            lua.RegisterFunction("key_shift", this, new Func<bool>(() => Keyboard.GetState().IsKeyDown(Key.ShiftLeft) || Keyboard.GetState().IsKeyDown(Key.ShiftRight)).Method);
            lua.RegisterFunction("key_control", this, new Func<bool>(() => Keyboard.GetState().IsKeyDown(Key.ControlLeft) || Keyboard.GetState().IsKeyDown(Key.ControlRight)).Method);
        }
        private void InitializeProperties()
        {
            lua.RegisterFunction("set_can_push", this, new Action<string, bool>((name, value) => Get<LuaStorageComponent>(name).CanPush = value).Method);
            lua.RegisterFunction("get_can_push", this,  new Func<string, bool>(name => Get<LuaStorageComponent>(name).CanPush).Method);
            lua.RegisterFunction("can_climb", this, new Func<string, bool>(name => Get<LuaStorageComponent>(name).CanClimb).Method);
            lua.RegisterFunction("can_get_over", this, new Func<string, bool>(name => Get<LuaStorageComponent>(name).CanGetOver).Method);
            lua.RegisterFunction("can_walk", this, new Func<string, bool>(name => Get<LuaStorageComponent>(name).CanWalk).Method);
            lua.RegisterFunction("can_hit_wall", this, new Func<string, bool>(name => Get<LuaStorageComponent>(name).CanHitWall).Method);
        }
        private void InitializeNavigation()
        {
            lua.RegisterFunction("get_pos_x", this, new Func<string, float>(name => Get<TransformComponent>(name).Translation.X).Method);
            lua.RegisterFunction("get_pos_y", this, new Func<string, float>(name => Get<TransformComponent>(name).Translation.Y).Method);
            lua.RegisterFunction("get_pos_z", this, new Func<string, float>(name => Get<TransformComponent>(name).Translation.Z).Method);
            lua.RegisterFunction("get_rot_x", this, new Func<string, float>(name => Get<TransformComponent>(name).Rotation.ToPitchYawRoll().X * 180f / 3.14159274f).Method);
            lua.RegisterFunction("get_rot_y", this, new Func<string, float>(name => Get<TransformComponent>(name).Rotation.ToPitchYawRoll().Y * 180f / 3.14159274f).Method);
            lua.RegisterFunction("get_rot_z", this, new Func<string, float>(name => Get<TransformComponent>(name).Rotation.ToPitchYawRoll().Z * 180f / 3.14159274f).Method);
            lua.RegisterFunction("angle", this, new Func<string, string, double>(GetAngle).Method);
            lua.RegisterFunction("distance", this, new Func<string, string, double>((name1, name2) => Distance(name1, name2)).Method);
            lua.RegisterFunction("set_pos", this, new Action<string, float, float, float>((name, x, y, z) => Get<TransformComponent>(name).Translation = new Vector3(x, y, z)).Method);
            lua.RegisterFunction("move_step_local", this, new Action<string, float, float, float>((name, x, y, z) => { }).Method);
            lua.RegisterFunction("move_step", this, new Action<string, float, float, float>((name, x, y, z) => { }).Method);
            lua.RegisterFunction("rotate_step", this, new Action<string, float, float, float>((name, x, y, z) => { }).Method);
            lua.RegisterFunction("jump", this, new Action(() => { }).Method);
        }

        private void InitializeNodes()
        {
            lua.RegisterFunction("get_node", this,  new Func<string, int>(name => Get<LuaStorageComponent>(name).CurrentNode).Method);
            lua.RegisterFunction("set_node", this,  new Action<string, int>((name, id) => Get<LuaStorageComponent>(name).CurrentNode = id).Method);
            lua.RegisterFunction("move_to_node", this,  new Action(() => { }).Method);
            lua.RegisterFunction("is_at_node", this,  new Func<string, bool>(name => Distance(name, Get<LuaStorageComponent>(name).Nodes[Get<LuaStorageComponent>(name).CurrentNode]) < 0.5f).Method);
            lua.RegisterFunction("distance_to_node", this,
                                 new Func<string, int, float>((name, id) => Distance(name, Get<LuaStorageComponent>(name).Nodes[id])).Method);
        }

        private void InitializePhysics()
        {
            lua.RegisterFunction("collision_between", this, new Func<string, string, bool>((name1, name2) => false).Method);
            lua.RegisterFunction("set_gravity", this,  new Action<string, float>((name, value) => { }).Method);
            lua.RegisterFunction("set_restitution", this, new Action<string, float>((name, value) => { }).Method);
            lua.RegisterFunction("get_floor_material", this, new Func<string, string>(name => "").Method);
        }

        private void InitializeAnimation()
        {
            lua.RegisterFunction("set_anim", this,  new Action<string, string>((name, anim) => GetAnimationController(name).Start(anim, true)).Method);
            lua.RegisterFunction("get_anim", this,  new Func<string, string>(name => Get<AnimationComponent>(name).Name).Method);
            lua.RegisterFunction("get_frame", this,  new Func<string, float>(name =>
                                                          {
                                                              var animationController = GetAnimationController(name);
                                                              return animationController.Time*animationController.Speed;
                                                          }).Method);
            lua.RegisterFunction("get_frame_ratio", this, new Func<string, float>(name => GetAnimationController(name).Speed).Method);
            lua.RegisterFunction("is_anim_finished", this,  new Func<string, bool>(name => Math.Abs(GetAnimationController(name).Time - GetAnimationController(name).Length) < 0.01).Method);
        }
        
        private void InitializeHealth()
        {
            lua.RegisterFunction("get_health", this, new Func<string, int>(name => GetHealthComponent(name).Health).Method);
            lua.RegisterFunction("set_health", this,  new Action<string, int>((name, value) => GetHealthComponent(name).Health = value).Method);
            lua.RegisterFunction("get_wounded", this,  new Func<string, bool>(name => GetHealthComponent(name).IsWounded).Method);
            lua.RegisterFunction("set_wounded", this,  new Action<string, bool>((name, value) => Log.WriteLine(LogLevel.Error, "IsWounded property is read-only. Use set_health instead")).Method);
        }

        private void InitializeSound()
        {
            lua.RegisterFunction("get_sound", this,  new Func<string, string>(name => name).Method);
            lua.RegisterFunction("play_sound", this,  new Action<string, string>((owner, sound) => PlaySound(owner, sound, false, 1f)).Method);
            lua.RegisterFunction("play_sound_loop", this,  new Action<string, string>((owner, sound) => PlaySound(owner, sound, true, 1f)).Method);
            lua.RegisterFunction("play_sound_random_pitch", this,  new Action<string, string>((owner, sound) => PlaySound(owner, sound, false, 0.95f + 0.1f * (float)rand.NextDouble())).Method);
        }

        private void InitializeText()
        {
            lua.RegisterFunction("get_choice", this, new Func<string>(() => "choice").Method);
            lua.RegisterFunction("set_choices", this, GetType().GetMethod("SetChoices"));
        }

        public void SetChoices(params object[] args)
        {
            foreach (var o in args)
            {
                Console.WriteLine(o);
            }
        }

        private double GetAngle(string name1, string name2)
        {
            Quaternion rotation = Get<TransformComponent>(name1).Rotation;
            Quaternion rotation2 = Get<TransformComponent>(name2).Rotation;
            return 2.0 * Math.Acos(rotation.X * rotation2.X + rotation.Y * rotation2.Y + rotation.Z * rotation2.Z + rotation.W * rotation2.W);
        }

        private float Distance(string name1, string name2)
        {
            var transform = Get<TransformComponent>(name1).Transform;
            var transform2 = Get<TransformComponent>(name2).Transform;
            return (transform.Translation - transform2.Translation).Length;
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

        private T Get<T>(string name) where T : Component
        {
            var t = Entity.Find(name).GetComponent(default(T), true);
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
