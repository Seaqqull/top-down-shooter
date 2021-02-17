using System.Collections;
using UnityEngine;
using System;


namespace Kool2Play.Behaviour.Enemies
{
    public abstract class Enemy : Entity
    {
#pragma warning disable CS0414, CS0649
        [SerializeField] protected float _movementSpeed = 2.5f;
        [SerializeField] private float _rotationSpeed = 16f;
        [SerializeField] private float _pathUpdateDelay = 0.022f;
        [Header("Target:")]
        [SerializeField] private float _targetUpdateDelay = 0.1f;
        [SerializeField] protected Entity _target;
        [SerializeField] [Utility.OwnAttribute.ReadOnly] private float _distanceToTarget;
        [SerializeField] [Utility.OwnAttribute.ReadOnly] private float _angleToTarget;
        [Header("Attack:")]
        [SerializeField] private float _validationPeriod = 0.022f;
        [SerializeField] private float _attackAngle = 45.0f;
        [SerializeField] private float _attackDistance = 0.5f;        
        [SerializeField] protected bool _defaultAttackCall = true;
        [SerializeField] protected bool _moveInAttack;
        [Header("Damage")]
        [SerializeField] private float _damageEffectTime = 0.5f;
        [Header("Dead")]
        [SerializeField] private Material _deadMaterial;
        [SerializeField] private float _dieTime;
        [SerializeField] private Utility.Variables.StringReference _layerOnDeath;

        [Header("Component References")]
        [SerializeField] protected Renderer _gfxRenderer;
#pragma warning restore CS0414, CS0649

        protected Weapons.Weapon _weapon;
        protected Material _baseMaterial;
        protected Collider _collider;
        protected Color _baseColor;

        private Action<Enemy> _onDead = delegate { };

        private Coroutine _updateCoroutation;
        private Coroutine _targetValidation;

        protected Vector3 _destination;
        protected float _speed;

        private float _targetUpdateElapsed;
        private float _pathUpdateElapsed;
        private bool _isTargetApproved;
        protected bool _performAttack;
        private float _initialMass;
        private bool _attacking;


        protected virtual bool CanAttack
        {
            get
            {
                return !IsAttacks && IsInAttackRange;
            }
        }
        public bool IsInAttackRange
        {
            get { return (_distanceToTarget <= _attackDistance); }
        }
        protected bool IsAttacks
        {
            get
            {
                return (_attacking) ||
                    (!(_weapon?.IsActionExecutable) ?? false);
            }
        }

        public event Action<Enemy> OnDead
        {
            add { this._onDead += value; }
            remove { this._onDead -= value; }
        }
        public float AttackDistance
        {
            get { return this._attackDistance; }
        }


        protected override void Awake()
        {
            base.Awake();

            _collider = GetComponent<Collider>();
            _weapon = GetComponentInChildren<Weapons.Weapon>();

            _initialMass = _body.mass;            
            _baseMaterial = _gfxRenderer.material;
            _baseColor = _gfxRenderer.material.GetColor(Utility.Constants.Material.BASE_COLOR);
        }

        private void OnEnable()
        {
            SetActive(true);
        }

        private void OnDisable()
        {
            SetActive(false);
        }


        private IEnumerator TargetValidation(float delay)
        {
            var wait = new WaitForSeconds(delay);

            while (true)
            {
                if (_target != null)
                {
                    _isTargetApproved = IsTargetDirect(Transform.position, Transform.forward, _target.AimPosition, precision: _attackAngle);
                    if (IsInAttackRange && !_isTargetApproved)
                    {
                        Vector3 targetDirection = (_target.AimPosition - Transform.position);
                        RotateToDirection(targetDirection, _rotationSpeed);
                    }
                }

                yield return wait;
            }
        }

        private void UpdateTarget(bool isImmediate = false)
        {
            if ((!isImmediate) &&
                (_targetUpdateElapsed < _targetUpdateDelay))
                return;

            OnUpdateTarget();

            _targetUpdateElapsed = 0.0f;
        }

        protected virtual void RotateToDirection(Vector3 direction, float rotationSpeed)
        {
            direction.y = 0f;

            Vector3 desiredForward = Vector3.RotateTowards(
                Transform.forward,
                direction.normalized,
                rotationSpeed * Time.deltaTime,
                0.1f);

            Transform.rotation = Quaternion.LookRotation(desiredForward);
        }

        private void UpdatePath(Vector3 destination, float speed = float.MinValue, bool isImmediate = false)
        {
            if ((_pathUpdateDelay < 0.0f) ||
                ((!isImmediate) &&
                 (_pathUpdateElapsed < _pathUpdateDelay)))
                return;

            _destination = destination;
            _speed = speed;

            OnUpdatePath();

            _pathUpdateElapsed = 0.0f;
        }

        private bool IsTargetDirect(Vector3 from, Vector3 direction, Vector3 target, float directAngle = 0.0f, float precision = 5.0f)
        {
            Vector3 targetDirection = (target - from);

            float angle = Vector2.Angle(new Vector2(targetDirection.x, targetDirection.z),
                new Vector2(direction.x, direction.z));
            return ((angle >= (directAngle - precision)) && (angle <= (directAngle + precision)));
        }


        protected virtual void Attack()
        {
            _body.mass = _initialMass *
                Utility.Constants.Action.ATTACK_MASS_NULTIPLIER;
        }

        protected override void OnDeath()
        {
            base.OnDeath();

            gameObject.layer = LayerMask.NameToLayer(_layerOnDeath);

            _body.isKinematic = true;
            _body.detectCollisions = false;

            _collider.enabled = false;

            _attacking = false;
            _performAttack = false;

            _onDead.Invoke(this);
            _onDead = delegate { };

            SetActive(false);

            StartCoroutine(OnDeadWithTime());
        }

        protected virtual void OnEndAttack()
        {
            _attacking = false;
            _body.mass = _initialMass;
        }

        protected virtual void OnBeginAttack()
        {
            if (_defaultAttackCall)
                _weapon.Shoot();

            _performAttack = false;
            _attacking = true;
        }

        protected override void OnHealthMinus()
        {
            base.OnHealthMinus();

            if (!_isDead)
                StartCoroutine(OnTakingDamage());
        }

        protected virtual void OnUpdateTarget()
        {
            // If not yet used the permission to attack
            if (_performAttack) return;

            if (_target is null)
            {
                _distanceToTarget = float.MaxValue;
                return;
            }
            else
                _distanceToTarget = (_target.Position - Transform.position).sqrMagnitude;

            if (_target && CanAttack && _isTargetApproved)
            {
                _performAttack = true;
            }
        }

        protected virtual IEnumerator OnDeadWithTime()
        {
            float transparency;
            float elapsed = 0.0f;


            _gfxRenderer.material = _deadMaterial;
            Color clrOrigin = _deadMaterial.GetColor(Utility.Constants.Material.BASE_COLOR);
            Color clr = clrOrigin;

            while (elapsed < _dieTime)
            {
                elapsed += Time.deltaTime;

                transparency = 1 - (elapsed / _dieTime);
                clr.a = transparency < 0 ? 0 : transparency;

                _gfxRenderer.material.SetColor(Utility.Constants.Material.BASE_COLOR, clr);

                yield return null;
            }

            OnPostDeath();
        }

        protected virtual IEnumerator OnTakingDamage()
        {
            float elapsed = 0.0f;

            while (elapsed < _damageEffectTime)
            {
                if (_isDead) break;

                elapsed += Time.deltaTime;

                yield return null;
            }

            OnPostTakingDamage();
        }

        protected virtual void SetActive(bool isActive)
        {
            if (!isActive)
            {
                StopAllCoroutines();
            }
            else
            {
                _targetValidation = StartCoroutine(TargetValidation(_validationPeriod));
                _updateCoroutation = StartCoroutine(UpdateTargets(_targetUpdateDelay));
            }
        }


        protected void Update()
        {
            if (_pathUpdateElapsed < _pathUpdateDelay)
                _pathUpdateElapsed += Time.deltaTime;
            if (_targetUpdateElapsed < _targetUpdateDelay)
                _targetUpdateElapsed += Time.deltaTime;

            if (_isDead) return;

            if (_performAttack)
                Attack();

            if (_target is null)
                UpdatePath(Transform.position, 0.0f);
            else if (_performAttack || (_attacking && !_moveInAttack) || IsInAttackRange)
                UpdatePath(Transform.position, 0.0f, isImmediate: true);
            else
                UpdatePath(_target.Position, _movementSpeed);
        }

        protected IEnumerator UpdateTargets(float delay)
        {
            var wait = new WaitForSeconds(delay);

            while (true)
            {
                UpdateTarget();
                yield return wait;                
            }
        }


        protected abstract void OnPostDeath();
        protected abstract void OnUpdatePath();
        protected abstract void OnPostTakingDamage();


        public void SetTarget(Entity target)
        {
            _target = target;
            UpdateTarget(true);
        }
    }
}
