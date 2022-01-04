using UnityEngine;
using System;


namespace TopDownShooter.Weapons.Bullets
{
    public class BulletRay : ImpactBullet
    {
        [SerializeField] private float _projectileSpeedMultiplier;

        private Vector3 _previousPosition;


        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (Math.Abs(_projectileSpeedMultiplier) > float.Epsilon)
            {
                _speed *= _projectileSpeedMultiplier;
                _rigidbody.AddForce(Transform.forward * (_speed * Time.fixedDeltaTime));
            }

            CheckCollision(_previousPosition);

            _previousPosition = Transform.position;
        }

        protected override void OnTriggerEnter(Collider other) { }


        protected override void InitStart()
        {
            base.InitStart();

            _previousPosition = Transform.position;
        }

        private void CheckCollision(Vector3 position)
        {
            RaycastHit hit;
            Vector3 direction = Transform.position - position;
            Ray ray = new Ray(position, direction);
            float dist = Vector3.Distance(Transform.position, position);

            if (Physics.Raycast(ray, out hit, dist, _targetMask))
            {
                //Transform.position = hit.point;
                Quaternion rot = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                Vector3 pos = hit.point;


                if ((_isLaunched) &&
                    ((1 << hit.collider.gameObject.layer) & _targetMask) != 0)
                {
                    Behaviour.Entity affectedTarget = hit.collider.GetComponent<Behaviour.Entity>();

                    if (affectedTarget)
                    {
                        OnBulletHit();
                        OnTargetHit(affectedTarget);

                        affectedTarget.Health.ModifyHealth(_damage);
                    }
                }

                OnBulletDestroy(hit, true);
            }
        }
    }
}
