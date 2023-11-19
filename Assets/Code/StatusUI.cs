using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    public List<Image> slots = new List<Image>(9);
    public List<RawImage> items = new List<RawImage>(9);
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
            {
                items[i].enabled = false;   
            }
            else
            {
                items[i].enabled = true;

                ObjectItem objectItem = items_list[i].GetComponent<ObjectItem>();
                EdibleItem edibleItem = items_list[i].GetComponent<EdibleItem>();
                WeaponItem weaponItem = items_list[i].GetComponent<WeaponItem>();

                if (objectItem != null){
                    if (objectItem.icon != null)
                        items[i].texture = objectItem.icon;
                }
                else if (edibleItem != null){
                    if (edibleItem.icon != null)
                        items[i].texture = edibleItem.icon;
                }
                else if (weaponItem != null){
                    if (weaponItem.icon != null)
                        items[i].texture = weaponItem.icon;
                }
            }
        }

        // Index
        itemIndex = itemIdx;
    }
}