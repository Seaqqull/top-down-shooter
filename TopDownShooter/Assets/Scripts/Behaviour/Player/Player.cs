using UnityEngine;


namespace TopDownShooter.Behaviour.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : Entity
    {
        [Header("Locomotion")]
        [SerializeField] private float _movementSpeed = 5f;
        [SerializeField] private float _rotationSpeed = 16f;
        [Header("Aiming")]
        [SerializeField] private Transform _viewPosition;

        private Weapons.WeaponController _weapon;

        public Vector3 MovementDirection { get; private set; }
        public Vector3 ViewDirection { get; private set; }


        protected override void Awake()
        {
            base.Awake();

            _weapon = GetComponent<Weapons.WeaponController>();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            Move(MovementDirection);
            Rotate();

            _weapon.InMove = (MovementDirection.magnitude > float.Epsilon);
        }



        private void Rotate()
        {
            if ((MovementDirection.magnitude > float.Epsilon) &&
                    !_weapon.ActiveOption.ActionOnMove)
            {
                RotateToDirection(MovementDirection, _rotationSpeed);
                _weapon.SetTarget(Vector3.forward);
            }
            else
            {
                RotateToDirection(ViewDirection, _rotationSpeed);
                _weapon.SetTarget(_viewPosition.position + ViewDirection);
            }
        }

        private void Move(Vector3 movementDirection)
        {
            if (movementDirection == Vector3.zero)
                return;

            var targetPosition = _body.position +
                movementDirection * ((_movementSpeed * _weapon.ActiveOption.MovementMultiplier) * Time.deltaTime);

            _body.position = targetPosition;
        }

        protected virtual void RotateToDirection(Vector3 direction, float rotationSpeed)
        {
            direction.y = 0f;

            Vector3 desiredForward = Vector3.RotateTowards(
                Transform.forward,
                direction.normalized,
                rotationSpeed * Time.deltaTime,
                0.1f);

            _body.rotation = Quaternion.LookRotation(desiredForward);
        }


        public void Shoot()
        {
            if (Scene.GameManager.Instance.GameStarted)
                _weapon.Shoot();
        }

        public void Revive()
        {
            _isDead = false;
            _health.ResetHealth();
            _body.velocity = Vector3.zero;
        }
        

        protected override void OnDeath()
        {
            base.OnDeath();

            MovementDirection = Vector3.zero;
            ViewDirection = Vector3.forward;

            Scene.GameManager.Instance.EndGame();
        }        

        public void UpdateView(Vector3 newDirection)
        {
            if(Scene.GameManager.Instance.GameStarted)
                ViewDirection = (newDirection - _viewPosition.position);
        }

        public void ChangeWeaponAmmo(float direction)
        {
            if (Scene.GameManager.Instance.GameStarted)
            {
                if (direction > float.Epsilon)
                    _weapon.ActiveWeapon.NextAmmoType();
                else if(direction < float.Epsilon)
                    _weapon.ActiveWeapon.PreviousAmmoType();
            }
        }

        public void UpdateMovement(Vector3 newDirection)
        {
            if (Scene.GameManager.Instance.GameStarted)
                MovementDirection = newDirection;
        }


        public bool CanChangeWeapon(int index)
        {
            return _weapon.ActiveWeapon.IsActionExecutable && (index >= 0 && index < _weapon.WeaponCount);
        }

        public void SetActiveWeapon(int index)
        {
            _weapon.SetActiveWeapon(index);
        }

        public Weapons.Weapon ChangeWeapon(Weapons.Weapon weapon, int index)
        {
            return _weapon.ChangeWeapon(weapon, index);
        }
    }
}
