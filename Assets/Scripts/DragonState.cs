using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DragonState{

    public enum STATE {

        IDLE,
        PATROL,
        PURSUE,
        ATTACK,
        SLEEP,
        RUNAWAY,
        AGGRESSIVE
    };

    public enum EVENT {

        ENTER,
        UPDATE,
        EXIT
    };

    public STATE name;
    protected EVENT stage;
    protected GameObject npc;
    protected Animator anim;
    protected Transform player;
    protected DragonState nextState;
    protected Vector3 destination;

    public float flyingSpeed = 5f;
    public float rotationSpeed = 2f;
    public float collisionAvoidanceDistance = 5f; 
    public Vector2 boundaryX = new Vector2(-125f, 125f);
    public Vector2 boundaryY = new Vector2(20f, 100f);
    public Vector2 boundaryZ = new Vector2(0f, 250f);

    float visDist = 10.0f;
    float visAngle = 30.0f;
    float shootDist = 7.0f;

    public DragonState(GameObject _npc, Animator _anim, Transform _player) {
        npc = _npc;
        anim = _anim;
        player = _player;
        stage = EVENT.ENTER;
    }

    public virtual void Enter() { stage = EVENT.UPDATE; }
    public virtual void Update() { stage = EVENT.UPDATE; }
    public virtual void Exit() { stage = EVENT.EXIT; }

    public DragonState Process() {

        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT) {

            Exit();
            return nextState;
        }

        return this;
    }

    protected void MoveTowardsDestination(Vector3 _destination) {
        destination = _destination;
        npc.transform.position = Vector3.MoveTowards(npc.transform.position, destination, flyingSpeed * Time.deltaTime);

        // Rotate towards the destination
        Quaternion targetRotation = Quaternion.LookRotation(destination - npc.transform.position);
        npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (Vector3.Distance(npc.transform.position, destination) < 1.0f )
        {
            SetRandomDestination();
        }
    }

    protected void SetRandomDestination() {
        float randomX = Random.Range(boundaryX.x, boundaryX.y);
        float randomY = Random.Range(boundaryY.x, boundaryY.y);
        float randomZ = Random.Range(boundaryZ.x, boundaryZ.y);
        destination = new Vector3(randomX, randomY, randomZ);
        Debug.DrawLine(npc.transform.position, destination, Color.blue, 1.0f);
        // Debug.DrawRay(destination, Vector3.up, Color.red, 1.0f);
    }
    public bool IsObstacleInPath()
    {
        RaycastHit hit;
        // Raycast forward from the current position to the target position
        if (Physics.Raycast(npc.transform.position, destination - npc.transform.position, out hit, collisionAvoidanceDistance))
        {
            // If an obstacle is hit, return true
            return true;
        }
        // No obstacle detected, return false
        return false;
    }

    public bool CanSeePlayer() {

        Vector3 direction = player.position - npc.transform.position;
        float angle = Vector3.Angle(direction, npc.transform.forward);

        if (direction.magnitude < visDist && angle < visAngle) {

            return true;
        }

        return false;
    }

    public float detectionRadius = 40.0f; 
    public bool IsPlayerNearby() {
        if (player != null) {
            float distanceToPlayer = Vector3.Distance(npc.transform.position, player.position);
            return distanceToPlayer <= detectionRadius;
        }
        return false;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(npc.transform.position, detectionRadius); // Draw a wireframe sphere representing the detection radius
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(npc.transform.position, collisionAvoidanceDistance); 
        
    }

    public bool IsPlayerBehind() {

        Vector3 direction = npc.transform.position - player.position;
        float angle = Vector3.Angle(direction, npc.transform.forward);
        if (direction.magnitude < 2.0f && angle < 30.0f) return true;
        return false;
    }

    public bool CanAttackPlayer() {

        Vector3 direction = player.position - npc.transform.position;
        if (direction.magnitude < shootDist) {

            return true;
        }

        return false;
    }

    protected void SetDestination(Vector3 _destination) {
        destination = _destination;
    }
}

public class DragonIdle : DragonState {

    public DragonIdle(GameObject _npc, Animator _anim, Transform _player)
        : base(_npc, _anim, _player) {
        name = STATE.IDLE;
    }

    public override void Enter() {
        anim.SetTrigger("IdleSimple");
        base.Enter();
    }

    public override void Update() {
        if (Random.Range(0, 100) < 10) {
            nextState = new DragonFlyRandomly(npc, anim, player);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit() {
        anim.ResetTrigger("IdleSimple");
        base.Exit();
    }
}

public class DragonFlyRandomly : DragonState {

    public DragonFlyRandomly(GameObject _npc, Animator _anim, Transform _player)
        : base(_npc, _anim, _player) {
        name = STATE.PATROL;
        flyingSpeed = 20.0f; // Adjust the flying speed for a dragon
    }

    public override void Enter() {
        anim.SetTrigger("FlyingFWD");
        SetRandomDestination(); // Set an initial random destination on enter
        base.Enter();
    }

    public override void Update() {
        // The dragon will move towards the destination in every frame
        MoveTowardsDestination(destination);

        // Decide whether to change state
        if (Random.Range(0, 100) < 1) {
            anim.SetTrigger("Drakaris");
            // nextState = new DragonIdle(npc, anim, player);
            // stage = EVENT.EXIT;
        } else if (CanSeePlayer()) {
            nextState = new DragonCirclePlayer(npc, anim, player);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit() {
        anim.ResetTrigger("FlyingFWD");
        anim.ResetTrigger("Drakaris");
        base.Exit();
    }
}

public class DragonCirclePlayer : DragonState {

    public DragonCirclePlayer(GameObject _npc, Animator _anim, Transform _player)
        : base(_npc, _anim, _player) {
        name = STATE.PURSUE;
        flyingSpeed = 25.0f; // Adjust the circling speed for a dragon
    }

    public override void Enter() {
        anim.SetTrigger("Hover");
        base.Enter();
    }

    public override void Update() {
        Vector3 circlePosition = player.position + Quaternion.Euler(0, Time.time * 30f, 0) * new Vector3(0, 0, 10f);
        MoveTowardsDestination(circlePosition);

        if (Vector3.Distance(npc.transform.position, player.position) > 10.0f) {
            nextState = new DragonAttack(npc, anim, player);
            stage = EVENT.EXIT;
        } else if (!CanSeePlayer()) {
            nextState = new DragonFlyRandomly(npc, anim, player);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit() {
        anim.ResetTrigger("Hover");
        base.Exit();
    }
}

public class DragonAttack : DragonState {

    public DragonAttack(GameObject _npc, Animator _anim, Transform _player)
        : base(_npc, _anim, _player) {
        name = STATE.ATTACK;
    }

    public override void Enter() {
        anim.SetTrigger("FlyingAttack");
        // Implement attack behavior, fire breathing or physical attack
        base.Enter();
    }

    public override void Update() {
        anim.ResetTrigger("Bite");

        if (Vector3.Distance(npc.transform.position, player.position) > 10.0f) {
            nextState = new DragonIdle(npc, anim, player);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit() {
        anim.ResetTrigger("FlyingAttack");
        base.Exit();
    }
}

public class DragonRestingState : DragonState {

    public DragonRestingState(GameObject _npc, Animator _anim, Transform _player)
        : base(_npc, _anim, _player) {
        name = STATE.SLEEP;
    }

    public override void Enter() {
        // Choose an idle animation based on some condition
        if (npc.GetComponent<DragonHealth>().IsLowHealth()) {
            anim.SetTrigger("IdleAgressive");
        } else if (IsPlayerNearby()) {
            anim.SetTrigger("IdleRestless");
        } else {
            anim.SetTrigger("IdleSimple");
        }
        base.Enter();
    }

    public override void Update() {
        // In Update, you might check for various conditions to transition out of the resting state
        if (CanSeePlayer()) {
            nextState = new DragonAggressiveState(npc, anim, player); // Transition to aggressive state if the player is spotted
            stage = EVENT.EXIT;
        } else if (npc.GetComponent<DragonHealth>().IsLowHealth()) {
            // Stay in aggressive idle if health is low, maybe add some growling sound effect or effect to show the dragon is hurt
            anim.SetTrigger("IdleAgressive");
        } else {
            // Perhaps the dragon will occasionally switch between idle animations
            int randomIdle = Random.Range(0, 2);
            if (randomIdle == 0) {
                anim.SetTrigger("IdleSimple");
            } else {
                anim.SetTrigger("IdleRestless");
            }
        }

        // Additional logic for when the dragon should wake up or perform other actions...
    }

    public override void Exit() {
        // Upon exiting the resting state, reset any resting-specific triggers or status effects
        anim.ResetTrigger("IdleSimple");
        anim.ResetTrigger("IdleAgressive");
        anim.ResetTrigger("IdleRestless");

        // Maybe the dragon stretches or shakes itself as it wakes up
        anim.SetTrigger("TakeOff");

        // Other cleanup operations as necessary
        // ...

        base.Exit();
    }
}

public class DragonAggressiveState : DragonState {
    private float chaseDistance = 20.0f; // The distance within which the dragon starts to chase the player

    public DragonAggressiveState(GameObject _npc, Animator _anim, Transform _player)
        : base(_npc, _anim, _player) {
        name = STATE.AGGRESSIVE;
    }

    public override void Enter() {
        anim.SetTrigger("BattleStance");
        base.Enter();
    }

    public override void Update() {
        Vector3 directionToPlayer = player.position - npc.transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (CanAttackPlayer()) {
            anim.SetTrigger("Bite");
            // Attack logic here (e.g., reduce player health)
        } else if (CanSeePlayer()) {
            if (distanceToPlayer <= chaseDistance) {
                // Chase the player
                ChasePlayer();
            } else {
                // Player is visible but too far to chase, so maybe breathe fire
                anim.SetTrigger("Drakaris");
                // Fire breathing (shoot a fireball towards the player)
            }
        } else {
            // If the player is no longer visible
            nextState = new DragonIdle(npc, anim, player);
            stage = EVENT.EXIT;
        }
    }

    private void ChasePlayer() {
        // Ensure the dragon is moving towards the player
        SetDestination(player.position);
        anim.SetTrigger("Walk"); 
        MoveTowardsDestination(destination); 
    }

    public override void Exit() {
        anim.ResetTrigger("BattleStance");
        anim.ResetTrigger("Bite");
        anim.ResetTrigger("Drakaris");
        anim.ResetTrigger("Walk"); 
        base.Exit();
    }
}

