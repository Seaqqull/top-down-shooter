using UnityEngine;


namespace Kool2Play.Utility.Variables
{
    [CreateAssetMenu(menuName = "Variable/Float")]
    public class FloatVariable : Variable<float>
    {
        public void ApplyChange(float amount)
        {
            Value += amount;
        }

        public void ApplyChange(Variable<float> amount)
        {
            Value += amount.Value;
        }
    }
}
