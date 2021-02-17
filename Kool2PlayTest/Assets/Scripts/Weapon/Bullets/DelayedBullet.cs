using System.Collections.Generic;
using UnityEngine;


namespace Kool2Play.Weapons.Bullets
{
    public class DelayedBullet : RandomImpactBullet
    {
        [SerializeField] [Range(0, ushort.MaxValue)] private float m_lifetime = 0.5f;

        private List<Behaviour.Entity> _affectedEntities;


        protected override void OnTriggerEnter(Collider other)
        {
            if ((!_isLaunched) ||
                (((1 << other.gameObject.layer) & _targetMask) == 0)) return;
            
            if (other.TryGetComponent<Behaviour.Entity>(out var affectedEntity) && 
                affectedEntity && !IsEntityAffected(affectedEntity))
            {
                OnBulletHit();
                OnTargetHit(affectedEntity);

                affectedEntity.Health.ModifyHealth(_damage);
                _affectedEntities.Add(affectedEntity);
            }
        }


        protected override void OnBulletStart()
        {
            base.OnBulletStart();

            _affectedEntities = new List<Behaviour.Entity>();
            Invoke("OnBulletDestroy", m_lifetime);
        }

        private bool IsEntityAffected(Behaviour.Entity hittedEntity)
        {
            for (int i = 0; i < _affectedEntities.Count; i++)
            {
                if (_affectedEntities[i].Id == hittedEntity.Id)
                    return true;
            }
            return false;
        }

    }
}
