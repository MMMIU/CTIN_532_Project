using Quest;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LockedDoor : NetworkBehaviour
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
        if (Inventory.main.HasKey1)
        {
            UnlockServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UnlockServerRpc()
    {
        UnlockClientRpc();
        if (TryGetComponent(out QuestProgressModifier questProgressModifier))
        {
            questProgressModifier.AddProgress();
        }
    }

    [ClientRpc]
    public void UnlockClientRpc()
    {
        Destroy(gameObject);
    }


}
