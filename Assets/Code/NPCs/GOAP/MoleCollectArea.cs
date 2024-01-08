using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleCollectArea : MonoBehaviour
{
    public GOAPAgent mole;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Detect objects
    /*
    public void OnTriggerEnter(Collider collider)
    {
        // Detect spaceship items
        if (collider.CompareTag("spaceship2_item"))
        {
            ObjectItem objectItem = collider.GetComponent<ObjectItem>();
            mole.beliefs.ModifyState("Collectable Mole SSItem " + objectItem.ID, 1);
        }

        if (collider.CompareTag("spaceship1_item"))
        {
            ObjectItem objectItem = collider.GetComponent<ObjectItem>();
            mole.beliefs.ModifyState("Collectable Babo SSItem " + objectItem.ID, 1);
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        // Detect spaceship items
        if (collider.CompareTag("spaceship2_item"))
        {
            ObjectItem objectItem = collider.GetComponent<ObjectItem>();
            mole.beliefs.RemoveState("Collectable Mole SSItem " + objectItem.ID);
        }

        if (collider.CompareTag("spaceship1_item"))
        {
            ObjectItem objectItem = collider.GetComponent<ObjectItem>();
            mole.beliefs.RemoveState("Collectable Babo SSItem " + objectItem.ID);
        }
    }*/
}
