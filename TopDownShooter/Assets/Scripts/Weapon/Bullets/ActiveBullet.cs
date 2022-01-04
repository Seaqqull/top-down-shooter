using UnityEngine;


namespace TopDownShooter.Weapons.Bullets
{
    [System.Serializable]
    [RequireComponent(typeof(Rigidbody))]
    public abstract class ActiveBullet : Utility.Base.BaseMonoBehaviour
    {
        protected Data.WeaponAction ShotAction;
        protected Data.WeaponAction FlyAction;
        protected Data.WeaponAction CollisionAction;
        protected Data.WeaponAction DestroyAction;

        protected int _damage;
        protected float _speed;
        protected float _range;
        protected LayerMask _targetMask;
        protected Rigidbody _rigidbody;

        protected bool _isLaunched;
        protected bool _lookRotation;

        protected bool _isAwakeInited;
        protected bool _isStartInited;

        protected Vector3 _startPosition;


        protected override void Awake()
        {
            base.Awake();

            InitAwake();
        }

        protected virtual void Start()
        {
            InitStart();
        }

        protected virtual void FixedUpdate()
        {
            if (!_isAwakeInited) InitAwake();
            if (!_isStartInited) InitStart();

            if ((!_isLaunched) ||
                (_speed == 0.0f)) return;

            if (PassedDistance())
                OnBulletDestroy();

            if (_lookRotation && _rigidbody.velocity != Vector3.zero)
            {
                Transform.rotation = Quaternion.LookRotation(_rigidbody.velocity);
            }
        }


        protected abstract void OnTriggerEnter(Collider other);
        protected abstract void OnTargetHit(Behaviour.Entity entity);


        protected bool PassedDistance()
        {
            return (_range < 0.0f) ? false :
                (_startPosition - Position).sqrMagnitude > _range;
        }

        protected virtual void InitAwake()
        {
            _isAwakeInited = true;

            _rigidbody = GetComponent<Rigidbody>();
        }

        protected virtual void InitStart()
        {
            _isStartInited = true;

            _startPosition = Transform.position;
            _rigidbody.AddForce(Transform.forward * _speed);
        }

        protected virtual void OnBulletHit()
        {
            // Emmit collision particles            

            if (CollisionAction.Audio)
                CollisionAction.Audio.Play();
        }

        protected virtual void OnBulletStart()
        {
            if (ShotAction.Audio)
                ShotAction.Audio.Play();

            if (FlyAction.Audio)
                FlyAction.Audio.Play();
        }

        protected virtual void OnBulletDestroy()
        {
            Destroy(GameObj);
        }

        protected virtual void OnBulletDestroy(Collider hit, bool spawnInHit = false)
        {

            if (FlyAction.Audio)
                FlyAction.Audio.Stop();


            if (DestroyAction.Audio)
                DestroyAction.Audio.Play();

            if (DestroyAction.Particle)
                SpawnOnPosition(DestroyAction.Particle, hit.transform, spawnInHit);

            OnBulletDestroy();
        }

        protected virtual void OnBulletDestroy(RaycastHit hit, bool spawnInHit = false)
        {
            if (FlyAction.Audio)
                FlyAction.Audio.Stop();

            if (DestroyAction.Audio)
                DestroyAction.Audio.Play();

            if (DestroyAction.Particle)
                SpawnOnPosition(DestroyAction.Particle, hit, spawnInHit);

            OnBulletDestroy();
        }

        protected void SpawnOnPosition(GameObject objectToSpawn, Transform hit, bool spawnInHit = false)
        {
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, hit.transform.position.normalized);
            Vector3 pos = hit.transform.position;

            GameObject spawnedObj = Instantiate(objectToSpawn, pos, rot);
            if (spawnInHit)
            {
                spawnedObj.transform.SetParent(hit);
            }
        }

        protected void SpawnOnPosition(GameObject objectToSpawn, RaycastHit hit, bool spawnInHit = false)
        {
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, hit.normal);
            Vector3 pos = hit.point;

            GameObject spawnedObj = Instantiate(objectToSpawn, pos, rot);
            if (spawnInHit)
            {
                spawnedObj.transform.SetParent(hit.collider.transform);
            }
        }


        public void Lunch()
        {
            _isLaunched = true;

            OnBulletStart();
        }

        public void BakeData(Bullet bullet)
        {
            this.CollisionAction = bullet.CollisionAction;
            this.DestroyAction = bullet.DestroyAction;
            this.ShotAction = bullet.ShotAction;
            this.FlyAction = bullet.FlyAction;

            this._lookRotation = bullet.LookRotation;
            this._damage = bullet.Damage;
            this._speed = bullet.Speed;
            this._range = bullet.Range;

            this._targetMask = bullet.TargetMask;
        }

        public void BakeFlowDirection(Transform bulletFlow)
        {
            Transform.position = bulletFlow.position;
            Transform.rotation = bulletFlow.rotation;
        }

        public void BakeFlowDirection(System.Tuple<Vector3, Vector3> bulletFlow)
        {
            Transform.position = bulletFlow.Item1;
            Transform.rotation = Quaternion.LookRotation((bulletFlow.Item2 - bulletFlow.Item1).normalized);
        }
    }
}
