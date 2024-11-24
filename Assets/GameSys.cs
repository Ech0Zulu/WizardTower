using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSys : MonoBehaviour
{
    [SerializeField]
    public InventoryManager inventoryManager;
    [SerializeField]
    public Text CrystalCount;
    [SerializeField]
    public Text EndGame;

    private int count;
    private float time;

    private void Start()
    {
        CrystalCount.text = "x 0";
        EndGame.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (inventoryManager != null)
        {
            count = inventoryManager.GetCrystalCount();
            CrystalCount.text = "x " + count;
            if (inventoryManager.GetCrystalCount() >= 10)
            {
                EndGame.text = "GG YOU WON";
                return;
            }
        }
    }
}