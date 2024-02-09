using Items;
using Players;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemFloatingPrompt : NetworkBehaviour
{
    [SerializeField]
    private GameObject floatingPrompt;

    [SerializeField]
    private ItemBase itemBase;

    public override void OnNetworkSpawn()
    {
        itemBase.Interactable.OnValueChanged += OnInteractableChanged;
        floatingPrompt.SetActive(itemBase.itemDataItem.interactable);
    }

    public override void OnNetworkDespawn()
    {
        itemBase.Interactable.OnValueChanged -= OnInteractableChanged;
    }

    private void OnInteractableChanged(bool oldValue, bool newValue)
    {
        Debug.Log("OnInteractableChanged: " + newValue);
        if (newValue == false)
        {
            floatingPrompt.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (itemBase.Interactable.Value && other.CompareTag("Player"))
        {
            Player p = other.GetComponent<Player>();
            if (p.IsLocalPlayer && (itemBase.itemDataItem.accessbility == ItemAccessbility.both || p.playerType == itemBase.itemDataItem.accessbility))
            {
                floatingPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (itemBase.Interactable.Value && other.CompareTag("Player"))
        {
            Player p = other.GetComponent<Player>();
            if (p.IsLocalPlayer && (itemBase.itemDataItem.accessbility == ItemAccessbility.both || p.playerType == itemBase.itemDataItem.accessbility))
            {
                floatingPrompt.SetActive(false);
            }
        }
    }
}
