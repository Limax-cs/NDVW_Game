/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
RE-DESIGN OF THE MOLE FROM SCRATCH
- Design the main as an utility system
- Generate subprocess with GOAP planning
- Avoid poly-morphism when not required
- Force the system to work in a controlled single thread execution (to avoid NullReference exceptions)
- Cross the fingers
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/


/* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
PACKAGES
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;


/* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
CLASSES AND CONSTRUCTS
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
public class SubGoal
{
    public Dictionary<string, float> sgoals;
    public bool remove;

    public SubGoal(string s, float i, bool r)
    {
        sgoals = new Dictionary<string, float>();
        sgoals.Add(s, i);
        remove = r;
    }
}

[System.Serializable]
public class AgentAffection
{
    public GameObject agent;
    public float affection;
}

[System.Serializable]
public class AgentCuriosity
{
    public GameObject item;
    public float curiosity;
}

[System.Serializable]
public class AgentParams
{
    public int ID;
    public float HP;
    public float MaxHP;
    public string type = "Mole";
    public float Mood;
    public float playerAffection;
    public List<AgentAffection> moleAffection;
    public List<AgentCuriosity> spaceshipCuriosity;
    public List<AgentCuriosity> weaponCuriosity;
    public List<AgentCuriosity> edibleCuriosity;
    public List<AgentCuriosity> explorableCuriosity;

    public AgentParams(int ID, float HP, float MaxHP)
    {
        this.ID = ID;
        this.HP = HP;
        this.MaxHP = MaxHP;
        this.Mood = 1.0f;
        this.playerAffection = 1.0f;
        this.moleAffection = new List<AgentAffection>();
        this.spaceshipCuriosity = new List<AgentCuriosity>();
        this.weaponCuriosity = new List<AgentCuriosity>();
        this.edibleCuriosity = new List<AgentCuriosity>();
        this.explorableCuriosity = new List<AgentCuriosity>();
    }
}

/* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
AGENT
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

public class MoleAgent : MonoBehaviour
{

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    HYPERPARAMETERS
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    // ACTION-ORIENTED PARAMETERS
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [Header("Action-Oriented Parameters")]
    public List<MoleAction> actions = new List<MoleAction>();
    public MoleAction currentAction;
    public List<float> actionScores = new List<float>();
    private GameObject[] mole_goals;
    public Dictionary<SubGoal, float> goals = new Dictionary<SubGoal, float>();
    public WorldStates beliefs = new WorldStates();
    

    // BACKPACK
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [Header("Backpack")]
    public List<GameObject> backpack;
    public int indexItem = 4;

    // NAVIGATION PARAMETERS
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [Header("Navigation")]
    public NavMeshAgent agent;

    // AGENT PARAMETERS
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [Header("Agent Parameters")]
    public AgentParams moleParams = new AgentParams(-1, 1.0f, 1.0f);
    public GameObject healthIndicator;


    // AGENT SENSES
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [Header("Agent Senses")]

    // Agent Attention
    public GameObject targetDirection;
    public LayerMask pickableLayerMask;

    [Min(1)]
    public float range = 7.0f;
    public RaycastHit hit;
    public Vector3 centerBias;

    //Agent Hand
    public Transform pickUpParent;
    public Transform pickUpParentStatic;

    //Agent Awareness
    public GameObject SenseArea;
    public MoleSenseV2 moleSensing;


    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    AGENT INITIALIZATION
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    void Start()
    {
        // COMPONENT INITIALIZATION
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    
        // Navmesh initialization
        agent = this.gameObject.GetComponent<NavMeshAgent>();

        // Initialize backpack
        backpack = new List<GameObject>();
        for (int i=0; i<9; i++)
        {
            backpack.Add(null);
        }

        // Mole Sensing
        moleSensing = SenseArea.GetComponent<MoleSenseV2>();

        // MAIN GOALS AND ACTIONS
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        // Instantiate Spaceship Goals
        mole_goals = GameObject.FindGameObjectsWithTag("spaceship2_item");
        foreach (GameObject g in mole_goals)
        {
            ObjectItem objectItem = g.GetComponent<ObjectItem>();
            SubGoal sg = new SubGoal("Recover " + objectItem.ID, objectItem.ID, true);
            goals.Add(sg, 1);
            //this.beliefs.SetState("Detect Mole SSItem " + objectItem.ID, 1);
        }
        //this.beliefs.SetState("Detect Mole SSItem 0", 1);

        // Include exploration
        SubGoal sge = new SubGoal("Explore", 1, false);
        goals.Add(sge, 1);
        MoleExplore act = this.gameObject.AddComponent<MoleExplore>();
        actions.Add(act);
        actionScores.Add(0.0f);
        currentAction = act;

        // Instantiate Spaceship recovery actions
        MoleRecover recover = this.gameObject.AddComponent<MoleRecover>();
        actions.Add(recover);
        actionScores.Add(0.0f);

        // Instantiate Spaceship recovery actions
        MoleAggresive aggressive = this.gameObject.AddComponent<MoleAggresive>();
        actions.Add(aggressive);
        actionScores.Add(0.0f);

        // Instantiate Spaceship recovery actions
        MoleCuriosity curiosity = this.gameObject.AddComponent<MoleCuriosity>();
        actions.Add(curiosity);
        actionScores.Add(0.0f);

        // UPDATE AGENT PARAMS
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        GameObject[] player_goals = GameObject.FindGameObjectsWithTag("spaceship1_item");
        GameObject[] moles = GameObject.FindGameObjectsWithTag("mole");
        GameObject[] weapons = GameObject.FindGameObjectsWithTag("weapon");
        GameObject[] consumables = GameObject.FindGameObjectsWithTag("consumable");
        GameObject[] explorable = GameObject.FindGameObjectsWithTag("explorable");
        moleParams.Mood = Random.Range(0.3f, 1.0f);
        moleParams.playerAffection = Random.Range(0.0f, 0.7f);
        
        float generalSpaceshipCuriosity = Random.Range(0.3f, 0.6f);
        foreach(GameObject g in player_goals)
        {
            AgentCuriosity agentCuriosity = new AgentCuriosity();
            agentCuriosity.item = g;
            agentCuriosity.curiosity = generalSpaceshipCuriosity + Random.Range(0.0f, 0.4f);
            moleParams.spaceshipCuriosity.Add(agentCuriosity);
        }

        foreach(GameObject g in moles)
        {
            if (g != this.gameObject)
            {
                AgentAffection agentAffection = new AgentAffection();
                agentAffection.agent = g;
                agentAffection.affection = Random.Range(0.7f, 1.0f);
                moleParams.moleAffection.Add(agentAffection);
            }
        }

        float generalWeaponsCuriosity = Random.Range(0.0f, 0.5f);
        foreach(GameObject g in weapons)
        {
            AgentCuriosity agentCuriosity = new AgentCuriosity();
            agentCuriosity.item = g;
            agentCuriosity.curiosity = generalWeaponsCuriosity + Random.Range(0.0f, 0.5f);
            moleParams.weaponCuriosity.Add(agentCuriosity);
        }

        float generalEdibleCuriosity = Random.Range(0.0f, 0.5f);
        foreach(GameObject g in consumables)
        {
            AgentCuriosity agentCuriosity = new AgentCuriosity();
            agentCuriosity.item = g;
            agentCuriosity.curiosity = generalEdibleCuriosity + Random.Range(0.0f, 0.5f);
            moleParams.edibleCuriosity.Add(agentCuriosity);
        }

        float generalExploreCuriosity = Random.Range(0.0f, 0.5f);
        foreach(GameObject g in explorable)
        {
            AgentCuriosity agentCuriosity = new AgentCuriosity();
            agentCuriosity.item = g;
            agentCuriosity.curiosity = generalExploreCuriosity + Random.Range(0.0f, 0.5f);
            moleParams.explorableCuriosity.Add(agentCuriosity);
        }

    }

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    MAIN THREAD
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
    void LateUpdate()
    {
        // Update Beliefs
        this.UpdateBeliefs();

        // Utility System
        this.UtilitySystem();

        // Update Agent Params
        this.UpdateAgentParams();
    }

    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    UPDATE BELIEFS
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public void UpdateBeliefs()
    {
        // UPDATE BACKPACK BELIEFS
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        // Remove Beliefs
        List<string> backpack_states = new List<string>();

        foreach(KeyValuePair<string, float> s in this.beliefs.GetStates())
        {   if (s.Key.StartsWith("Has")) {   backpack_states.Add(s.Key);} 
            if (s.Key.StartsWith("FreeSpace")) {   backpack_states.Add(s.Key);}
            if (s.Key.StartsWith("Collectable")) {   backpack_states.Add(s.Key);}}

        foreach(string s in backpack_states)
        {   this.beliefs.RemoveState(s);    }

        // Items in backpack
        for(int k = 0; k < this.backpack.Count; k++)
        {
            if(this.backpack[k] == null)
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
                else if(this.backpack[k].tag == "weapon")
                {
                    WeaponItem weaponItem = this.backpack[k].GetComponent<WeaponItem>();
                    this.beliefs.SetState("Has Weapon " + weaponItem.weaponDescrib.ID, k);
                }
                else if(this.backpack[k].tag == "consumable")
                {
                    EdibleItem edibleItem = this.backpack[k].GetComponent<EdibleItem>();
                    this.beliefs.SetState("Has Edible " + edibleItem.edibleDescrib.ID, k);
                }
            }
        }

        // UPDATE COLLECTABLE BELIEFS
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

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
                if (hit.collider.tag == "spaceship2_item")
                {
                    ObjectItem objectItem = hit.collider.GetComponent<ObjectItem>();
                    this.beliefs.ModifyState("Collectable Mole SSItem " + objectItem.ID, 1);
                }

                if (hit.collider.tag == "spaceship1_item")
                {
                    ObjectItem objectItem = hit.collider.GetComponent<ObjectItem>();
                    this.beliefs.ModifyState("Collectable Babo SSItem " + objectItem.ID, 1);
                }
            }

        }
        // MOLE LOCATION
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        this.beliefs.SetState("Agent X POS", this.transform.position[0]);
        this.beliefs.SetState("Agent Y POS", this.transform.position[1]);
        this.beliefs.SetState("Agent Z POS", this.transform.position[2]);

        // UPDATE SENSE BELIEFS
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        moleSensing.UpdateBeliefs(this.beliefs);


        // DEBUG BELIEFS
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        //string showBeliefs = "";
        //foreach (KeyValuePair<string, float> state in this.beliefs.GetStates()){ showBeliefs += state.Key + "(" + state.Value + "), ";}
        //Debug.Log("Beliefs: " + showBeliefs);

        //string showGoals = "";
        //foreach (KeyValuePair<SubGoal, float> state in this.goals)
        //{
        //    foreach (KeyValuePair<string, float> s in state.Key.sgoals)
        //    {
        //        showGoals += s.Key + "(" + s.Value + "), ";
        //    }
        //}
        //Debug.Log("Goals: " + showGoals);
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


    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ACTION RUNNER AND SELECTOR
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public void UtilitySystem()
    {
        // Extract the utility score
        float totalScore = 0;
        for(int k=0; k < actions.Count; k++)
        {
            // Set agent params
            actions[k].SetAgentStatus(this.goals, this.beliefs, this.backpack, this.indexItem,
                            this.targetDirection, this.pickableLayerMask, this.hit, this.centerBias, this.pickUpParent, this.pickUpParentStatic,
                            this.range, this.moleParams);

            // Extract utility score
            actionScores[k] = actions[k].ComputeUtilityScore();
            totalScore += actionScores[k];
        }

        string utilitiesValue = "";
        for(int k=0; k < actions.Count; k++)
        {
            utilitiesValue += actions[k].actionName + ": " + Mathf.Round(10000*actionScores[k])/100 + "%; ";
        }
        Debug.Log("Agent " + moleParams.ID + " - Utilities: " + utilitiesValue);

        if (totalScore > 0)
        {
            // Select the action
            float randVal = UnityEngine.Random.Range(0f, totalScore);
            float cumScore = 0;
            for(int k=0; k < actions.Count; k++)
            {
                if((randVal >= cumScore) && (randVal <= cumScore + actionScores[k]))
                {
                    currentAction = actions[k];
                    break;
                }
                cumScore += actionScores[k];
            }

            // Perform the action
            currentAction.Perform();
        }
        else
        {
            Debug.Log("Mole " + moleParams.ID + " - No action available");
        }

        
    }


    /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    UPDATE AGENT PARAMETERS
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public void UpdateAgentParams()
    {
        // Update Health
        healthIndicator.transform.localScale = new Vector3((float) moleParams.HP/moleParams.MaxHP, 1.0f, 1.0f);

        // Update backpack visibility
        for(int k=0; k < this.backpack.Count; k++)
        {
            if (k == this.indexItem)
                MakeVisible(k);
            else
                MakeNotVisible(k);
        }
    }

    private void MakeVisible(int item)
    {
        if (backpack[item] != null)
        {

            Renderer[] mrs = backpack[item].GetComponentsInChildren<Renderer>();
            foreach(Renderer mr in mrs)
            {
                mr.enabled = true;
            }
            
            backpack[item].GetComponent<Collider>().GetComponent<Highlight>()?.ToggleHighLight(false);
        }
    }

    private void MakeNotVisible(int item)
    {
        if (backpack[item] != null)
        {
            Renderer[] mrs = backpack[item].GetComponentsInChildren<Renderer>();
            foreach(Renderer mr in mrs)
            {
                mr.enabled = false;
            }

            //Collider coll = backpack[item].GetComponent<Collider>();
            //coll.enabled = false;
        }
    }

    public void UpdateIdex(int index)
    {
        this.indexItem = index;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "damage")
        {
            WeaponItem weaponItem = collision.collider.GetComponent<WeaponItem>();
            moleParams.HP = Mathf.Max(moleParams.HP - weaponItem.weaponDescrib.attack, 0.0f);
            Debug.Log("Mole Hit");
        }
    }
}
