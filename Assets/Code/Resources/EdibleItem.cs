using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EdibleDescription
{
    public int ID = -1;
    public string userType = "Mole";
    public int agentID = -1;
    public float HPrecover = 0;
    public float defenceFactor = 1.0f;
    public float attackFactor = 1.0f;
    public float velocityFactor = 1.0f;
    public float effectTime = 10;
}

public class EdibleItem : MonoBehaviour
{
    public Texture2D icon;
    public EdibleDescription edibleDescrib;
    public GameObject target;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // COnsume method
    public void Consume()
    {
        Debug.Log("Potion consumed to " + target.name);
        Destroy(this.gameObject);
    }
}
