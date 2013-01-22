using System;
using System.Collections.Generic;
using Calcifer.Engine.Scenery;
using Calcifer.Utilities.Logging;
using ComponentKit;
using ComponentKit.Model;

namespace Calcifer.Engine.Scripting
{
    public class HealthComponent : Component, IConstructable
    {
        private int health;

        public HealthComponent() : this(100)
        {}

        public HealthComponent(int maxHealth)
        {
            MaxHealth = maxHealth;
            health = maxHealth;
        }

        public int Health
        {
            get { return health; }
            set { DoDamage(health - value); }
        }

        public int MaxHealth { get; set; }

        public bool IsWounded
        {
            get { return Health < MaxHealth; }
        }

        public bool IsDead
        {
            get { return Health == 0; }
        }

        public event EventHandler<EntityEventArgs> Dying;
        public event EventHandler<EntityEventArgs> Hit;

        public void DoDamage(int value)
        {
            if (value > Health)
            {
                health = 0;
                OnDying(new EntityEventArgs(Record));
            }
            else
            {
                health -= value;
                OnHit(new EntityEventArgs(Record));
            }
            Log.WriteLine(LogLevel.Debug, "{0} received {1} damage. {2} HP left", Record.Name, value, health);
        }

        protected virtual void OnHit(EntityEventArgs e)
        {
            EventHandler<EntityEventArgs> hit = Hit;
            if (hit != null)
            {
                hit(this, e);
            }
        }

        protected virtual void OnDying(EntityEventArgs e)
        {
            EventHandler<EntityEventArgs> dying = Dying;
            if (dying != null)
            {
                dying(this, e);
            }
        }

        void IConstructable.Construct(IDictionary<string, string> param)
        {
            MaxHealth = int.Parse(param["hp"] ?? "100");
            health = MaxHealth;
        }
    }
}