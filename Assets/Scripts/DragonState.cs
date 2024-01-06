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
        RUNAWAY
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

    public float flyingSpeed = 30f;
    public float rotationSpeed = 2f;

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
    }

    public bool CanSeePlayer() {

        Vector3 direction = player.position - npc.transform.position;
        float angle = Vector3.Angle(direction, npc.transform.forward);

        if (direction.magnitude < visDist && angle < visAngle) {

            return true;
        }

        return false;
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
    public Vector2 boundaryX = new Vector2(-125f, 125f);
    public Vector2 boundaryY = new Vector2(10f, 50f);
    public Vector2 boundaryZ = new Vector2(-125f, 125f);

    public DragonFlyRandomly(GameObject _npc, Animator _anim, Transform _player)
        : base(_npc, _anim, _player) {
        name = STATE.PATROL;
        flyingSpeed = 20.0f; // Adjust the flying speed for a dragon
    }

    public override void Enter() {
        anim.SetTrigger("FlyingFWD");
        base.Enter();
    }

    public override void Update() {
        if (Random.Range(0, 100) < 10) {
            nextState = new DragonIdle(npc, anim, player);
            stage = EVENT.EXIT;
        } else if (CanSeePlayer()) {
            nextState = new DragonCirclePlayer(npc, anim, player);
            stage = EVENT.EXIT;
        } else {
            SetRandomDestination();
        }

        MoveTowardsDestination(destination);
    }

    private void SetRandomDestination() {
        float randomX = Random.Range(boundaryX.x, boundaryX.y);
        float randomY = Random.Range(boundaryY.x, boundaryY.y);
        float randomZ = Random.Range(boundaryZ.x, boundaryZ.y);
        destination = new Vector3(randomX, randomY, randomZ);
        Debug.DrawRay(destination, Vector3.up, Color.red, 1.0f);
    }

    private Vector3 GetRandomDestination() {
        // This should be larger than the maximum expected distance to the edge of your NavMesh
        float maxDistance = 50f; // You can adjust this value as needed

        // Try multiple times to find a valid point
        for (int i = 0; i < 10; i++) {
            Vector3 randomDirection = Random.insideUnitSphere * maxDistance;
            randomDirection += npc.transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, maxDistance, NavMesh.AllAreas)) {
                return hit.position;
            } else {
                Debug.DrawRay(randomDirection, Vector3.up, Color.red, 1.0f); // Draw a red ray where it tried to find a position
            }
        }

        Debug.LogError("Failed to find a valid random point on the NavMesh after multiple attempts.");
        return npc.transform.position; // Fallback to current position if no valid point is found
    }



    public override void Exit() {
        anim.ResetTrigger("FlyingFWD");
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

