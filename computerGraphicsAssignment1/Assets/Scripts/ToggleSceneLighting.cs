using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime;


public class ToggleSceneLighting : MonoBehaviour
{
    [SerializeField] private int[] sceneLightingToggles;
    [SerializeField] private List<int[]> sceneLightingTogglesCombinations = new List<int[]>(); // List to store all combinations

    bool hasChanged;
    int maxLength;
    int currentLight;


    [SerializeField] private List<Material> MaterialsToToggle = new List<Material>();
    private string[] ToggleProperties = new string[4];

    [SerializeField] private TextMeshProUGUI illumnationDispay;




    private void Awake()
    {
        ToggleProperties[0] = "_ToggleAmbient";
        ToggleProperties[1] = "_ToggleDiffuse";
        ToggleProperties[2] = "_ToggleSpecular";
        ToggleProperties[3] = "_ToggleDiffuseWrap";

        int numOfToggles = ToggleProperties.Length;
        int numOfCombinations = (int)Math.Pow(2, numOfToggles);
        sceneLightingToggles = new int[numOfCombinations];

        GenerateCombinations(numOfToggles);
    }

    private void GenerateCombinations(int numOfToggles)
    {
        //a list containing all of the property name indexs that should be equal to one

        for (int i = 0; i < (1 << numOfToggles); i++)
        {
            int[] combination = new int[numOfToggles];
            for (int j = 0; j < numOfToggles; j++)
            {
                combination[j] = (i & (1 << j)) != 0 ? 1 : 0;
            }
            sceneLightingTogglesCombinations.Add(combination);
        }


    }

    private void Start()
    {

        if(illumnationDispay == null){
            illumnationDispay = GameObject.Find("IlluminationTypeUIDisplay").GetComponent<TextMeshProUGUI>();
        }
        Renderer[] allRenderers = FindObjectsOfType<Renderer>();
        foreach (Renderer renderer in allRenderers)
        {
            foreach (Material mat in renderer.sharedMaterials)
            {
                foreach (string ToggleProperty in ToggleProperties)
                {
                    if (mat.HasProperty(ToggleProperty))
                    {
                        MaterialsToToggle.Add(mat);
                        break;
                    }
                }


            }
        }

        foreach (Material mat in MaterialsToToggle)
        {
            foreach (string ToggleProperty in ToggleProperties)
            {
                if (mat.HasProperty(ToggleProperty))
                {
                    mat.SetFloat(ToggleProperty, 1f);
                }
            }
        }

        for (int i = 0; i < sceneLightingToggles.Length; i++)
        {
            sceneLightingToggles[i] = i + 1;

        }

        
        hasChanged = false;
        maxLength = sceneLightingToggles.Length - 1;
        currentLight = maxLength;
        illumnationDispay.text = toggleGUI(currentLight);
        //set all toggle values to 0 or 1;
    }





    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.J))
        {
            if (currentLight > 0)
            {
                currentLight--;
            }
            else
            {
                currentLight = maxLength;
            }
            hasChanged = true;

        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            if (currentLight < maxLength)
            {
                currentLight++;
            }
            else
            {
                currentLight = 0;
            }
            hasChanged = true;

        }
        else
        {
            hasChanged = false;
        }

        toggleLights(currentLight);
        if(hasChanged && illumnationDispay != null){
            illumnationDispay.text = toggleGUI(currentLight);
        }
    }

    string ExtractAfterToggle(string property)
    {
        if (property.Contains("Toggle"))
        {
            return property.Substring(property.IndexOf("Toggle") + "Toggle".Length);
        }
        return property;
    }

    string InsertSpaces(string input)
    {
        return Regex.Replace(input, "(\\B[A-Z])", " $1");
    }

    void toggleLights(int currentLight)
    {
        foreach (Material mat in MaterialsToToggle)
        {
            for (int i = 0; i < ToggleProperties.Length; i++)
            {
                if (mat.HasProperty(ToggleProperties[i]))
                {

                    mat.SetFloat(ToggleProperties[i], sceneLightingTogglesCombinations[currentLight][i]);
                }
            }

        }
    }
    string toggleGUI(int currentLight)
    {
        string guiText;
        int onesCount = 0;
        List<int> activeBits = new List<int>();

        // Count 1s and store the indices of active bits
        for (int i = 0; i < sceneLightingTogglesCombinations[currentLight].Length; i++)
        {
            if (sceneLightingTogglesCombinations[currentLight][i] == 1)
            {
                onesCount++;
                activeBits.Add(i); // Add the index of active bit
            }
        }

        // Handle GUI text based on onesCount
        if (onesCount == 1)
        {
            guiText = InsertSpaces(ExtractAfterToggle(ToggleProperties[activeBits[0]])) + " illumination only";
        }
        else if (onesCount > 1)
        {
            guiText = string.Join(" + ", activeBits.Select(index => InsertSpaces(ExtractAfterToggle(ToggleProperties[index]))));
        }
        else
        {
            guiText = "No Lighting";
        }

        Debug.Log(guiText);
        return guiText;

    }
    void OnDisable()
    {
        foreach (Material mat in MaterialsToToggle)
        {
            foreach (string ToggleProperty in ToggleProperties)
            {
                if (mat.HasProperty(ToggleProperty))
                {
                    mat.SetFloat(ToggleProperty, 1f);
                }
            }
        }
    }
}
