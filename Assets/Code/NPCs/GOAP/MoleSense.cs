using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleSense : MonoBehaviour
{
    public GameObject moleObject;
    public GOAPAgent mole = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mole is null)
            mole = moleObject.GetComponent<GOAPAgent>();
    }

    // Detect objects
    public void OnTriggerEnter(Collider collider)
    {
        if (mole != null)
        {
            // Detect spaceship items
            if (collider.CompareTag("spaceship2_item"))
            {
                ObjectItem objectItem = collider.GetComponent<ObjectItem>();
                mole.beliefs.ModifyState("Detect Mole SSItem " + objectItem.ID, 1);
            }

            if (collider.CompareTag("spaceship1_item"))
            {
                ObjectItem objectItem = collider.GetComponent<ObjectItem>();
                mole.beliefs.ModifyState("Detect Babo SSItem " + objectItem.ID, 1);
            }
        }
    }
}
