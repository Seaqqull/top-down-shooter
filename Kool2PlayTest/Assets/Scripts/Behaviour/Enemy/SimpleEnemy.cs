using Pathfinding;
using UnityEngine;


namespace Kool2Play.Behaviour.Enemies
{
    public class SimpleEnemy : EnemyPoolable
    {
        protected Animator _animator;
        protected AIPath _ai;


        protected override void Awake()
        {
            base.Awake();

            _animator = GetComponent<Animator>();
            _ai = GetComponent<AIPath>();
        }


        protected override void Attack()
        {
            base.Attack();

            _animator.SetBool(Utility.Constants.Animation.ATTACK_TRIGGER, true);
        }

        protected override void OnDeath()
        {
            base.OnDeath();

            _animator.SetTrigger(Utility.Constants.Animation.DEAD_TRIGGER);
        }

        protected override void OnUpdatePath()
        {
            _ai.destination = _destination;

            if (_speed >= 0.0f)
                _ai.maxSpeed = _speed;

            if (Mathf.Approximately(_ai.velocity.magnitude, 0.0f))
                _animator.SetBool(Utility.Constants.Animation.CHAISING_TRIGGER, false);
            else
                _animator.SetBool(Utility.Constants.Animation.CHAISING_TRIGGER, true);
        }

        protected override void OnInitialize()
        {
            _ai.enabled = true;
        }

        protected override void OnPostTakingDamage()
        {
            // Some event for enemy
            // For example
            // if enemy has low health then some parameters can be raised
        }


        public void AttackCallback()
        {
            OnBeginAttack();
        }

        public void EndAttackCallback()
        {
            OnEndAttack();
            _animator.SetBool(Utility.Constants.Animation.ATTACK_TRIGGER, false);
        }

    }
}
