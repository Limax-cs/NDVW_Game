using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaboAnimatorConfiguration : MonoBehaviour
{
    Animator animator;
    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void OnAnimatorIK(int layerIndex)
    {
 
    }
}
