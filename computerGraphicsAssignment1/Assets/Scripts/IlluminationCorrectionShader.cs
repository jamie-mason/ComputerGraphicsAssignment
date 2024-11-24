using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IlluminationCorrectionShader : MonoBehaviour
{
    public Shader illuminationShader = null;  // Illumination shader
    public Material illuminationMaterial;     // Material using the shader

    // Called when the camera finishes rendering the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // If the illumination material and shader are assigned, apply the shader
        if (illuminationMaterial != null)
        {
            Graphics.Blit(source, destination, illuminationMaterial);
        }
        else
        {
            // If no material, simply copy the source to the destination (no modification)
            Graphics.Blit(source, destination);
        }
    }
}
