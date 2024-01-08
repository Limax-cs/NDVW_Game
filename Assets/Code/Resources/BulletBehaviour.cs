using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public float speed = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(this.transform.forward * speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }

}
