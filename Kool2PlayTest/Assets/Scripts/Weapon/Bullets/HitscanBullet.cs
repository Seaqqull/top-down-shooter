using System.Collections;
using System.Linq;
using UnityEngine;



namespace Kool2Play.Weapons.Bullets
{
    // Required component
    public abstract class HitscanBullet : RandomImpactBullet
    {
        [SerializeField] private int _damageCount;
        [SerializeField] private float _periodBetweenDamage;


        protected override void OnTriggerEnter(Collider other)
        { }


        protected override void InitStart()
        {
            base.InitStart();
            StartCoroutine(CheckDamage());
        }


        private IEnumerator CheckDamage()
        {
            int damageCount = 0;
            var updateDelay =
                new WaitForSeconds(_periodBetweenDamage);

            while (true)
            {
                // Here could be used connection with [GameManager], 
                // but in order to make bullets completely separate from other modules I decided not to do so
                var targets = FindObjectsOfType<Behaviour.Entity>().Where((target) => {
                    return (
                        (((1 << target.GameObj.layer) & _targetMask) != 0) && 
                        CheckTargetAffectedness(target)
                    );
                });

                for (int i = 0; i < targets.Count(); i++)
                {
                    var target = targets.ElementAt(i);

                    OnBulletHit();
                    OnTargetHit(target);

                    target.Health.ModifyHealth(_damage);
                }

                if (++damageCount < _damageCount)
                    yield return updateDelay;
                else
                    break;
            }

            Destroy(GameObj);
        }

        protected abstract bool CheckTargetAffectedness(Behaviour.Entity entity);
    }
}
