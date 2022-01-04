using UnityEngine.Events;
using UnityEngine;
using System;


namespace TopDownShooter.Utility.Input
{
    public class InputController : MonoBehaviour
    {
        [Serializable]
        public class InputUpdateEvent : UnityEvent<Vector3> { }
        [Serializable]
        public class InputUpdateFloatEvent : UnityEvent<float> { }


#pragma warning disable CS0649
        [Header("Keyboard")]
        [SerializeField] private InputUpdateEvent _onMovementUpdate;
        [Header("Mouse")]
        [SerializeField] private InputUpdateFloatEvent _onMouseWheel;
        [SerializeField] private InputUpdateEvent _onMouseUpdate;
        [SerializeField] private UnityEvent _onMouseClick;
        [SerializeField] [Range(0.0f, 50.0f)] private float _maxDistance = 50.0f;
#pragma warning restore CS0649

        private Camera _camera;


        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            // Movement input
            _onMovementUpdate.Invoke(new Vector3(
                UnityEngine.Input.GetAxis("Horizontal"),
                0.0f,
                UnityEngine.Input.GetAxis("Vertical"))
            );

            // Mouse hover
            Ray ray = _camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance))
            {
                _onMouseUpdate.Invoke(hit.point);
            }

            // Mouse click
            if (UnityEngine.Input.GetMouseButtonDown(0))
                _onMouseClick.Invoke();

            // Mouse wheel
            var wheelDelta = UnityEngine.Input.mouseScrollDelta.y;
            
            if (Mathf.Abs(wheelDelta) > float.Epsilon)
            {
                _onMouseWheel.Invoke(wheelDelta);
            }
        }

    }
}
