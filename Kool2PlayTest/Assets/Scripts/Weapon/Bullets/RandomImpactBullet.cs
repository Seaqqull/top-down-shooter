using UnityEngine;


namespace Kool2Play.Weapons.Bullets
{
    public abstract class RandomImpactBullet : ImpactBullet
    {
        [SerializeField, Range(0.0f, 1.0f)] protected float _impactChance = 1.0f;

        public float ImpactChance
        {
            get { return this._impactChance; }
        }


        protected override void OnTargetHit(Behaviour.Entity target)
        {
            if (Random.Range(0.0f, 1.0f) <= _impactChance)
                target.ApplyForce(Transform.forward * ImpactPower);
        }
    }
}
