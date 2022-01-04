using UnityEngine;


namespace Kool2Play.Weapons.Bullets
{
    [RequireComponent(typeof(Aiming.Accuracy))]
    public class ConeHitscanBullet : HitscanBullet
    {
        [SerializeField] [Utility.OwnAttribute.ReadOnly] private Aiming.Accuracy _accuracy;


        protected override void InitAwake()
        {
            base.InitAwake();

            _accuracy = GetComponent<Aiming.Accuracy>();
        }


        protected override bool CheckTargetAffectedness(Behaviour.Entity entity)
        {
            return _accuracy.IsInside(entity.AimPosition);
        }
    }
}
