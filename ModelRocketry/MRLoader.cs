using UnityEngine;
using KSPAssets.Loaders;
using KSPAssets;

namespace ModelRocketry
{
    [KSPAddon(KSPAddon.Startup.MainMenu,true)]
    class LoadShader : MonoBehaviour
    {
        public static Shader MRDiffuse;
        public static Shader MRDiffuseAlpha;
        public static Texture2D Checker;

        void Start()
        {
            AssetDefinition mrdiffusedef = AssetLoader.GetAssetDefinitionWithName("ModelRocketry/AssetBundles/mrdiffuse", "ModelRocketry/Diffuse");
            AssetDefinition mrdiffusealphadef = AssetLoader.GetAssetDefinitionWithName("ModelRocketry/AssetBundles/mrdiffusealpha", "ModelRocketry/DiffuseAlpha");
            AssetDefinition checkerdef = AssetLoader.GetAssetDefinitionWithName("ModelRocketry/AssetBundles/mrtexture", "checker");
            AssetLoader.LoadAssets(AssetLoaded, mrdiffusedef);
            AssetLoader.LoadAssets(AssetLoaded, mrdiffusealphadef);
            AssetLoader.LoadAssets(AssetLoaded, checkerdef);
        }

        void AssetLoaded(AssetLoader.Loader loader)
        {
            for (int i = 0; i < loader.definitions.Length; i++)
            {
                Object o = loader.objects[i];
                switch (o.name) {
                    case "ModelRocketry/Diffuse":
                        MRDiffuse = o as Shader;
                        Debug.Log("[ModelRocketry] loaded \"ModelRocketry/Diffuse\"");
                        break;
                    case "ModelRocketry/DiffuseAlpha":
                        MRDiffuseAlpha = o as Shader;
                        Debug.Log("[ModelRocketry] loaded \"ModelRocketry/DiffuseAlpha\"");
                        break;
                    case "checker":
                        Checker = o as Texture2D;
                        Debug.Log("[ModelRocketry] loaded \"checker\"");
                        break;
                    default:
                        continue;
                }
            }
        }
    }
}
