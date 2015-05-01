using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSPModelRocketry
{
    public class VariableTexture : PartModule
    {
        [Persistent]
        Color color1 = new Color(1, 1, 1);
        [Persistent]
        Color color2 = new Color(0, 0, 0);

        [KSPField(guiActiveEditor = true, guiName = "Red", guiFormat = "F2")]
        [UI_FloatRange(minValue = 0, maxValue = 1, stepIncrement = .05f)]
        public float red = 1f;

        [KSPField(guiActiveEditor = true, guiName = "Green", guiFormat = "F2")]
        [UI_FloatRange(minValue = 0, maxValue = 1, stepIncrement = .05f)]
        public float green = 1f;

        [KSPField(guiActiveEditor = true, guiName = "Blue", guiFormat = "F2")]
        [UI_FloatRange(minValue = 0, maxValue = 1, stepIncrement = .05f)]
        public float blue = 1f;

        [KSPField(guiActiveEditor = true, guiName = "Edit Color:")]
        [UI_Toggle(enabledText = "Primary", disabledText = "Secondary")]
        public bool switchColor = true;
        private bool swtclr;

        [KSPField(guiActiveEditor = true, guiName = "Pattern", isPersistant = true),
        KSPAPIExtensions.UI_ChooseOption(options = new string[] { "Plain", "Quarter 1", "Quarter 2",
            "Quarter 3", "Quarter 4", "Checker", "Half 1", "Half 2"})]
        public string texturePattern = "Plain";
        private string texpat = "";

        [KSPField(guiActiveEditor = true, guiName = "Texture Repeat", guiFormat = "F2", isPersistant = true)]
        [UI_FloatRange(minValue = 1, maxValue = 4, stepIncrement = 1)]
        public float textureRepeat = 2;
        public float texRep;
        private bool[,] pattern = new bool[2, 2];

        [KSPField]
        public string textureMeshName = "Variable";
        private Renderer texRend;
        private Material mat;

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

        public override void OnLoad(ConfigNode node)
        {
            if (node.GetNode("color1") != null)
            {
                ConfigNode c1 = node.GetNode("color1");
                color1 = new Color(Convert.ToSingle(c1.GetValue("red")),
                    Convert.ToSingle(c1.GetValue("green")),
                    Convert.ToSingle(c1.GetValue("blue")));
                red = color1.r;
                green = color1.g;
                blue = color1.b;
                switchColor = true;
            }
            if (node.GetNode("color2") != null)
            {
                ConfigNode c2 = node.GetNode("color2");
                color2 = new Color(Convert.ToSingle(c2.GetValue("red")),
                    Convert.ToSingle(c2.GetValue("green")),
                    Convert.ToSingle(c2.GetValue("blue")));
            }
            base.OnLoad(node);
            setPattern();
            setColor();
        }

        public override void OnSave(ConfigNode node)
        {
            ConfigNode c1 = new ConfigNode("color1");
            c1.AddValue("red", color1.r);
            c1.AddValue("green", color1.g);
            c1.AddValue("blue", color1.b);
            node.AddNode(c1);
            ConfigNode c2 = new ConfigNode("color2");
            c2.AddValue("red", color2.r);
            c2.AddValue("green", color2.g);
            c2.AddValue("blue", color2.b);
			node.AddNode(c2);
			node.RemoveNode ("texturePattern_UIFlight");
			node.RemoveNode ("textureRepeat_UIFlight");
			node.RemoveNode ("EVENTS");
			node.RemoveNode ("ACTIONS");
            base.OnSave(node);
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        public void Update()
        {
            if (switchColor != swtclr && HighLogic.LoadedSceneIsEditor)
            {
                if (switchColor)
                {
                    red = color1.r;
                    green = color1.g;
                    blue = color1.b;
                }
                else
                {
                    red = color2.r;
                    green = color2.g;
                    blue = color2.b;
                }
                swtclr = switchColor;
            }
            if (HighLogic.LoadedSceneIsEditor && ((
                switchColor ? red != color1.r || green != color1.g || blue != color1.b :
                red != color2.r || green != color2.g || blue != color2.b) ||
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
                    color1.r = red;
                    color1.g = green;
                    color1.b = blue;
                }
                else
                {
                    color2.r = red;
                    color2.g = green;
                    color2.b = blue;
                }
			Texture2D tex = new Texture2D(2 * (int)textureRepeat, 2, TextureFormat.RGB24, false);
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
                            colors[index] = color2;
                        }
                        else
                        {
                            colors[index] = color1;
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
                case "Half 1":
                    pattern[0, 0] = true;
                    pattern[0, 1] = false;
                    pattern[1, 0] = true;
                    pattern[1, 1] = false;
				break;
				case "Half 2":
					pattern[0, 0] = true;
					pattern[0, 1] = true;
					pattern[1, 0] = false;
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
