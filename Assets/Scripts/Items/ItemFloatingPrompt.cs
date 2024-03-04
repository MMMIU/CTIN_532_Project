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

    [SerializeField]
    private bool disableOnInteract = true;

    [SerializeField]
    private bool reverseJudge = false;

    private void Awake()
    {
        floatingPrompt.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        itemBase.Interactable.OnValueChanged += OnInteractableChanged;
        Debug.Log("ItemFloatingPrompt OnNetworkSpawn: " + itemBase.Interactable);
    }

    public override void OnNetworkDespawn()
    {
        itemBase.Interactable.OnValueChanged -= OnInteractableChanged;
    }

    private void OnInteractableChanged(bool oldValue, bool newValue)
    {
        Debug.Log("OnInteractableChanged: " + newValue);
        bool judge = (disableOnInteract && !newValue) != reverseJudge;
        if (judge)
        {
            floatingPrompt.SetActive(false);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        bool judge = ((!disableOnInteract || itemBase.Interactable.Value) && other.CompareTag("Player"))!= reverseJudge;
        if (judge)
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
        bool judge = ((!disableOnInteract || itemBase.Interactable.Value) && other.CompareTag("Player")) != reverseJudge;
        if (judge)
        {
            Player p = other.GetComponent<Player>();
            if (p.IsLocalPlayer && (itemBase.itemDataItem.accessbility == ItemAccessbility.both || p.playerType == itemBase.itemDataItem.accessbility))
            {
                floatingPrompt.SetActive(false);
            }
        }
    }
}
