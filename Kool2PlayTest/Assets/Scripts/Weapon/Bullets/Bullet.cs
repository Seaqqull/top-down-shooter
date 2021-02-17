using UnityEngine;


namespace Kool2Play.Weapons.Bullets
{
    [CreateAssetMenu(menuName = "Weapon/Bullet/Create")]
    public class Bullet : ScriptableObject
    {
        public GameObject BulletObject;
        public bool LookRotation = true;
        
        public Data.WeaponAction ShotAction;
        public Data.WeaponAction FlyAction;
        public Data.WeaponAction CollisionAction;
        public Data.WeaponAction DestroyAction;

        public int Damage;
        public float Speed;
        public float Range;
        public LayerMask TargetMask;
    }
}
