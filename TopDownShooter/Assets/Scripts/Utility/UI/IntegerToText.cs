using UnityEngine;


namespace TopDownShooter.Utility.UI
{
    public class IntegerToText : MonoBehaviour
    {
        [SerializeField] private Variables.IntegerReference _value;
        [SerializeField] private TMPro.TMP_Text _text;


        public void UpdateText()
        {
            _text.text = _value.Value.ToString();
        }
    }
}
