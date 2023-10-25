using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SlugBehaviour : MonoBehaviour
{
    //Controls
    public float horizontalMove;
    public float verticalMove;
    public float mouseX;
    public float mouseY;
    public CharacterController player;
    public GameObject CameraTarget;
    public GameObject slugModel;
    public Animator animator;
    private Vector3 playerInput;

    //Moviment
    private Vector3 movePlayer;
    private Vector3 playerInputGrounded;
    public float gravity = 9.81f;
    private float fallSpeed = 0;
    public float InAirFriction = 0.995f;
    public float InertiaContribution = 0.9f;

    //public bool isOnSlope = false;
    public Vector3 hitnormal;
    public int terrainHitTimer;
    //public float slideVelocity = 7f;
    //public float slopeForceDown = 5f;

    //Camera
    public GameObject mainCamera;
    private Vector3 camForward;
    private Vector3 camRight;
    public float rotationConstant = 10;

    //OtherProperties
    public bool isHit = false;
    public bool isDefeated = false;
    public Vector3 normalDirection;
    public Vector3 hitDirection;
    private float hitTime;
    public float hitImpulse;
    public GameObject footReference;
    public float angle1;
    public float angle2;

    private float hitscenarioTime;

    //Character properties
    public int playerID;
    public char characterType;
    public float playerSpeed = 0.1f;
    public float JumpForce = 10;
    public float playerAttack;
    public float playerDefence;

    //Others
    //public SlugStatusUI SlugStatus;
    //public Rig RigAimDirection;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<CharacterController>();
        //RigAimDirection = GameObject.Find("Rig 1").GetComponent<Rig>();
    }

    void Update()
    {
        //Detecció dels controls
        //RigAimDirection.weight = 1;
        horizontalMove = Input.GetAxis("Horizontal");
        verticalMove = Input.GetAxis("Vertical");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
    }

    void FixedUpdate()
    {
        if (!isDefeated)
        {

            //Normalitzar el moviment perquè es mogui a la mateixa velocitat en totes les direccions

            if (!isHit)
            {
                if (player.isGrounded)
                {
                    playerInput = new Vector3(horizontalMove, 0, verticalMove);
                    playerInput = Vector3.ClampMagnitude(playerInput, 1);
                    playerInputGrounded = playerInput;
                }
                else
                {
                    playerInput = new Vector3(playerInputGrounded.x + (1 - InertiaContribution) * horizontalMove, 0, playerInputGrounded.z + (1 - InertiaContribution) * verticalMove);
                    playerInput = Vector3.ClampMagnitude(playerInput, 1) * InAirFriction;
                }
            }


            //Moviment del jugador segons la càmera
            CamDirection();
            movePlayer = playerInput.x * camRight + playerInput.z * camForward;
            Vector3 forwardSlug = player.transform.position + (playerInput.z + playerInput.x * playerInput.x) * camForward;
            //Debug.Log("Cam Forward: " + camForward + " |  player Rotation: " + player.transform.forward + " | normal : " + normalDirection);
            //Vector3 axisSlug = forwardSlug - player.transform.location;
            //axisSlug = axisSlug * Vector3.AngleAxis(90 - Vector3.Angle(normalDirection, axisSlug), Vector3.Cross(normalDirection, axisSlug));
            //forwardSlug.rotation = forwardSlug.rotation * Quaternion.AngleAxis(Vector3.Angle(normalDirection, player.transform.up), Vector3.Cross(normalDirection, player.transform.up));
            player.transform.LookAt(forwardSlug);
            //Debug.Log(" |  player Rotation: " + player.transform.forward);
            //player.transform.rotation = player.transform.rotation * Quaternion.AngleAxis(Vector3.Angle(normalDirection, player.transform.up), Vector3.Cross(normalDirection, player.transform.up));
            /*
            if (terrainHitTimer > 0)
            {
                player.transform.LookAt(player.transform.position + (playerInput.z + playerInput.x * playerInput.x) * camForward);
                player.transform.rotation = player.transform.rotation * Quaternion.AngleAxis(Vector3.Angle(normalDirection, player.transform.up), Vector3.Cross(normalDirection, player.transform.up));
            }
            else
            {
                player.transform.LookAt(player.transform.position + (playerInput.z + playerInput.x * playerInput.x) * camForward);
            }*/
            

            

            //Velocitat del jugador
            movePlayer = movePlayer * playerSpeed;
        }
        else
        {
            //RigAimDirection.weight = 0;
        }

        //Moviment de caiguda
        SetGravity();

        //Habilitats del llimac
        if (!isDefeated)
            SlugSkills();

        //Moviment
        HitImpulse();
        //FootAngle();
        player.Move(movePlayer * Time.deltaTime);

        //Animator Parameters
        animator.SetBool("isGrounded",player.isGrounded);
        float movement = horizontalMove * horizontalMove + verticalMove * verticalMove;
        animator.SetFloat("MovementVelocity", movement);
        animator.SetFloat("VerticalMovement", movePlayer.y);
        animator.SetBool("Jump", Input.GetButtonDown("Jump"));
        animator.SetBool("isHit", isHit);
        //isDefeated = SlugStatus.isSlugDefeated(playerID);
        //animator.SetBool("isDefeated", isDefeated);

        if (Input.GetKeyDown(KeyCode.T))
        {
            animator.SetBool("Taunt1", true);
        }
        else
        {
            animator.SetBool("Taunt1", false);
        }

        //Decrease hit timer
        if (terrainHitTimer > 0)
        {
            terrainHitTimer = terrainHitTimer -1;
        }
            
    }

    // Player ID
    public int GetPlayerID()
    {
        return playerID;
    }

    //Dependència del moviment segons la càmera
    public void CamDirection()
    {
        camForward = mainCamera.transform.forward;
        camRight = mainCamera.transform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward = camForward.normalized;
        camRight = camRight.normalized;
    }

    //Habilitats del jugador
    public void SlugSkills()
    {
        //Salts
        if(player.isGrounded && Input.GetButtonDown("Jump"))
        {
            fallSpeed = JumpForce;
            movePlayer.y = fallSpeed;
        }
    }

    //Actualització de la gravetat del jugador
    public void SetGravity()
    {

        if (player.isGrounded)
        {
            movePlayer.y = 0;
            fallSpeed = -gravity * Time.deltaTime;
            movePlayer.y = fallSpeed;
        }
        else
        {
            fallSpeed -= gravity * Time.deltaTime;
            movePlayer.y = fallSpeed;
        }

        
    }

    /*
    //Relliscar en rampes massa inclinades
    public void SlideDown()
    {
        isOnSlope = Vector3.Angle(Vector3.up, hitnormal) >= player.slopeLimit;

        if(isOnSlope)
        {
            movePlayer.x += (1f - hitnormal.y) * hitnormal.x * slideVelocity;
            movePlayer.z += (1f - hitnormal.y) * hitnormal.z * slideVelocity;
            movePlayer.y -= (1f - hitnormal.y) * slopeForceDown;
        }
    }*/


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Terrain Collision
        if (hit.collider.CompareTag("Terrain"))
        {
            normalDirection = hit.normal;
            terrainHitTimer = 5;
        }

    /*
        if(hit.collider.CompareTag("Salt"))
        {
            if (Time.time > (hitscenarioTime + 2))
            {
                hitscenarioTime = Time.time;
                isHit = true;
                //if (!isDefeated)
                //    SlugStatus.SetScenarioDamage(this.playerID, "Salt", this.playerDefence);
            }
        }*/
    }

    //HitReaction
    //On collision
    private void OnCollisionEnter(Collision collision)
    {
        
        
        /*
        if (collision.collider.CompareTag("SlimedSaltBullet") || collision.collider.CompareTag("SaltBullet") || collision.collider.CompareTag("DirectSaltBullet"))
        {
            //Debug.Log("collision");
            isHit = true;
            hitDirection = collision.contacts[0].normal;
            //Debug.Log(hitDirection);
            hitTime = Time.time;

            //Variables
            //BulletBehaviour bulletBehaviour = collision.collider.GetComponent<BulletBehaviour>();
            //if(!isDefeated)
            //    SlugStatus.SetAttackerDamage(this.playerID, bulletBehaviour.GetPlayerID(), bulletBehaviour.damage, bulletBehaviour.GetPlayerAttack(), playerDefence, bulletBehaviour.GetBounceCount(), bulletBehaviour.BulletType);
        }*/

    }

    public void HitImpulse()
    {
        if ((hitTime + 0.5 > Time.time) && (isHit))
        {
            movePlayer += hitDirection * hitImpulse;
            //RigAimDirection.weight = 0;
        }
        else if (isHit)
        {
            isHit = false;
            //if(!isDefeated)
            //    RigAimDirection.weight = 1;
        }
    }

    /*
    //Posició del peu
    //public void FootAngle()
    {
        if(player.isGrounded)
        {
            footReference.transform.localRotation = Quaternion.Euler(270 + (90 - Vector3.Angle(hitnormal, player.transform.forward)), 0,0);
            angle1 = 270 + (90 - Vector3.Angle(hitnormal, player.transform.forward));
            angle2 = (90 - Vector3.Angle(hitnormal, player.transform.right));
        }
    }
    */
}
