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
        private Slider m_Hueslider;
        public Slider HueSLider { get { return m_Hueslider; } set { if(SetPropertyUtility.SetClass(ref m_Hueslider, value)) { } } }

        [SerializeField]
        private BoxSlider m_SVSlider;
        public BoxSlider HueSatSlider { get { return m_SVSlider; } set { if(SetPropertyUtility.SetClass(ref m_SVSlider, value)) { } } }

        [SerializeField]
        private Image m_ColorOutput;
        public Image ColorOutput { get { return m_ColorOutput; } set { if (SetPropertyUtility.SetClass(ref m_ColorOutput,value)) { } } }

        [SerializeField]
        private Color m_color = Color.red;
        public Color color { get { return m_color; } set { Set(value); } }
        
        private float Hue = 0;
        private float Saturation = 1;
        private float Value = 1;
        
        void Set(Color input, bool Invoke = true)
        {
            if (m_color == input)
                return;
            m_color = input;
            RGBtoHSV(input);
            m_Hueslider.value = Hue;
            updatBoxSlider();
            m_ColorOutput.color = input;
            if(Invoke)
                m_OnColorChange.Invoke(input);
        }

        private void updatBoxSlider()
        {
            m_SVSlider.normalizedXValue = Saturation;
            m_SVSlider.normalizedYValue = Value;
            m_SVSlider.gameObject.GetComponent<Image>().material.color = HSVtoRGB(Hue,1,1);
        }

        public void onHue(float input)
        {
            Set(HSVtoRGB(input, Saturation, Value),false);
        }

        public void onSaturation(float input)
        {
            Set(HSVtoRGB(Hue, input, Value),false);
        }

        public void onValue(float input)
        {
            Set(HSVtoRGB(Hue, Saturation, input),false);
        }

        private void RGBtoHSV(Color input)
        {
            float min, max, delta,newH,newS,newV;
            min = Mathf.Min(input.r, input.g, input.b);
            max = Mathf.Max(input.r, input.g, input.b);
            newV = max;

            delta = min - max;

            if (max != 0)
            {
                newS = delta / max;
                if (input.r == max)
                    newH = (input.g - input.b) / delta;
                else if (input.g == max)
                    newH = 2 + (input.b - input.r) / delta;
                else
                    newH = 4 + (input.r - input.b) / delta;
                newH /= 6; //normalize hue
            }
            else
            {
                newS = 0;
                newH = Hue; //Just keep hue the same
            }
            Hue = newH;
            Saturation = newS;
            Value = newV;
        }

        public Color HSVtoRGB(float hue,float sat,float val)
        {
            Color newColor = Color.red;
            int i;
            float f,b,u,d;
            if (hue > 1) hue /= 360;//Normalize hue if not already input that way.
            hue *= 6;
            i = (int)Mathf.Floor(hue);
            f = hue - i; //factor
            b = val * (1 - sat); //bottom value
            d = val * (1 - sat * f); //value going down
            u = val * (1 - sat * (1 - f)); //value going up
            switch (i)
            {
                case 0:
                    newColor.r = val;
                    newColor.g = u;
                    newColor.b = b;
                    break;
                case 1:
                    newColor.r = d;
                    newColor.g = val;
                    newColor.b = b;
                    break;
                case 2:
                    newColor.r = b;
                    newColor.g = val;
                    newColor.b = u;
                    break;
                case 3:
                    newColor.r = b;
                    newColor.g = d;
                    newColor.b = val;
                    break;
                case 4:
                    newColor.r = u;
                    newColor.g = b;
                    newColor.b = val;
                    break;
                default:
                    newColor.r = val;
                    newColor.g = b;
                    newColor.b = d;
                    break;
            }
            return newColor;
        }
    } 
}
