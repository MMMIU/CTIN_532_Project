using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    // Start is called before the first frame update

    public void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TryOpen();
        }
    }

    public void TryOpen()
    {
        if (Inventory.main.HasKey1())
        {
            this.gameObject.SetActive(false);
        }
    }
}
