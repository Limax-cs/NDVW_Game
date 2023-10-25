using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    [SerializeField]
    private List<Renderer> renderers;

    [SerializeField]
    private Color color = Color.white;

    private List<Material> materials;

    private void Awake()
    {
        materials = new List<Material>();
        foreach (var Renderer in renderers)
        {
            materials.AddRange(new List<Material>(GetComponent<Renderer>().materials));
        }
    }

    public void ToggleHighLight(bool val)
    {
        if (val)
        {
            foreach (var material in materials)
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color);
            }
        }
        else
        {
            foreach (var material in materials)
            {
                material.DisableKeyword("_EMISSION");
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
