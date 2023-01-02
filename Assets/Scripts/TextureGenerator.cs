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

    void OnEnable()
    {
        if (Screen.width <= 0) return;
        renderTexture = new RenderTexture(Screen.width * 2, Screen.height * 2, 16);
        renderTexture.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8_UNorm;
        renderTexture.filterMode = FilterMode.Point;
        Graphics.Blit(null, renderTexture, material);
        GetComponent<Renderer>().sharedMaterial.mainTexture = renderTexture;
    }

}
