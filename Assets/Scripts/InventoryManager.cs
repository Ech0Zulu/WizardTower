using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private int CrystalCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        CrystalCount = 0;
    }

    public int GetCrystalCount() {return CrystalCount;}

    public void Add()
    {
        CrystalCount++;
    }

    public void Sub()
    {
        if (CrystalCount>=0) CrystalCount--;
    }

}
