using Events;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Items
{
    public class ItemInteractableModifier : NetworkBehaviour
    {
        [SerializeField]
        List<int> items;

        public void SetInteractable(bool interactable, float delay = 0f)
        {
            foreach (var item in items)
            {
                Debug.Log("SetInteractable " + item + " " + interactable);
                new ItemSetInteractableEvent(item, interactable, delay);
            }
        }
    }
}