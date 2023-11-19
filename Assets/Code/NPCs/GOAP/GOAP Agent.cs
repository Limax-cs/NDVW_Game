using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAPAgent : GAgent
{
    public CharacterController GA;

    //Moviment
    //private Vector3 moveGA;
    //private Vector3 GAInputGrounded;
    //public float gravity = 9.81f;
    //private float fallSpeed = 0;
    //public float InAirFriction = 0.995f;
    //public float InertiaContribution = 0.9f;
    //public float CoyoteTime = 0.2f;
    //public float coyoteCounter = 0f;

    //OtherProperties
    public bool isHit = false;
    public bool isDefeated = false;

    // Start is called before the first frame update
    void Start()
    {
        GA = GetComponent<CharacterController>();

        base.Start();
        SubGoal s1 = new SubGoal("arrivedW", 1, true);
        goals.Add(s1, 1);

        //foreach(KeyValuePair<SubGoal, int> i in goals){Debug.Log(i);}
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        //SetGravity();
        //GA.Move(moveGA * Time.deltaTime);
    }

/*
    public void SetGravity()
    {

        if (GA.isGrounded)
        {
            moveGA.y = 0;
            fallSpeed = -gravity * Time.deltaTime;
            moveGA.y = fallSpeed;
        }
        else
        {
            fallSpeed -= gravity * Time.deltaTime;
            moveGA.y = fallSpeed;
        }

        
    }*/
}
