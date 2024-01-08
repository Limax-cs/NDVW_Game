using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponDescription
{
    public int ID = -1;
    public string userType = "Mole";
    public int agentID = -1;
    public string type = "Blade";
    public float range = 2;
    public float attack = 1;
    public float impulse = 0;
    public float speed = 0;
    public float cooldown = 0;
}


public class WeaponItem : MonoBehaviour
{
    public Texture2D icon;
    public WeaponDescription weaponDescrib;
    public GameObject damageArea;
    public GameObject bullet;
    public Transform shootingOut; 

    private float time = 0;
    private float attackTime = 0;
    private Collider bladeCollider;
    
    // Start is called before the first frame update
    void Start()
    {
        if (weaponDescrib.type == "Blade")
        {
            if(damageArea != null)
            {
                bladeCollider = damageArea.GetComponent<Collider>();
                bladeCollider.enabled = false;
                WeaponItem weaponItem = bladeCollider.GetComponent<WeaponItem>();
                weaponItem.weaponDescrib = weaponDescrib;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (time > 0)
            time -= Time.deltaTime;
        if (attackTime > 0)
            attackTime -= Time.deltaTime;
        else if (weaponDescrib.type == "Blade")
        {
            this.transform.localScale = new Vector3(1f,1f,1f);
            if (bladeCollider != null)
                bladeCollider.enabled = false;
        }

        if (weaponDescrib.type == "Blade")
        {
            if(damageArea != null)
            {
                WeaponItem weaponItem = bladeCollider.GetComponent<WeaponItem>();
                weaponItem.weaponDescrib = weaponDescrib;
            }
        }
    }


    // Attack with the weapon
    public void Attack()
    {
        if (time <= 0)
        {
            if (weaponDescrib.type == "Blade")
                this.BladeAttack();
            else if (weaponDescrib.type == "Gun")
                this.GunAttack();
        }

    }

    public void BladeAttack()
    {
        //this.transform.Rotate(Vector3.right,weaponDescrib.speed * Time.deltaTime);
        //float targetRotation = 115f;
        //Debug.Log(this.transform.rotation.eulerAngles.x);
        //if (this.transform.rotation.eulerAngles.x > targetRotation)
        //{
        //    this.transform.rotation = Quaternion.identity;
        //    time = weaponDescrib.cooldown;
        //
        //}
        this.transform.localScale = new Vector3(2.0f,2.0f,2.0f);
        time = weaponDescrib.cooldown;
        attackTime = 0.05f;
        bladeCollider.enabled = true;
    }

    public void GunAttack()
    {
        /*
        if (bullet != null)
        {
            BulletBehaviour bulletBehaviour = bullet.GetComponent<BulletBehaviour>();
            if (bulletBehaviour != null)
                bulletBehaviour.speed = weaponDescrib.speed;
            Instantiate(bullet, shootingOut.position, shootingOut.rotation);
        }*/

        if (bullet != null)
        {
            // Instantiate the bullet
            GameObject newBullet = Instantiate(bullet, shootingOut.position, Quaternion.Euler(0.0f, 0.0f, 0.0f)*shootingOut.rotation);

            // Set forward direction
            newBullet.transform.forward = shootingOut.forward;

            // Apply force in the forward direction
            Rigidbody bulletRb = newBullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.velocity = shootingOut.forward * weaponDescrib.speed;
            }

            // Update bullet statistics
            WeaponItem weaponItem = newBullet.GetComponent<WeaponItem>();
            weaponItem.weaponDescrib = weaponDescrib;
        
        }

        time = weaponDescrib.cooldown;
    }
}
