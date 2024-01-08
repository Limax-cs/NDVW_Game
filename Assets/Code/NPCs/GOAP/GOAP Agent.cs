using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAPAgent : GAgent
{

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    HYPERPARAMETERS
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    // Goals
    private GameObject[] mole_goals;

    // Actions
    //private List<GAction> compActions = new List<GAction>();

    // Agent Senses
    public GameObject targetDirection;

    public LayerMask pickableLayerMask;

    [Min(1)]
    public float range = 7.0f;
    public RaycastHit hit;
    public Vector3 centerBias;

    public Transform pickUpParent;
    public Transform pickUpParentStatic;

    // Agent Healtbar
    public float HP = 1;
    public float MaxHP = 1;
    public GameObject healthIndicator;


    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    INITIALIZE GOALS AND ACTIONS
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/


    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Status 1");
        // Use base start
        base.Start();

        //Debug.Log("Status 2");

        // Update Game Actions
        GAction[] acts = this.GetComponents<GAction>();
        foreach (GAction a in acts)
            actions.Add(a);

        //Debug.Log("Status 3");

        // Instantiate Spaceship Goals
        mole_goals = GameObject.FindGameObjectsWithTag("spaceship2_item");
        foreach (GameObject g in mole_goals)
        {
            ObjectItem objectItem = g.GetComponent<ObjectItem>();
            SubGoal sg = new SubGoal("Recover " + objectItem.ID, objectItem.ID, true);
            goals.Add(sg, 1);
            this.beliefs.SetState("Detect Mole SSItem " + objectItem.ID, 1);
            
        }
        //this.beliefs.SetState("Detect Mole SSItem 2", 1);

        //Debug.Log("Status 4");

        // Instantiate Spaceship recovery actions
        Recover recover = this.gameObject.AddComponent<Recover>();
        actions.Add(recover);

        //Debug.Log("Status 5");

        // Add fixing spaceship goal
        SubGoal fsg = new SubGoal("Fix Spaceship", 1, true);
        goals.Add(fsg, 1);

        //Debug.Log("Status 6");

        // Include exploration
        SubGoal sge = new SubGoal("Explore", 1, false);
        goals.Add(sge, 1);
        Explore act = this.gameObject.AddComponent<Explore>();
        actions.Add(act);

        //Debug.Log("Status 7");
        
    }

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    UPDATE STATUS
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    void LateUpdate()
    {
        // Update Beliefs
        UpdateBeliefs();

        //string showBeliefs = "";
        //foreach (KeyValuePair<string, float> state in this.beliefs.GetStates()){ showBeliefs += state.Key + "(" + state.Value + "), ";}
        //Debug.Log("Beliefs: " + showBeliefs);

        // Update Actions
        UpdateActions();

        // Update backpack visibility
        for(int k=0; k < this.backpack.Count; k++)
        {
            if (k == this.indexItem)
                MakeVisible(k);
            else
                MakeNotVisible(k);
        }

        // Update Health
        healthIndicator.transform.localScale = new Vector3((float) HP/MaxHP, 0.0f, 0.0f);


        // Call parent
        base.LateUpdate();

    }

    private void MakeVisible(int item)
    {
        if (backpack[item] != null)
        {
            MeshRenderer mr = backpack[item].GetComponent<MeshRenderer>();
            mr.enabled = true;
            Collider coll = backpack[item].GetComponent<Collider>();
            coll.enabled = true;
            coll.isTrigger = true;
            backpack[item].GetComponent<Collider>().GetComponent<Highlight>()?.ToggleHighLight(false);
        }
    }

    private void MakeNotVisible(int item)
    {
        if (backpack[item] != null)
        {
            MeshRenderer mr = backpack[item].GetComponent<MeshRenderer>();
            mr.enabled = false;
            Collider coll = backpack[item].GetComponent<Collider>();
            coll.enabled = false;
        }
    }

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    UPDATE ACTIONS
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    void UpdateActions()
    {
        //actions.Clear();

        // Add base Actions
        //foreach (GAction a in compActions)
        //    actions.Add(a);

        // Set agent params
        /*
        GAction[] acts = this.GetComponents<GAction>();
        foreach (GAction a in acts)
        {
            a.SetAgentParams(this.agent, this.beliefs, this.backpack,  this.targetDirection, this.range);
        }*/
    }

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    UPDATE BELIEFS
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    void UpdateBeliefs()
    {
        // Remove Beliefs
        List<string> backpack_states = new List<string>();

        foreach(KeyValuePair<string, float> s in this.beliefs.GetStates())
        {   if (s.Key.StartsWith("Has")) {   backpack_states.Add(s.Key);} 
            if (s.Key.StartsWith("FreeSpace")) {   backpack_states.Add(s.Key);} }

        foreach(string s in backpack_states)
        {   this.beliefs.RemoveState(s);    }

        // Items in backpack
        for(int k = 0; k < this.backpack.Count; k++)
        {
            if(this.backpack[k] is null)
            {
                this.beliefs.ModifyState("FreeSpace", 1);
            }
            else
            {
                if(this.backpack[k].tag == "spaceship1_item")
                {
                    ObjectItem objectItem = this.backpack[k].GetComponent<ObjectItem>();
                    this.beliefs.SetState("Has Babo SSItem " + objectItem.ID, k);
                }
                else if(this.backpack[k].tag == "spaceship2_item")
                {
                    ObjectItem objectItem = this.backpack[k].GetComponent<ObjectItem>();
                    this.beliefs.SetState("Has Mole SSItem " + objectItem.ID, k);
                }
                else
                {
                    this.beliefs.SetState("Has " + this.backpack[k].name, k);
                }
            }
        }

        // Collectable items
        if (this.beliefs.HasState("FreeSpace"))
        {
            // Draw Raycast
            Vector3 heading = targetDirection.transform.position - centerBias - transform.position;
            Debug.DrawRay(transform.position + centerBias, heading/heading.magnitude * range, Color.green);
            
            // De-activate previous collider illuminations
            if (hit.collider != null)
            {
                hit.collider.GetComponent<Highlight>()?.ToggleHighLight(false);
            }

            // Detect object
            if(Physics.Raycast(
                transform.position + centerBias, 
                heading/heading.magnitude, 
                out hit, 
                range,
                pickableLayerMask))
            {
                // Add agent believes
                hit.collider.GetComponent<Highlight>()?.ToggleHighLight(true);
                if (GetComponent<Collider>().CompareTag("spaceship2_item"))
                {
                    ObjectItem objectItem = GetComponent<Collider>().GetComponent<ObjectItem>();
                    this.beliefs.ModifyState("Collectable Mole SSItem " + objectItem.ID, 1);
                }

                if (GetComponent<Collider>().CompareTag("spaceship1_item"))
                {
                    ObjectItem objectItem = GetComponent<Collider>().GetComponent<ObjectItem>();
                    this.beliefs.ModifyState("Collectable Babo SSItem " + objectItem.ID, 1);
                }
            }

        }

        // Location
        //this.beliefs.SetState("Agent X POS", this.transform.position[0]);
        //this.beliefs.SetState("Agent Y POS", this.transform.position[1]);
        //this.beliefs.SetState("Agent Z POS", this.transform.position[2]);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("spaceship1"))
        {
            this.beliefs.ModifyState("InSpaceship1", 1);
        }
        if (collider.CompareTag("spaceship2"))
        {
            this.beliefs.ModifyState("InSpaceship2", 1);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("spaceship1"))
        {
            this.beliefs.RemoveState("InSpaceship1");
        }
        if (collider.CompareTag("spaceship2"))
        {
            this.beliefs.RemoveState("InSpaceship2");
        }
    }

}
