using System;
using ComponentKit;
using ComponentKit.Model;

namespace Calcifer.Engine.Scripting
{
    public class HealthComponent : Component
    {
        private int health;
        public event EventHandler<EntityEventArgs> Dying;
        public event EventHandler<EntityEventArgs> Hit;
        public int Health
        {
            get
            {
                return this.health;
            }
            set
            {
                this.DoDamage(this.health - value);
            }
        }
        public int MaxHealth
        {
            get;
            set;
        }
        public bool IsWounded
        {
            get
            {
                return this.Health < this.MaxHealth;
            }
        }
        public bool IsDead
        {
            get
            {
                return this.Health == 0;
            }
        }
        public HealthComponent(int maxHealth)
        {
            this.MaxHealth = maxHealth;
        }
        public void DoDamage(int value)
        {
            if (value > this.Health)
            {
                this.health = 0;
                this.OnDying(new EntityEventArgs(this.Record));
            }
            else
            {
                this.health -= value;
                this.OnHit(new EntityEventArgs(this.Record));
            }
        }
        protected virtual void OnHit(EntityEventArgs e)
        {
            var hit = this.Hit;
            if (hit != null)
            {
                hit(this, e);
            }
        }
        protected virtual void OnDying(EntityEventArgs e)
        {
            var dying = this.Dying;
            if (dying != null)
            {
                dying(this, e);
            }
        }
    }
}