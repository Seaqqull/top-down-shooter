using UnityEngine;


namespace Kool2Play.Utility.Constants
{
    public class Animation
    {
        public static readonly int JUMP_TRIGGER = Animator.StringToHash("Jump");
        public static readonly int RUN_TRIGGER = Animator.StringToHash("IsRun");
        public static readonly int DEAD_TRIGGER = Animator.StringToHash("IsDead");
        public static readonly int ATTACK_TRIGGER = Animator.StringToHash("IsAttack");
        public static readonly int VERTICAL_TRIGGER = Animator.StringToHash("Vertical");
        public static readonly int CHAISING_TRIGGER = Animator.StringToHash("IsChaising");
        public static readonly int HORIZONTAL_TRIGGER = Animator.StringToHash("Horizontal");
        public static readonly int ANIMATION_SPEED_TRIGGER = Animator.StringToHash("AnimationSpeed");
    }

    public class Material
    {
        public static readonly int BASE_COLOR = Shader.PropertyToID("_BaseColor");
    }

    public class Action
    {
        public static readonly float ATTACK_MASS_NULTIPLIER = 100.0f;
        public static readonly float RESET_DELAY = 2.0f;
    }
}
