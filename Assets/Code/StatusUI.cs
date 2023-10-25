using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    public List<Image> slots = new List<Image>(9);
    public List<Image> items = new List<Image>(9);
    private int itemIndex = 4;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < slots.Count; i++)
        {

            if (i == itemIndex)
                slots[i].color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
            else
                slots[i].color = new Color(1.0f, 1.0f, 1.0f, 0.04f);

            if (i == itemIndex && i == 4)
                slots[i].color = new Color(0.0f, 0.0f, 0.0f, 0.2f);
            else if (i == 4)
                slots[i].color = new Color(0.0f, 0.0f, 0.0f, 0.82f);

            
        }

        for (int i = 0; i < items.Count; i++)
        {
            if (i == itemIndex)
                items[i].rectTransform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            else
                items[i].rectTransform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        }
    }

    public void setElements(List<GameObject> items_list, int itemIdx)
    {
        // Objects
        for (int i = 0; i < items_list.Count; i++)
        {
            if (items_list[i] == null)
                items[i].enabled = false;
            else
                items[i].enabled = true;
        }

        // Index
        itemIndex = itemIdx;
    }
}
