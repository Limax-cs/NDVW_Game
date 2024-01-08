using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    [SerializeField]
    private List<Renderer> renderers;

    [SerializeField]
    private Color color = Color.white;
    public Color emissionColor;
    public Texture emissionTexture;
    public bool emissionEnable;

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
            for (int i= 0; i < materials.Count; i++)
            {
                materials[i].EnableKeyword("_EMISSION");
                materials[i].SetColor("_EmissionColor", color);
                materials[i].SetTexture("_EmissionMap", null);
            }
        }
        else
        {
            for (int i= 0; i < materials.Count; i++)
            {
                if  (!emissionEnable)
                {
                    materials[i].DisableKeyword("_EMISSION");
                }

                materials[i].SetColor("_EmissionColor", emissionColor);
                materials[i].SetTexture("_EmissionMap", emissionTexture);
                
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
