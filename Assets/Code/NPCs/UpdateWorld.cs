using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class UpdateWorld : MonoBehaviour
{
    // States data
    public TextMeshProUGUI states;

    // Player and Rival Goals
    public GameObject pg_ui;
    public GameObject rg_ui;
    public GameObject[] player_goals;
    public GameObject[] rival_goals;
    public List<GameObject> player_goals_ui = new List<GameObject>();
    public List<GameObject> rival_goals_ui = new List<GameObject>();

    // Player and Agents
    public GameObject player;
    public GameObject[] moles;
    public List<GameObject> rival_HP_back = new List<GameObject>();
    public List<GameObject> rival_HP = new List<GameObject>();
    public List<GameObject> player_HP_back = new List<GameObject>();
    public List<GameObject> player_HP = new List<GameObject>();

    // Weapons and Consumable
    public GameObject[] weapons;
    public GameObject[] consumables;

    // Crystals
    public TextMeshProUGUI blueCrystalPlayer;
    public TextMeshProUGUI gemCrystalPlayer;
    public TextMeshProUGUI purpleCrystalPlayer;
    public TextMeshProUGUI redCrystalPlayer;

    public TextMeshProUGUI blueCrystalRival;
    public TextMeshProUGUI gemCrystalRival;
    public TextMeshProUGUI purpleCrystalRival;
    public TextMeshProUGUI redCrystalRival;

    // Win and Loss menus
    public GameObject WinScreen;
    public GameObject LossScreen;

    void Start()
    {
        // GOALS STATUS
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        player_goals = GameObject.FindGameObjectsWithTag("spaceship1_item");
        rival_goals = GameObject.FindGameObjectsWithTag("spaceship2_item");
        //Debug.Log("Player goals: " + player_goals.Length);
        //Debug.Log("Rival goals: " + rival_goals.Length);

        int count = 0;
        foreach (GameObject g in player_goals)
        {
            // Create game object
            ObjectItem objectItem = g.GetComponent<ObjectItem>();
            objectItem.ID = count;
            GameObject g_ui = new GameObject();
            RawImage g_image = g_ui.AddComponent<RawImage>();
            g_image.texture = objectItem.icon;
            Color whitea = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            //whitea.a = 0.5f;
            g_image.color = whitea;
            player_goals_ui.Add(g_ui);
            RectTransform rt = g_ui.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(150,150);
            g_ui.transform.position = new Vector3(100 + count* 120, 1910/2, 0);
            g_ui.transform.SetParent(pg_ui.transform);

            // Instantiate object
            Instantiate(g_ui);
            count += 1;
        }

        //RectTransform rtpgui = pg_ui.GetComponent<RectTransform>();
        //rtpgui.sizeDelta = new Vector2(-100 - count* 150,200);
        GWorld.Instance.GetWorld().ModifyState("Player Goals", count);
        GWorld.Instance.GetWorld().SetState("Player_Goals_Achieved", 0);

        count = 0;
        foreach (GameObject g in rival_goals)
        {
            // Create game object
            ObjectItem objectItem = g.GetComponent<ObjectItem>();
            objectItem.ID = count;
            GameObject g_ui = new GameObject();
            RawImage g_image = g_ui.AddComponent<RawImage>();
            g_image.texture = objectItem.icon;
            Color whitea = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            //whitea.a = 0.5f;
            g_image.color = whitea;
            rival_goals_ui.Add(g_ui);
            RectTransform rt = g_ui.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(150,150);
            g_ui.transform.position = new Vector3(1920 - 100 - count* 120, 1910/2, 0);
            g_ui.transform.SetParent(rg_ui.transform);

            // Instantiate object
            Instantiate(g_ui);
            count += 1;
        }

        //RectTransform rtrgui = rg_ui.GetComponent<RectTransform>();
        //rtrgui.sizeDelta = new Vector2(-100 - count* 100,120);
        GWorld.Instance.GetWorld().ModifyState("Rival Goals", count);
        GWorld.Instance.GetWorld().SetState("Rival_Goals_Achieved", 0);

        // AGENT STATUS
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        player = GameObject.FindGameObjectWithTag("Player");
        moles = GameObject.FindGameObjectsWithTag("mole");

        count = 0;
        foreach (GameObject g in moles)
        {
            // Call the mole and generate its ID
            MoleAgent moleAgent = g.GetComponent<MoleAgent>();
            moleAgent.moleParams.ID = count;

            // Create its HP bar


            count += 1;
        }

        GWorld.Instance.GetWorld().ModifyState("Moles", count);

        // WEAPONS AND CONSUMABLES
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        weapons = GameObject.FindGameObjectsWithTag("weapon");
        consumables = GameObject.FindGameObjectsWithTag("consumable");

        count = 0;
        foreach (GameObject g in weapons)
        {
            // Generate its ID
            WeaponItem weaponItem = g.GetComponent<WeaponItem>();
            weaponItem.weaponDescrib.ID = count;
            count += 1;
        }
        GWorld.Instance.GetWorld().ModifyState("Weapons", count);

        count = 0;
        foreach (GameObject g in consumables)
        {
            // Generate its ID
            EdibleItem edibleItem = g.GetComponent<EdibleItem>();
            edibleItem.edibleDescrib.ID = count;
            count += 1;
        }
        GWorld.Instance.GetWorld().ModifyState("Consumables", count);
    }
    
    void LateUpdate()
    {
        // Define World state
        Dictionary<string, float> worldStates = GWorld.Instance.GetWorld().GetStates();
        states.text = "";
        foreach(KeyValuePair<string, float> s in worldStates)
        {
            states.text += s.Key + ", " + s.Value + "\n";

            if (s.Key == "Player_Goals_Achieved")
            {
                BitArray b = new BitArray(new int[] { (int)s.Value });
                bool[] bits = new bool[b.Count];
                b.CopyTo(bits, 0);
                //for(int i = 0; i < b.Count; i++)
                //{
                //    Debug.Log("Player goal achieved - Element " + i +  ": " + (bits[i]));
                //}

                for(int i = 0; i < player_goals_ui.Count; i++)
                {
                    RawImage g_image = player_goals_ui[i].GetComponent<RawImage>();
                    Color whitea = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                    if(bits[i]) {
                        whitea = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    }
                    g_image.color = whitea;
                }
            }
            else if (s.Key == "Rival_Goals_Achieved")
            {
                BitArray b = new BitArray(new int[] { (int)s.Value });
                bool[] bits = new bool[b.Count];
                b.CopyTo(bits, 0);

                for(int i = 0; i < rival_goals_ui.Count; i++)
                {
                    RawImage g_image = rival_goals_ui[i].GetComponent<RawImage>();
                    Color whitea = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                    if(bits[i]) {
                        whitea = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    }
                    g_image.color = whitea;
                }
            }
            else if (s.Key == "Player Red Crystal Count")
            {
                redCrystalPlayer.text = "" + (int)s.Value;
            }
            else if (s.Key == "Player Purple Crystal Count")
            {
                purpleCrystalPlayer.text = "" + (int)s.Value;
            }
            else if (s.Key == "Player Gem Crystal Count")
            {
                gemCrystalPlayer.text = "" + (int)s.Value;
            }
            else if (s.Key == "Player Blue Crystal Count")
            {
                blueCrystalPlayer.text = "" + (int)s.Value;
            }
            else if (s.Key == "Rival Red Crystal Count")
            {
                redCrystalRival.text = "" + (int)s.Value;
            }
            else if (s.Key == "Rival Purple Crystal Count")
            {
                purpleCrystalRival.text = "" + (int)s.Value;
            }
            else if (s.Key == "Rival Gem Crystal Count")
            {
                gemCrystalRival.text = "" + (int)s.Value;
            }
            else if (s.Key == "Rival Blue Crystal Count")
            {
                blueCrystalRival.text = "" + (int)s.Value;
            }
        }

        // Available items
        GameObject[] spaceship1_items = GameObject.FindGameObjectsWithTag("spaceship1_item");
        foreach(GameObject g in spaceship1_items)
        {
            ObjectItem objectItem = g.GetComponent<ObjectItem>();
            if(g.transform.parent == null)
            {
                GWorld.Instance.GetWorld().SetState("Available Babo SSItem " + objectItem.ID, 1);
            }
            else
            {
                GWorld.Instance.GetWorld().RemoveState("Available Babo SSItem " + objectItem.ID);
            }
        }

        GameObject[] spaceship2_items = GameObject.FindGameObjectsWithTag("spaceship2_item");
        foreach(GameObject g in spaceship2_items)
        {
            ObjectItem objectItem = g.GetComponent<ObjectItem>();
            if(g.transform.parent == null)
            {
                GWorld.Instance.GetWorld().SetState("Available Mole SSItem " + objectItem.ID, 1);
            }
            else
            {
                GWorld.Instance.GetWorld().RemoveState("Available Mole SSItem " + objectItem.ID);
            }
        }

        GameObject[] weapon_items = GameObject.FindGameObjectsWithTag("weapon");
        foreach(GameObject g in weapon_items)
        {
            WeaponItem weaponItem = g.GetComponent<WeaponItem>();
            if(g.transform.parent == null)
            {
                GWorld.Instance.GetWorld().SetState("Available Weapon " + weaponItem.weaponDescrib.ID, 1);
            }
            else
            {
                GWorld.Instance.GetWorld().RemoveState("Available Weapon " + weaponItem.weaponDescrib.ID);
            }
        }

        GameObject[] edible_items = GameObject.FindGameObjectsWithTag("consumable");
        foreach(GameObject g in edible_items)
        {
            EdibleItem edibleItem = g.GetComponent<EdibleItem>();
            if(g.transform.parent == null)
            {
                GWorld.Instance.GetWorld().SetState("Available Edible " + edibleItem.edibleDescrib.ID, 1);
            }
            else
            {
                GWorld.Instance.GetWorld().RemoveState("Available Edible " + edibleItem.edibleDescrib.ID);
            }
        }

        // Win and Loss Menus
        bool achievedGoals = true;
        BitArray b2 = new BitArray(new int[] { (int)GWorld.Instance.GetWorld().GetState("Player_Goals_Achieved") });
        bool[] bits2 = new bool[b2.Count];
        b2.CopyTo(bits2, 0);
        for(int i = 0; i < player_goals_ui.Count; i++)
        {
            if(bits2[i] == false)
            {
                achievedGoals = false;
            }
        }


        if(achievedGoals)
        {
            WinScreen.SetActive(true);
        }
        else
        {
            WinScreen.SetActive(false);
        }

        ResourceInteraction resourceInteraction = player.GetComponent<ResourceInteraction>();
        if(resourceInteraction.agentParams.HP <= 0)
        {
            LossScreen.SetActive(true);
        }
        else
        {
            LossScreen.SetActive(false);
        }

    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("Game");
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
