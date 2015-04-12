using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSPModelRocketry
{
    public class VariableTexture : PartModule
    {
        [KSPField(guiActiveEditor = true, guiName = "Red", guiFormat = "F2", isPersistant = true)]
        [UI_FloatRange(minValue = 0, maxValue = 1, stepIncrement = .05f)]
        public float red = 1f;
        private float r;

        [KSPField(guiActiveEditor = true, guiName = "Green", guiFormat = "F2", isPersistant = true)]
        [UI_FloatRange(minValue = 0, maxValue = 1, stepIncrement = .05f)]
        public float green = 1f;
        private float g;

        [KSPField(guiActiveEditor = true, guiName = "Blue", guiFormat = "F2", isPersistant = true)]
        [UI_FloatRange(minValue = 0, maxValue = 1, stepIncrement = .05f)]
        public float blue = 1f;
        private float b;

        [KSPField]
        public string textureMeshName = "Variable";
        private Renderer texRend;
        private Material mat;

        [KSPField(guiActiveEditor = true, guiName = "Pattern", isPersistant = true),
        KSPAPIExtensions.UI_ChooseOption(options = new string[] { "Plain", "Quarter", "Checker", "Half" })]
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
            if ((HighLogic.LoadedSceneIsEditor || HighLogic.LoadedSceneIsFlight) &&
                (red != r || green != g || blue != b || !texturePattern.Equals(texpat) || textureRepeat != texRep))
            {
                if (red != r) print("red");
                if (green != g) print("green");
                if (blue != b) print("blue");
                if (!texturePattern.Equals(texpat)) print("texturePattern");
                if (textureRepeat != texRep) print("textureRepeat");
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
            r = red;
            g = green;
            b = blue;
            Color color = new Color(r, g, b);
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
                            colors[index] = Color.black;
                        }
                        else
                        {
                            colors[index] = color;
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
                case "Quarter":
                    pattern[0, 0] = true;
                    pattern[0, 1] = false;
                    pattern[1, 0] = false;
                    pattern[1, 1] = false;
                    break;
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
