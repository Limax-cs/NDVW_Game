using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponDescription
{
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
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (time > 0)
            time -= Time.deltaTime;
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

            time = weaponDescrib.cooldown;
        }
    }

    public void BladeAttack()
    {

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
            GameObject newBullet = Instantiate(bullet, shootingOut.position, Quaternion.Euler(0.0f, 0.0f, 90.0f)*shootingOut.rotation);

            // Set forward direction
            newBullet.transform.forward = shootingOut.forward;

            // Apply force in the forward direction
            Rigidbody bulletRb = newBullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.velocity = shootingOut.forward * weaponDescrib.speed;
            }
        
        }
    }
}
