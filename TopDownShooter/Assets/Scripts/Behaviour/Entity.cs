using System.Collections;
using UnityEngine;
using System;


namespace Kool2Play.Behaviour
{
    [Serializable]
    public class Entity : Utility.Base.BaseMonoBehaviour, Utility.IRunLater
    {
        // Restriction of attached [Health] components to [Entity]
        [SerializeField] [Utility.OwnAttribute.ReadOnly] protected Kool2Play.Behaviour.Health _health;
        [SerializeField] protected Utility.Points.AimPoint _aimPoint;

        private static int _idCnter;
        
        protected bool _movementPossible;
        protected Rigidbody _body;
        protected bool _isDead;
        private int _id;

        public Kool2Play.Behaviour.Health Health
        {
            get { return this._health; }
        }
        public Vector3 AimPosition
        {
            get { return this._aimPoint?.Position ?? Transform.position; }
        }
        protected Rigidbody Body
        {
            get { return this._body; }
        }
        public bool IsDead
        {
            get { return this._isDead; }
        }
        // Can be used to prevert double damage, by using list with affected entities that have only distinct id's
        public int Id
        {
            get { return this._id; }
        }


        protected override void Awake()
        {
            base.Awake();

            _id = _idCnter++;
            _isDead = false;

            _health = GetComponent<Health>();
            

            if (_health == null)
                _health = Health.HealthNo.Instance;
            else
            {
                _health.OnHealthMinus += OnHealthMinus;
                _health.OnHealthPlus += OnHealthPlus;
            }

            _body = GetComponent<Rigidbody>();

            if (_aimPoint == null)
                _aimPoint = GetComponentInChildren<Utility.Points.AimPoint>();
        }

        protected virtual void FixedUpdate()
        {
            if (_movementPossible) return;

            if (_body.velocity.magnitude <= 0.1f)
            {
                _movementPossible = true;
            }
        }


        protected virtual void OnDeath()
        {
            _isDead = true;
        }

        protected virtual bool CheckDead()
        {
            return (_isDead)? false : _health.IsZero;
        }

        protected virtual void OnHealthPlus() { }
        protected virtual void OnHealthMinus()
        {
            if (CheckDead())
                OnDeath();
        }


        public virtual void ApplyForce(Vector3 direction, ForceMode force = ForceMode.Force)
        {
            _movementPossible = false;

            _body.AddForce(direction * _body.mass, force);
        }
        public virtual void ApplyExplosionForce(Vector3 position, float power, float radius, ForceMode force = ForceMode.Force)
        {
            _movementPossible = false;

            _body.AddExplosionForce(power, position, radius, 1.0f, force);
        }        


        #region RunLater
        public void RunLater(Action method, float waitSeconds)
        {
            RunLaterValued(method, waitSeconds);
        }

        public Coroutine RunLaterValued(Action method, float waitSeconds)
        {
            if ((waitSeconds < 0) || (method == null))
                return null;

            return StartCoroutine(RunLaterCoroutine(method, waitSeconds));
        }

        public IEnumerator RunLaterCoroutine(Action method, float waitSeconds)
        {
            yield return new WaitForSeconds(waitSeconds);
            method();
        }
        #endregion        
    }
}
