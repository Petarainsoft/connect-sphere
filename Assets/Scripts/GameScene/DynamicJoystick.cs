using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Vector2 = UnityEngine.Vector2;

namespace Common
{
    public class DynamicJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public RectTransform _joystickArea; // The joystick's background component
        public RectTransform _joystickKnob; // The handle
        public float _maxRadius = 50.0f; // Maximum drag radius

        private Vector2 _joystickCenter; // Center position of the joystick

        public UnityEvent<Vector2> OnJoystickValueChanged; // Called when the joystick value changes
        private Vector2 _joystickValue; // Current joystick value
        
        private Vector2 _initialPosition; // Initial position of the joystick
        private void Start()
        {
            if ( _joystickArea == null )
                _joystickArea = GetComponent<RectTransform>();
            if ( _joystickKnob == null && transform.childCount > 0 )
                _joystickKnob = transform.GetChild(0).GetComponent<RectTransform>();

            //_joystickArea.gameObject.SetActive(false);
            _initialPosition = _joystickArea.localPosition;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, eventData.position,
                eventData.pressEventCamera, out var localPoint);
            // Move the entire joystick to the touch position
            _joystickArea.localPosition = localPoint;
            _joystickArea.gameObject.SetActive(true); // Make the joystick visible

            _joystickCenter = localPoint;
            _joystickKnob.anchoredPosition = Vector2.zero; // Reset the knob position

            UpdateJoystick(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            UpdateJoystick(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _joystickArea.gameObject.SetActive(false); // Hide the joystick again

            // Optional: Inform other components that the joystick is released
            OnJoystickReleased();
        }

        private void UpdateJoystick(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, eventData.position,
                eventData.pressEventCamera, out var localPoint);
            Vector2 direction = localPoint - _joystickCenter;
            // Debug.Log($"<color=yellow>joystickCenter is {_joystickCenter}, direction {direction}</color>");
            float distance = direction.magnitude;

            // Limit the handle's movement within the joystick's radius
            Vector2 clampedDirection = direction*(distance > _maxRadius ? _maxRadius/distance : 1);
            _joystickKnob.anchoredPosition = clampedDirection;

            // Debug.Log($"<color=yellow>ClampedDirection {clampedDirection}</color>");
            // Optional: Inform other components about joystick movement
            _joystickValue = clampedDirection/_maxRadius;
        }

        private void Update()
        {
            OnJoystickMoved(_joystickValue);
        }

        // Example callback for joystick use
        private void OnJoystickMoved(Vector2 direction)
        {
            OnJoystickValueChanged?.Invoke(direction);
        }

        // Example callback for joystick release
        private void OnJoystickReleased()
        {
            _joystickValue = Vector2.zero;
        }

        public void ResetJoystick()
        {
            _joystickArea.localPosition = _initialPosition;
            _joystickKnob.anchoredPosition = Vector2.zero;
            _joystickCenter = _initialPosition;
            _joystickValue = Vector2.zero;
        }

        public void SetEnable(bool active)
        {
            _joystickArea.gameObject.SetActive(active);
            enabled = active;
        }
    }
}