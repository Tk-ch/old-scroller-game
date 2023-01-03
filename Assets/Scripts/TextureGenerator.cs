using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
public class TextureGenerator : MonoBehaviour
{
    [SerializeField]
    RenderTexture renderTexture; 
    [SerializeField]
    Material material;

    /// <summary>
    /// Generates a Render texture from a shader; for now it is needed to cache the procedural noise into memory without using image textures
    /// </summary>
    void OnEnable()
    {
        if (Screen.width <= 0) return;
        renderTexture = new RenderTexture(Screen.width * 2, Screen.height * 2, 16);
        renderTexture.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8_UNorm;
        renderTexture.filterMode = FilterMode.Point;
        Graphics.Blit(null, renderTexture, material);
        GetComponent<Renderer>().sharedMaterial.mainTexture = renderTexture; //TODO: Use MaterialPropertyBlock instead of sharedMaterial; works fine for now though
    }

}
