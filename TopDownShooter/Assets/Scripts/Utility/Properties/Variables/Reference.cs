using UnityEngine;


namespace Kool2Play.Utility.Variables
{
    public class Reference<TClass, Tvariable> where TClass : Variable<Tvariable>
    {
        [SerializeField] private bool UseConstant = true;
        [SerializeField] private Tvariable ConstantValue;
#pragma warning disable 0649
        [SerializeReference] private TClass Variable;
#pragma warning restore 0649

        public Tvariable Value
        {
            get { return UseConstant ? ConstantValue : Variable.Value; }
        }


        public Reference() { }

        public Reference(Tvariable value)
        {
            UseConstant = true;
            ConstantValue = value;
        }


        public static implicit operator Tvariable(Reference<TClass, Tvariable> reference)
        {
            return reference.Value;
        }
    }
}
