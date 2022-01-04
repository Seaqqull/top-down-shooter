using UnityEngine;


namespace TopDownShooter.Utility.Base
{
    public class BaseMonoBehaviour : MonoBehaviour
    {
        public GameObject GameObj { get; private set; }
        public Transform Transform { get; private set; }
        public Quaternion Rotation
        {
            get { return Transform.rotation; }
            set { Transform.rotation = value; }
        }
        public Vector3 Position 
        {
            get { return Transform.position; } 
        }
        public Vector3 Forward 
        { 
            get { return Transform.forward; } 
        }


        protected virtual void Awake()
        {
            Transform = transform;
            GameObj = gameObject;
        }
    }
}
