using UnityEngine;


namespace TopDownShooter.Weapons.Bullets
{
    public abstract class ImpactBullet : ActiveBullet
    {
        [SerializeField] private float _impactPower;

        public float ImpactPower
        {
            get { return this._impactPower; }
        }

        protected override void OnTargetHit(Behaviour.Entity target)
        {
            target.ApplyForce(Transform.forward * _impactPower);
        }

    }
}