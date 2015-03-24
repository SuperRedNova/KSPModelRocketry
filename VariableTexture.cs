using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSPModelRocketry
{
    public class VariableTexture : PartModule
    {

        public static float scale = 21;

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
        public Mesh textureMesh;
        public Renderer texRend;
        public Material mat;

        public float updateDelay = 0;



        /// <summary>
        /// Raises the start event.
        /// </summary>
        /// <param name="state">State.</param>
        public override void OnStart(StartState state)
        {
            if (textureMesh == null)
            {
                textureMesh = part.FindModelComponent<MeshFilter>(textureMeshName).mesh;
            }
            if (texRend == null)
            {
                texRend = part.FindModelComponent<Renderer>(textureMeshName);
                mat = new Material(texRend.material.shader);
            }
            changeColor(true);
            setUV();
        }

        /// <summary>
        /// Standardizes the uv coords of the mesh.
        /// </summary>
        private void setUV()
        {
            Vector2[] uvs = new Vector2[textureMesh.vertices.Length];
            for (int i = 0; i < uvs.Length; i++)
            {
                int j = i % 4;
                switch (j)
                {
                    case 0:
                        uvs[i] = Vector2.zero;
                        break;
                    case 1:
                        uvs[i] = Vector2.up;
                        break;
                    case 2:
                        uvs[i] = Vector2.right;
                        break;
                    case 3:
                        uvs[i] = Vector2.one;
                        break;
                }
            }
            textureMesh.uv = uvs;
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        public void Update()
        {
            if (HighLogic.LoadedSceneIsEditor)
            {
                updateDelay -= Time.deltaTime;
                if (updateDelay <= 0)
                {
                    changeColor();
                    updateDelay = 0.02f;
                }
            }
        }

        /// <summary>
        /// Changes the color of a mesh of a part implementing this PartModlue based on it's red, green, and blue values.
        /// </summary>
        void changeColor(Boolean fromOnStart = false)
        {
            if (red != r | green != g | blue != b | fromOnStart)
            {
                r = red;
                g = green;
                b = blue;
                Color color = new Color(r, g, b);
                Texture2D tex = new Texture2D(10, 10);
                Color[] colors = new Color[tex.height * tex.width];
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = color;
                }
                tex.SetPixels(colors);
                tex.Apply();
                mat.mainTexture = tex;
                texRend.material = mat;
                if (fromOnStart)
                {
                    print("[VariableTexture]:Changing Color!" +color);
                }
            }
        }
    }
}
