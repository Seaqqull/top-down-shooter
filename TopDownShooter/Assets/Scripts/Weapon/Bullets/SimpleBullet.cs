using UnityEngine;


namespace Kool2Play.Weapons.Bullets
{
    public class SimpleBullet : ImpactBullet
    {
        protected override void OnTriggerEnter(Collider other)
        {
            if ((_isLaunched) &&
                ((1 << other.gameObject.layer) & _targetMask) != 0)
            {
                Behaviour.Entity affectedTarget = other.GetComponent<Behaviour.Entity>();

                if (affectedTarget)
                {
                    OnBulletHit();
                    OnTargetHit(affectedTarget);

                    affectedTarget.Health.ModifyHealth(_damage);
                }
            }

            OnBulletDestroy();
        }

    }
}
