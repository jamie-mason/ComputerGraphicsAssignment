using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System.Linq;

public class ToggleSceneLighting : MonoBehaviour
{
    [SerializeField] private int[] sceneLightingToggles; // Array to hold lighting toggle values
    [SerializeField] private List<int[]> sceneLightingTogglesCombinations = new List<int[]>(); // List of all possible toggle combinations

    bool hasChanged; // Flag to track changes in lighting
    int maxLength; // Maximum length of the lighting combinations
    int currentLight; // Currently selected lighting combination

    public Shader customLightingShader; // Shader to be applied to materials
    private Renderer[] renderers; // Array to hold all renderers in the scene
    private List<Material> materials = new List<Material>(); // List to store modified materials
    private string[] ToggleProperties = new string[4]; // Shader properties to be toggled

    [SerializeField] private TextMeshProUGUI illumnationDispay; // UI text to display current illumination type

    private void Awake()
    {
        // Initialize toggle property names
        ToggleProperties[0] = "_ToggleAmbient";
        ToggleProperties[1] = "_ToggleDiffuse";
        ToggleProperties[2] = "_ToggleSpecular";
        ToggleProperties[3] = "_ToggleDiffuseWrap";

        int numOfToggles = ToggleProperties.Length; // Number of toggle properties
        int numOfCombinations = (int)Math.Pow(2, numOfToggles); // Calculate number of combinations
        sceneLightingToggles = new int[numOfCombinations]; // Initialize the array

        GenerateCombinations(numOfToggles); // Generate all toggle combinations
    }

    private void GenerateCombinations(int numOfToggles)
    {
        // Generate all possible combinations of toggle states (0 or 1)
        for (int i = 0; i < (1 << numOfToggles); i++)
        {
            int[] combination = new int[numOfToggles];
            for (int j = 0; j < numOfToggles; j++)
            {
                combination[j] = (i & (1 << j)) != 0 ? 1 : 0; // Set toggle states
            }
            sceneLightingTogglesCombinations.Add(combination); // Add combination to the list
        }
    }

    private void Start()
    {
        getExistingMaterials(); // Retrieve existing materials in the scene
        if (illumnationDispay == null)
        {
            illumnationDispay = GameObject.Find("IlluminationTypeUIDisplay").GetComponent<TextMeshProUGUI>(); // Find the UI element
        }

        // Initialize scene lighting toggle values
        for (int i = 0; i < sceneLightingToggles.Length; i++)
        {
            sceneLightingToggles[i] = i + 1;
        }

        hasChanged = false; // Initialize change flag
        maxLength = sceneLightingToggles.Length - 1; // Set maximum length
        currentLight = maxLength; // Start with the last lighting configuration
        illumnationDispay.text = toggleGUI(currentLight); // Update display with current lighting
        UpdateMaterials(maxLength); // Apply the initial lighting configuration to materials
    }

    void getExistingMaterials()
    {
        renderers = FindObjectsOfType<Renderer>(); // Find all renderers in the scene

        foreach (Renderer renderer in renderers)
        {
            if (renderer != null && renderer.material != null)
            {
                Material existingMaterial = renderer.material; // Get the existing material

                existingMaterial.shader = customLightingShader; // Apply custom shader
                existingMaterial.mainTexture = renderer.material.mainTexture; // Preserve the original texture

                // Set shader properties safely
                if (existingMaterial.HasProperty("_Color"))
                {
                    existingMaterial.SetColor("_Color", existingMaterial.GetColor("_Color"));
                }
                else
                {
                    existingMaterial.SetColor("_Color", Color.white);
                }

                if (existingMaterial.HasProperty("_SpecColor"))
                {
                    existingMaterial.SetColor("_SpecColor", existingMaterial.GetColor("_SpecColor"));
                }
                else
                {
                    existingMaterial.SetColor("_SpecColor", Color.white);
                }

                if (existingMaterial.HasProperty("_Shininess"))
                {
                    existingMaterial.SetFloat("_Shininess", existingMaterial.GetFloat("_Shininess"));
                }
                else
                {
                    existingMaterial.SetFloat("_Shininess", 10f);
                }

                materials.Add(existingMaterial); // Add the modified material to the list
            }
        }
    }

    private void Update()
    {
        // Handle input for toggling lights
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (currentLight > 0)
            {
                currentLight--; // Move to the previous lighting configuration
            }
            else
            {
                currentLight = maxLength; // Wrap around to the last configuration
            }
            hasChanged = true;
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            if (currentLight < maxLength)
            {
                currentLight++; // Move to the next lighting configuration
            }
            else
            {
                currentLight = 0; // Wrap around to the first configuration
            }
            hasChanged = true;
        }
        else
        {
            hasChanged = false; // No changes made
        }

        UpdateMaterials(currentLight); // Update material properties based on current lighting configuration
        if (hasChanged && illumnationDispay != null)
        {
            illumnationDispay.text = toggleGUI(currentLight); // Update UI text if there were changes
        }
    }

    string ExtractAfterToggle(string property)
    {
        if (property.Contains("Toggle"))
        {
            return property.Substring(property.IndexOf("Toggle") + "Toggle".Length); // Extract the property name after "Toggle"
        }
        return property;
    }

    string InsertSpaces(string input)
    {
        return Regex.Replace(input, "(\\B[A-Z])", " $1"); // Add spaces before capital letters in the string
    }

    void UpdateMaterials(int currentLight)
    {
        foreach (Material material in materials)
        {
            // Update material properties based on the current toggle state
            for (int i = 0; i < ToggleProperties.Length; i++)
            {
                if (material.HasProperty(ToggleProperties[i]))
                {
                    material.SetFloat(ToggleProperties[i], sceneLightingTogglesCombinations[currentLight][i]); // Set the toggle property
                }
            }
        }
    }

    string toggleGUI(int currentLight)
    {
        string guiText;
        int onesCount = 0; // Count of active toggle states
        List<int> activeBits = new List<int>(); // List to store indices of active bits

        // Count active bits and store their indices
        for (int i = 0; i < sceneLightingTogglesCombinations[currentLight].Length; i++)
        {
            if (sceneLightingTogglesCombinations[currentLight][i] == 1)
            {
                onesCount++;
                activeBits.Add(i);
            }
        }

        // Generate GUI text based on active toggles
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
            guiText = "No Lighting"; // No active lighting toggles
        }

        Debug.Log(guiText); // Log the current GUI text
        return guiText; // Return the GUI text
    }

    void OnDisable()
    {
        UpdateMaterials(maxLength); // Reset materials when disabled
    }
}
