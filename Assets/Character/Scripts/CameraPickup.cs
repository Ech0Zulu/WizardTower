using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraPickup : MonoBehaviour
{
    [SerializeField]
    private float pickupRange = 2.6f;
    [SerializeField]
    public InventoryManager inventoryManager;

    private Transform highlight;

    void Update()
    {
        RaycastHit hit;
        if (highlight != null)
        {
            highlight.gameObject.GetComponent<Outline>().enabled = false;
            highlight = null;
        }
        Debug.DrawRay(transform.position, transform.forward, Color.red);
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickupRange))
        {
            if (hit.transform.CompareTag("Pickable"))
            {
                Debug.Log("There is an item in front of us");
                highlight = hit.transform;

                if (highlight.gameObject.GetComponent<Outline>() != null)
                {
                    highlight.gameObject.GetComponent<Outline>().enabled = true;
                }
                else
                {
                    Outline outline = highlight.gameObject.AddComponent<Outline>();
                    outline.enabled = true;
                    highlight.gameObject.GetComponent<Outline>().OutlineColor = Color.magenta;
                    highlight.gameObject.GetComponent<Outline>().OutlineWidth = 7.0f;
                }
            }

            else
            {
                highlight = null;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                inventoryManager.Add();
                Destroy(hit.transform.gameObject);
            }

        }
    }
}
