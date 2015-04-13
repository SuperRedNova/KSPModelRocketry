using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSPModelRocketry
{
    public class VariableTexture : PartModule
    {
        [KSPField(guiActiveEditor = true, guiName = "Red", guiFormat = "F2")]
        [UI_FloatRange(minValue = 0, maxValue = 1, stepIncrement = .05f)]
        public float red = 1f;
        [KSPField(isPersistant = true)]
        private float red1;
        [KSPField(isPersistant = true)]
        private float red2 = 0f;

        [KSPField(guiActiveEditor = true, guiName = "Green", guiFormat = "F2")]
        [UI_FloatRange(minValue = 0, maxValue = 1, stepIncrement = .05f)]
        public float green = 1f;
        [KSPField(isPersistant = true)]
        private float green1;
        [KSPField(isPersistant = true)]
        private float green2 = 0f;

        [KSPField(guiActiveEditor = true, guiName = "Blue", guiFormat = "F2")]
        [UI_FloatRange(minValue = 0, maxValue = 1, stepIncrement = .05f)]
        public float blue = 1f;
        [KSPField(isPersistant = true)]
        private float blue1;
        [KSPField(isPersistant = true)]
        private float blue2 = 0f;

        [KSPField]
        public string textureMeshName = "Variable";
        private Renderer texRend;
        private Material mat;

        [KSPField(guiActiveEditor = true, guiName = "Edit Color:", isPersistant = true)]
        [UI_Toggle(enabledText="Primary",disabledText="Secondary")]
        public bool switchColor = true;
        private bool swtclr;

        [KSPField(guiActiveEditor = true, guiName = "Pattern", isPersistant = true),
        KSPAPIExtensions.UI_ChooseOption(options = new string[] { "Plain", "Quarter 1", "Quarter 2",
            "Quarter 3", "Quarter 4", "Checker", "Half" })]
        public string texturePattern = "Plain";
        private string texpat = "";

        [KSPField(guiActiveEditor = true, guiName = "Texture Repeat", guiFormat = "F2", isPersistant = true)]
        [UI_FloatRange(minValue = 1, maxValue = 4, stepIncrement = 1)]
        public float textureRepeat = 2;
        public float texRep;
        private bool[,] pattern = new bool[2, 2];

        /// <summary>
        /// Raises the start event.
        /// </summary>
        /// <param name="state">State.</param>
        public override void OnAwake()
        {
            setPattern();
            setColor();
            base.OnAwake();
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        public void Update()
        {
            if (switchColor != swtclr)
            {
                if (switchColor)
                {
                    red = red1;
                    green = green1;
                    blue = blue1;
                    swtclr = switchColor;
                }
                else
                {
                    red = red2;
                    green = green2;
                    blue = blue2;
                    swtclr = switchColor;
                }
            }
            if ((HighLogic.LoadedSceneIsEditor || HighLogic.LoadedSceneIsFlight) &&
                ((switchColor ? red != red1 || green != green1 || blue != blue1 :
                red != red2 || green != green2 || blue != blue2) || 
                (!texturePattern.Equals(texpat) || textureRepeat != texRep)))
            {
                setPattern();
                setColor();
            }
        }

        /// <summary>
        /// Changes the color of a mesh of a part implementing this PartModlue based on it's red, green, and blue values.
        /// </summary>
        public void setColor()
        {
            if (texRend == null)
            {
                texRend = part.FindModelComponent<Renderer>(textureMeshName);
                mat = new Material(texRend.material.shader);
            }
            if (switchColor)
            {
                red1 = red;
                green1 = green;
                blue1 = blue;
            }
            else
            {
                red2 = red;
                green2 = green;
                blue2 = blue;
            }
            Texture2D tex = new Texture2D(2 * (int)textureRepeat, 2);
            Color[] colors = new Color[tex.height * tex.width];
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < textureRepeat; k++)
                    {
                        int index = (2 * k) + j + (2 * i * (int)textureRepeat);
                        if (pattern[i, j])
                        {
                            colors[index] = new Color(red2, green2, blue2);
                        }
                        else
                        {
                            colors[index] = new Color(red1, green1, blue1);
                        }

                    }
                }
            }
            tex.SetPixels(colors);
            tex.Apply();
            tex.filterMode = FilterMode.Point;
            mat.mainTexture = tex;
            texRend.material = mat;
        }

        public void setPattern()
        {
            texpat = texturePattern;
            texRep = textureRepeat;
            switch (texturePattern)
            {
                case "Checker":
                    pattern[0, 0] = true;
                    pattern[0, 1] = false;
                    pattern[1, 0] = false;
                    pattern[1, 1] = true;
                    break;
                case "Quarter 1":
                    pattern[0, 0] = true;
                    pattern[0, 1] = false;
                    pattern[1, 0] = false;
                    pattern[1, 1] = false;
                    break;
                case "Quarter 2":
                    pattern[0, 0] = false;
                    pattern[0, 1] = false;
                    pattern[1, 0] = false;
                    pattern[1, 1] = true;
                    break;
                case "Quarter 3":
                    pattern[0, 0] = false;
                    pattern[0, 1] = false;
                    pattern[1, 0] = true;
                    pattern[1, 1] = false;
                    break;
                case "Quarter 4":
                    pattern[0, 0] = false;
                    pattern[0, 1] = true;
                    pattern[1, 0] = false;
                    pattern[1, 1] = false;
                    break;
                case "Half":
                    pattern[0, 0] = true;
                    pattern[0, 1] = false;
                    pattern[1, 0] = true;
                    pattern[1, 1] = false;
                    break;
                default:
                    pattern[0, 0] = false;
                    pattern[0, 1] = false;
                    pattern[1, 0] = false;
                    pattern[1, 1] = false;
                    break;
            }
        }
    }
}
