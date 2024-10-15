using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerRimLighting : MonoBehaviour
{
    [SerializeField] private Shader rimLighting;
    [SerializeField] private Material rimMaterial;
    private Material materialOnRenderer; // List to store modified materials

    Renderer rend;

    // Update is called once per frame
    void Start(){
        rend = GetComponent<Renderer>();
        if(rend!=null){
            if(rend.material != null){
                materialOnRenderer = rend.material;
            }
        }
        UpdateMaterial();

    }
    void Update()
    {
        
    }
    void UpdateMaterial(){
        Material newMat = materialOnRenderer;
        newMat.shader = rimLighting;
        materialOnRenderer.shader = rimLighting; // Apply custom shader
        materialOnRenderer.mainTexture = rend.material.mainTexture; // Preserve the original texture
        
        if(rend.material.HasProperty("_Color")){
            materialOnRenderer.SetColor("_BaseColour", rend.material.GetColor("_Color"));

        }
        if (materialOnRenderer.HasProperty("_SpecColor"))
        {
            materialOnRenderer.SetColor("_SpecColor", materialOnRenderer.GetColor("_SpecColor"));
        }
        else
        {
            materialOnRenderer.SetColor("_SpecColor", Color.white);
        }

        if (materialOnRenderer.HasProperty("_Shininess"))
        {
            materialOnRenderer.SetFloat("_Shininess", materialOnRenderer.GetFloat("_Shininess"));
        }
        else
        {
            materialOnRenderer.SetFloat("_Shininess", 10f);
        }
    }
}
