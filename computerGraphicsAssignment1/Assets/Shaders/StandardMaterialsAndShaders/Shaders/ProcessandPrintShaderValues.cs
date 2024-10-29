using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessandPrintShaderValues : MonoBehaviour
{
    [SerializeField] private Shader texture;
    [SerializeField] private Material material;
    void Start()
    {
        if (material.HasProperty("_MainTex"))
        {

            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
