using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceInteraction : MonoBehaviour
{
    public List<GameObject> backpack = new List<GameObject>();
    public GameObject targetDirection;
    public StatusUI status;
    private int indexItem = 4;

    [SerializeField]
    private LayerMask pickableLayerMask;

    [SerializeField]
    [Min(1)]
    public float range = 7.0f;

    private RaycastHit hit;

    [SerializeField]
    private Transform pickUpParent;

    // Start is called before the first frame update
    void Start()
    {
        for (int i=0; i<9; i++)
        {
            backpack.Add(null);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Change Item
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            MakeNotVisible(indexItem);
            indexItem++;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            MakeNotVisible(indexItem);
            indexItem--;
        }

        if (indexItem < 0)
            indexItem = backpack.Count-1;
        else if (indexItem >= backpack.Count)
            indexItem = 0;

        // Make the item visible
        MakeVisible(indexItem);


        if (backpack[indexItem] == null)
        {
            // Detect Object
            Vector3 heading = targetDirection.transform.position - transform.position;
            Debug.DrawRay(transform.position, heading/heading.magnitude * range, Color.red);
            if (hit.collider != null)
            {
                hit.collider.GetComponent<Highlight>()?.ToggleHighLight(false);

                // Get item
                if(Input.GetButton("Fire1"))
                {
                    Interact();
                }
            }

            if(Physics.Raycast(
                transform.position, 
                heading/heading.magnitude, 
                out hit, 
                range,
                pickableLayerMask))
            {
                hit.collider.GetComponent<Highlight>()?.ToggleHighLight(true);
                //Debug.Log(hit.collider);
            }

        }
        else
        {
            // Throw Item
            if(Input.GetButton("Fire2"))
            {
                Drop();
            }
        }

        // Update UI
        status.setElements(backpack, indexItem);
    }

    // Object Interaction
    private void Use()
    {

    }

    private void Drop()
    {
        if (backpack[indexItem] is not null)
        {
            backpack[indexItem].transform.SetParent(null);
            backpack[indexItem] = null;
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }
        
    }

    private void Interact()
    {
        Debug.Log(hit.collider.name);
        Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
        if (hit.collider.GetComponent<EdibleItem>() || hit.collider.GetComponent<WeaponItem>())
        {
            Debug.Log("Edible!!!");
            hit.collider.GetComponent<Highlight>()?.ToggleHighLight(false);
            backpack[indexItem] = hit.collider.gameObject;
            backpack[indexItem].transform.position = Vector3.zero;
            backpack[indexItem].transform.rotation = Quaternion.identity;
            backpack[indexItem].transform.SetParent(pickUpParent.transform, false);
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
        if (hit.collider.GetComponent<ObjectItem>())
        {
            Debug.Log("Object");
            hit.collider.GetComponent<Highlight>()?.ToggleHighLight(false);
            backpack[indexItem] = hit.collider.gameObject;
            backpack[indexItem].transform.SetParent(pickUpParent.transform, true);
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
        
    }

    private void MakeVisible(int item)
    {
        if (backpack[item] != null)
        {
            MeshRenderer mr = backpack[item].GetComponent<MeshRenderer>();
            mr.enabled = true;
            Collider coll = backpack[item].GetComponent<Collider>();
            coll.enabled = true;
            backpack[item].GetComponent<Collider>().GetComponent<Highlight>()?.ToggleHighLight(false);
            UnityEngine.AI.NavMeshObstacle nm = backpack[item].GetComponent<UnityEngine.AI.NavMeshObstacle>();
            nm.enabled = true;
        }
    }

    private void MakeNotVisible(int item)
    {
        if (backpack[item] != null)
        {
            MeshRenderer mr = backpack[item].GetComponent<MeshRenderer>();
            mr.enabled = false;
            Collider coll = backpack[item].GetComponent<Collider>();
            coll.enabled = false;
            UnityEngine.AI.NavMeshObstacle nm = backpack[item].GetComponent<UnityEngine.AI.NavMeshObstacle>();
            nm.enabled = false;
            
        }
    }
}
