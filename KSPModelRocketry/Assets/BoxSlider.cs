using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

namespace ColorPicker
{
    [AddComponentMenu("UI/BoxSlider")]
    [RequireComponent(typeof(RectTransform))]
    public class BoxSlider : Selectable, IDragHandler, IInitializePotentialDragHandler, ICanvasElement
    {
        public enum Direction
        {
            LeftToRightAndBottomToTop,
            LeftToRightAndTopToBottom,
            RightToLeftAndBottomToTop,
            RightToLeftAndTopToBottom,
        }

        [Serializable]
        public class BoxSliderXEvent : UnityEvent<float> { }

        [Serializable]
        public class BoxSliderYEvent : UnityEvent<float> { }

        [SerializeField]
        private RectTransform m_HandleRect;
        public RectTransform handleRect { get { return m_HandleRect; } set { if (SetPropertyUtility.SetClass(ref m_HandleRect, value)) { UpdateCachedReferences(); UpdateVisuals(); } } }

        [Space]

        [SerializeField]
        private Direction m_Direction = Direction.LeftToRightAndBottomToTop;
        public Direction direction { get { return m_Direction; } set { if (SetPropertyUtility.SetStruct(ref m_Direction, value)) UpdateVisuals(); } }

        [SerializeField]
        private float m_MinXValue = 0;
        public float minXValue { get { return m_MinXValue; } set { if (SetPropertyUtility.SetStruct(ref m_MinXValue, value)) { SetX(m_XValue); UpdateVisuals(); } } }

        [SerializeField]
        private float m_MinYValue = 0;
        public float minYValue { get { return m_MinYValue; } set { if (SetPropertyUtility.SetStruct(ref m_MinYValue, value)) { SetY(m_YValue); UpdateVisuals(); } } }

        [SerializeField]
        private float m_MaxXValue = 1;
        public float maxXValue { get { return m_MaxXValue; } set { if (SetPropertyUtility.SetStruct(ref m_MaxXValue, value)) { SetX(m_XValue); UpdateVisuals(); } } }

        [SerializeField]
        private float m_MaxYValue = 1;
        public float maxYValue { get { return m_MaxYValue; } set { if (SetPropertyUtility.SetStruct(ref m_MaxYValue, value)) { SetY(m_YValue); UpdateVisuals(); } } }

        [SerializeField]
        private bool m_XWholeNumbers = false;
        public bool XwholeNumbers { get { return m_XWholeNumbers; } set { if (SetPropertyUtility.SetStruct(ref m_XWholeNumbers, value)) { SetX(m_XValue); UpdateVisuals(); } } }

        [SerializeField]
        private bool m_YWholeNumbers = false;
        public bool YwholeNumbers { get { return m_YWholeNumbers; } set { if (SetPropertyUtility.SetStruct(ref m_YWholeNumbers, value)) { SetY(m_YValue); UpdateVisuals(); } } }

        [SerializeField]
        protected float m_XValue;
        public virtual float Xvalue
        {
            get
            {
                if (XwholeNumbers)
                    return Mathf.Round(m_XValue);
                return m_XValue;
            }
            set
            {
                SetX(value);
            }
        }

        public float normalizedXValue
        {
            get
            {
                if (Mathf.Approximately(minXValue, maxXValue))
                    return 0;
                return Mathf.InverseLerp(minXValue, maxXValue, Xvalue);
            }
            set
            {
                Xvalue = Mathf.Lerp(minXValue, maxXValue, value);
            }
        }

        [SerializeField]
        protected float m_YValue;
        public virtual float Yvalue
        {
            get
            {
                if (YwholeNumbers)
                    return Mathf.Round(m_YValue);
                return m_YValue;
            }
            set
            {
                SetY(value);
            }
        }

        public float normalizedYValue
        {
            get
            {
                if (Mathf.Approximately(minYValue, maxYValue))
                    return 0;
                return Mathf.InverseLerp(minYValue, maxYValue, Yvalue);
            }
            set
            {
                Yvalue = Mathf.Lerp(minYValue, maxYValue, value);
            }
        }

        [Space]

        // Allow for delegate-based subscriptions for faster events than 'eventReceiver', and allowing for multiple receivers.
        [SerializeField]
        private BoxSliderXEvent m_OnXValueChanged = new BoxSliderXEvent();
        public BoxSliderXEvent onXValueChanged { get { return m_OnXValueChanged; } set { m_OnXValueChanged = value; } }

        [SerializeField]
        private BoxSliderYEvent m_OnYValueChanged = new BoxSliderYEvent();
        public BoxSliderYEvent onYValueChanged { get { return m_OnYValueChanged; } set { m_OnYValueChanged = value; } }

        // Private fields

        private Transform m_HandleTransform;
        private RectTransform m_HandleContainerRect;

        // The offset from handle position to mouse down position
        private Vector2 m_Offset = Vector2.zero;

        private DrivenRectTransformTracker m_Tracker;

        // Size of each step.
        float XstepSize { get { return XwholeNumbers ? 1 : (maxXValue - minXValue) * 0.1f; } }
        float YstepSize { get { return YwholeNumbers ? 1 : (maxYValue - minYValue) * 0.1f; } }

        protected BoxSlider()
        { }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (XwholeNumbers)
            {
                m_MinXValue = Mathf.Round(m_MinXValue);
                m_MaxXValue = Mathf.Round(m_MaxXValue);
            }

            if (YwholeNumbers)
            {
                m_MinYValue = Mathf.Round(m_MinYValue);
                m_MaxYValue = Mathf.Round(m_MaxYValue);
            }

            //Onvalidate is called before OnEnabled. We need to make sure not to touch any other objects before OnEnable is run.
            if (IsActive())
            {
                UpdateCachedReferences();
                SetX(m_XValue, false);
                SetY(m_YValue, false);
                // Update rects since other things might affect them even if value didn't change.
                UpdateVisuals();
            }

            var prefabType = UnityEditor.PrefabUtility.GetPrefabType(this);
            if (prefabType != UnityEditor.PrefabType.Prefab && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

#endif // if UNITY_EDITOR

        public virtual void Rebuild(CanvasUpdate executing)
        {
#if UNITY_EDITOR
            if (executing == CanvasUpdate.Prelayout)
            {
                onXValueChanged.Invoke(Xvalue);
                onYValueChanged.Invoke(Yvalue);
            }

#endif
        }

        public virtual void LayoutComplete()
        { }

        public virtual void GraphicUpdateComplete()
        { }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateCachedReferences();
            SetX(m_XValue, false);
            SetY(m_YValue, false);
            // Update rects since they need to be initialized correctly.
            UpdateVisuals();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            base.OnDisable();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            // Has value changed? Various elements of the slider have the old normalisedValue assigned, we can use this to perform a comparison.
            // We also need to ensure the value stays within min/max.
            m_XValue = ClampXValue(m_XValue);
            m_YValue = ClampYValue(m_YValue);
            float oldNormalizedXValue = normalizedXValue;
            float OldNormalizedYValue = normalizedYValue;
            if (m_HandleContainerRect != null)
            {
                oldNormalizedXValue = (reverseXValue ? 1 - m_HandleRect.anchorMin.x : m_HandleRect.anchorMin.x);
                OldNormalizedYValue = (reverseYValue ? 1 - m_HandleRect.anchorMin.y : m_HandleRect.anchorMin.y);
            }


            UpdateVisuals();

            if (oldNormalizedXValue != normalizedXValue)
                onXValueChanged.Invoke(m_XValue);
            if (OldNormalizedYValue != normalizedYValue)
                onYValueChanged.Invoke(m_YValue);
        }

        void UpdateCachedReferences()
        {
            if (m_HandleRect)
            {
                m_HandleTransform = m_HandleRect.transform;
                if (m_HandleTransform.parent != null)
                    m_HandleContainerRect = m_HandleTransform.parent.GetComponent<RectTransform>();
            }
            else
            {
                m_HandleContainerRect = null;
            }
        }

        float ClampXValue(float input)
        {
            float newValue = Mathf.Clamp(input, minXValue, maxXValue);
            if (XwholeNumbers)
                newValue = Mathf.Round(newValue);
            return newValue;
        }

        float ClampYValue(float input)
        {
            float newValue = Mathf.Clamp(input, minYValue, maxYValue);
            if (YwholeNumbers)
                newValue = Mathf.Round(newValue);
            return newValue;
        }

        // Set the valueUpdate the visible Image.
        void SetX(float input)
        {
            SetX(input, true);
        }

        protected virtual void SetX(float input, bool sendCallback)
        {
            // Clamp the input
            float newValue = ClampXValue(input);

            // If the stepped value doesn't match the last one, it's time to update
            if (m_XValue == newValue)
                return;

            m_XValue = newValue;
            UpdateVisuals();
            if (sendCallback)
                m_OnXValueChanged.Invoke(newValue);
        }

        // Set the valueUpdate the visible Image.
        void SetY(float input)
        {
            SetY(input, true);
        }

        protected virtual void SetY(float input, bool sendCallback)
        {
            // Clamp the input
            float newValue = ClampYValue(input);

            // If the stepped value doesn't match the last one, it's time to update
            if (m_YValue == newValue)
                return;

            m_YValue = newValue;
            UpdateVisuals();
            if (sendCallback)
                m_OnYValueChanged.Invoke(newValue);
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            //This can be invoked before OnEnabled is called. So we shouldn't be accessing other objects, before OnEnable is called.
            if (!IsActive())
                return;

            UpdateVisuals();
        }

        bool reverseXValue { get { return m_Direction == Direction.RightToLeftAndTopToBottom || m_Direction == Direction.RightToLeftAndBottomToTop; } }
        bool reverseYValue { get { return m_Direction == Direction.RightToLeftAndTopToBottom || m_Direction == Direction.LeftToRightAndTopToBottom; } }

        // Force-update the slider. Useful if you've changed the properties and want it to update visually.
        private void UpdateVisuals()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UpdateCachedReferences();
#endif

            m_Tracker.Clear();

            if (m_HandleContainerRect != null)
            {
                m_Tracker.Add(this, m_HandleRect, DrivenTransformProperties.Anchors);
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;
                anchorMin.x = anchorMax.x = (reverseXValue ? (1 - normalizedXValue) : normalizedXValue);
                anchorMin.y = anchorMax.y = (reverseYValue ? (1 - normalizedYValue) : normalizedYValue);
                m_HandleRect.anchorMin = anchorMin;
                m_HandleRect.anchorMax = anchorMax;
            }
        }

        // Update the slider's position based on the mouse.
        void UpdateDrag(PointerEventData eventData, Camera cam)
        {
            RectTransform clickRect = m_HandleContainerRect;
            if (clickRect != null && clickRect.rect.size.x > 0 && clickRect.rect.size.y > 0)
            {
                Vector2 localCursor;
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, eventData.position, cam, out localCursor))
                    return;
                localCursor -= clickRect.rect.position;

                float xval = Mathf.Clamp01((localCursor - m_Offset).x / clickRect.rect.size.x);
                normalizedXValue = (reverseXValue ? 1f - xval : xval);
                float yval = Mathf.Clamp01((localCursor - m_Offset).y / clickRect.rect.size.y);
                normalizedYValue = (reverseYValue ? 1f - yval : yval);
            }
        }

        private bool MayDrag(PointerEventData eventData)
        {
            return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            base.OnPointerDown(eventData);

            m_Offset = Vector2.zero;
            if (m_HandleContainerRect != null && RectTransformUtility.RectangleContainsScreenPoint(m_HandleRect, eventData.position, eventData.enterEventCamera))
            {
                Vector2 localMousePos;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_HandleRect, eventData.position, eventData.pressEventCamera, out localMousePos))
                    m_Offset = localMousePos;
            }
            else
            {
                // Outside the slider handle - jump to this point instead
                UpdateDrag(eventData, eventData.pressEventCamera);
            }
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;
            UpdateDrag(eventData, eventData.pressEventCamera);
        }

        public override void OnMove(AxisEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
            {
                base.OnMove(eventData);
                return;
            }

            switch (eventData.moveDir)
            {
                case MoveDirection.Left:
                    if (FindSelectableOnLeft() == null)
                        SetX(reverseXValue ? Xvalue + XstepSize : Xvalue - XstepSize);
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Right:
                    if (FindSelectableOnRight() == null)
                        SetX(reverseXValue ? Xvalue - XstepSize : Xvalue + XstepSize);
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Up:
                    if (FindSelectableOnUp() == null)
                        SetY(reverseYValue ? Yvalue - YstepSize : Yvalue + YstepSize);
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Down:
                    if (FindSelectableOnDown() == null)
                        SetY(reverseYValue ? Yvalue + YstepSize : Yvalue - YstepSize);
                    else
                        base.OnMove(eventData);
                    break;
            }
        }

        public override Selectable FindSelectableOnLeft()
        {
            if (navigation.mode == Navigation.Mode.Automatic)
                return null;
            return base.FindSelectableOnLeft();
        }

        public override Selectable FindSelectableOnRight()
        {
            if (navigation.mode == Navigation.Mode.Automatic)
                return null;
            return base.FindSelectableOnRight();
        }

        public override Selectable FindSelectableOnUp()
        {
            if (navigation.mode == Navigation.Mode.Automatic)
                return null;
            return base.FindSelectableOnUp();
        }

        public override Selectable FindSelectableOnDown()
        {
            if (navigation.mode == Navigation.Mode.Automatic)
                return null;
            return base.FindSelectableOnDown();
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        public void SetDirection(Direction direction, bool includeRectLayouts)
        {
            bool oldReverseX = reverseXValue;
            bool oldReverseY = reverseYValue;
            this.direction = direction;

            if (!includeRectLayouts)
                return;

            if (reverseXValue != oldReverseX)
                RectTransformUtility.FlipLayoutOnAxis(transform as RectTransform, 0, true, true);

            if (reverseYValue != oldReverseY)
                RectTransformUtility.FlipLayoutOnAxis(transform as RectTransform, 1, true, true);
        }
    }

    internal static class SetPropertyUtility
    {
        public static bool SetColor(ref Color currentValue, Color newValue)
        {
            if (currentValue.r == newValue.r && currentValue.g == newValue.g && currentValue.b == newValue.b && currentValue.a == newValue.a)
                return false;

            currentValue = newValue;
            return true;
        }

        public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (currentValue.Equals(newValue))
                return false;

            currentValue = newValue;
            return true;
        }

        public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return false;

            currentValue = newValue;
            return true;
        }
    }
}
