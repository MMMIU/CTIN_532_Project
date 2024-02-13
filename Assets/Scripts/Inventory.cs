using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory main;
    // Start is called before the first frame update
    public bool key1 = false;
    void Awake()
    {
        main = this;
    }

    public bool HasKey1()
    {
        return key1;
    }

    public void GetKey1()
    {
        key1 = true;
    }
}
