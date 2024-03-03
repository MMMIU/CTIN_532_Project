using Managers;
using System;
using UnityEngine;

namespace Events
{
    public class ItemSetInteractableEvent : EventBase
    {
        public int item_uid;
        public bool interactable;

        public ItemSetInteractableEvent(int item_uid, bool interactable, float delay = 0f, string name = nameof(ItemSetInteractableEvent)) : base(name, delay)
        {
            this.item_uid = item_uid;
            this.interactable = interactable;
        }
    }
}