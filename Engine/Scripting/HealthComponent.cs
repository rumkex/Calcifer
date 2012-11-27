using System;
using ComponentKit;
using ComponentKit.Model;

namespace Calcifer.Engine.Scripting
{
    public class HealthComponent : Component
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
    }
}