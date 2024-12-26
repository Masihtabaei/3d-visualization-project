using UnityEngine;
using UnityEditor;

public class FBXFixer : MonoBehaviour
{
    public GameObject fbxModel; // Drag your FBX model here
    public string textureFolderPath = "Assets/Textures"; // Path to your texture folder

    public void Start()
    {
        ApplyAllTextures();
    }

    public void ApplyAllTextures()
    {
        // Load all textures in the specified folder
        string[] texturePaths = AssetDatabase.FindAssets("t:Texture2D", new[] { textureFolderPath });

        if (texturePaths.Length == 0)
        {
            Debug.LogError("No textures found in the specified folder.");
            return;
        }

        // Map textures by name
        var textures = new System.Collections.Generic.Dictionary<string, Texture2D>();
        foreach (string guid in texturePaths)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (texture != null)
            {
                textures[texture.name] = texture;
            }
        }

        // Iterate through all the renderers in the FBX model
        Renderer[] renderers = fbxModel.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.sharedMaterials)
            {
                if (material != null && textures.TryGetValue(material.name, out Texture2D matchedTexture))
                {
                    material.mainTexture = matchedTexture;
                    Debug.Log($"Applied texture {matchedTexture.name} to material {material.name}.");
                }
                else
                {
                    Debug.LogWarning($"No matching texture found for material {material.name}.");
                }
            }
        }
    }
}
