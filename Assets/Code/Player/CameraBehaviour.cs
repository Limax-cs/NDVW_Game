using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
public class CameraBehaviour : MonoBehaviour
{

    public Transform target;
    public Vector3 offset;
    [Range(0,1)]public float lerpValue;
    public float sensitivityHoritzontal;
    public float sensitivityVertical;
    public float verticalTargetOffset;

    //VerticalMove
    public float verticalCameraMove;
    public float MaxVerticalMove = 0.8f;
    public float MinVerticalMove = 0.3f;

    //Shooting reference
    public float range = 200.0f;
    public GameObject ShootingTarget;

    //Text on upper left corner
    public Text points;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("SlugPlayer").transform;
       
    }

    // Update is called once per frame
    private void Update()
    {
        // System.DateTime currentDateTime = System.DateTime.Now;
        // string formattedDateTime = currentDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");
        // points.text = "Current date and time: " + formattedDateTime;
    }
    void LateUpdate()
    {
        //VerticalCameraMove
        verticalCameraMove -= Input.GetAxis("Mouse Y") * sensitivityVertical;
        if (MaxVerticalMove < verticalCameraMove)
            verticalCameraMove = MaxVerticalMove;
        else if (MinVerticalMove > verticalCameraMove)
            verticalCameraMove = MinVerticalMove;
        

        //Moviment de la càmera
        transform.position = Vector3.Lerp(transform.position, target.position + offset, lerpValue);
        offset.y = 2;
        offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * sensitivityHoritzontal, Vector3.up) * offset + new Vector3(0, verticalCameraMove, 0);
        transform.LookAt(target.position + new Vector3(0,verticalTargetOffset,0));

        //Raycast
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward * range, Color.blue);
        if(Physics.Raycast(transform.position, transform.forward, out hit, range))
        {
            //Debug.Log(hit.transform.name);
            float angle = Vector3.Angle(hit.point - target.position, target.forward);
            if (angle < 90)
                ShootingTarget.transform.position = hit.point;
            else
                ShootingTarget.transform.position = target.transform.position + 20 * target.forward;

            
        }
    }
}