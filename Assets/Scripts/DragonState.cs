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
    protected NavMeshAgent agent;
    protected Animator anim;
    protected Transform player;
    protected DragonState nextState;
    protected Vector3 destination;

    float visDist = 10.0f;
    float visAngle = 30.0f;
    float shootDist = 7.0f;

    public DragonState(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player) {
        npc = _npc;
        agent = _agent;
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

    public DragonIdle(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
        : base(_npc, _agent, _anim, _player) {
        name = STATE.IDLE;
    }

    public override void Enter() {
        anim.SetTrigger("IdleSimple");
        base.Enter();
    }

    public override void Update() {
        if (Random.Range(0, 100) < 10) {
            nextState = new DragonFlyRandomly(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit() {
        anim.ResetTrigger("IdleSimple");
        base.Exit();
    }
}

public class DragonFlyRandomly : DragonState {

    public DragonFlyRandomly(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
        : base(_npc, _agent, _anim, _player) {
        name = STATE.PATROL;
        agent.speed = 5.0f; // Adjust the flying speed for a dragon
        agent.isStopped = false;
    }

    public override void Enter() {
        anim.SetTrigger("FlyingFWD");
        base.Enter();
    }

    public override void Update() {
        if (Random.Range(0, 100) < 10) {
            nextState = new DragonIdle(npc, agent, anim, player);
            stage = EVENT.EXIT;
        } else if (CanSeePlayer()) {
            nextState = new DragonCirclePlayer(npc, agent, anim, player);
            stage = EVENT.EXIT;
        } else {
            // Implement random flying behavior
            SetDestination(GetRandomDestination());
        }
    }

    private Vector3 GetRandomDestination() {
        // Implement logic to get a random flying destination
        // You may want to consider the dragon's flight boundaries
        // For simplicity, you can just use a random point within a certain range for this example.
        return new Vector3(Random.Range(-120f, 0f), Random.Range(5f, 15f), Random.Range(-20f, 20f));
    }

    public override void Exit() {
        anim.ResetTrigger("FlyingFWD");
        base.Exit();
    }
}

public class DragonCirclePlayer : DragonState {

    public DragonCirclePlayer(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
        : base(_npc, _agent, _anim, _player) {
        name = STATE.PURSUE;
        agent.speed = 8.0f; // Adjust the circling speed for a dragon
        agent.isStopped = false;
    }

    public override void Enter() {
        anim.SetTrigger("Hover");
        base.Enter();
    }

    public override void Update() {
        
        // For simplicity, you can just orbit around the player's position.
        Vector3 circlePosition = player.position + Quaternion.Euler(0, Time.time * 30f, 0) * new Vector3(0, 0, 10f);
        agent.SetDestination(circlePosition);

        if (Vector3.Distance(npc.transform.position, player.position) > 10.0f) {
            nextState = new DragonAttack(npc, agent, anim, player);
            stage = EVENT.EXIT;
        } else if (!CanSeePlayer()) {
            nextState = new DragonFlyRandomly(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit() {
        anim.ResetTrigger("Hover");
        base.Exit();
    }
}

public class DragonAttack : DragonState {

    public DragonAttack(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
        : base(_npc, _agent, _anim, _player) {
        name = STATE.ATTACK;
    }

    public override void Enter() {
        anim.SetTrigger("FlyingAttack");
        agent.isStopped = true;
        // Implement attack behavior, fire breathing or physical attack
        base.Enter();
    }

    public override void Update() {
        anim.ResetTrigger("Bite");

        if (Vector3.Distance(npc.transform.position, player.position) > 10.0f) {
            nextState = new DragonIdle(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit() {
        anim.ResetTrigger("FlyingAttack");
        base.Exit();
    }
}

