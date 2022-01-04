using UnityEngine;


namespace TopDownShooter.Utility.Points
{
    public class AimPoint : ScenePoint
    {
#pragma warning disable 0649
        [SerializeField] private Color _color = Color.black;
#pragma warning restore 0649

        public override Color Color 
        { 
            get { return _color; } 
        }
    }
}
