using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonCollider : MonoBehaviour
{
    public DragonHealth dragonHealth;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Dragon Hit");
        Debug.Log("Collider Tag: " + collision.gameObject.tag);
        //Debug.Log("Dragon Hit");
        if (collision.gameObject.tag == "damage")
        {
            WeaponItem weaponItem = collision.collider.GetComponent<WeaponItem>();
            dragonHealth.TakeDamage((int)(weaponItem.weaponDescrib.attack/10));
            Debug.Log("Dragon Hit");
            //Debug.Log("Mole Hit");
        }
    }
}
