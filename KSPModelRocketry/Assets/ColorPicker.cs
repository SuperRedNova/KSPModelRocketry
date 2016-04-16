using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

namespace ColorPicker
{
    [AddComponentMenu("UI/ColorPicker")]
    [RequireComponent(typeof(RectTransform))]
    public class ColorPicker : MonoBehaviour
    {

        [Serializable]
        public class ColorEvent : UnityEvent<Color> { }

        [SerializeField]
        private ColorEvent m_OnColorChange = new ColorEvent();
        public ColorEvent OnColorChange { get { return m_OnColorChange; } set { m_OnColorChange = value; } }

        [SerializeField]
        private Image m_ColorOutput;
        public Image ColorOutput { get { return m_ColorOutput; } set { if (SetPropertyUtility.SetClass(ref m_ColorOutput,value)) { } } }

        [SerializeField]
        private Color m_color;
        public Color color { get { return m_color; } set { Set(value); } }

        [SerializeField]
        private float Hue;


        void Set(Color input)
        {
            if (m_color == input)
                return;
            m_color = input;
            m_OnColorChange.Invoke(input);
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    } 
}
