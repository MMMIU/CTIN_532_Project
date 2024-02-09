using Managers;
using System;
using UnityEngine;

namespace Events
{
    public class ItemSetInteractableEvent : BaseEvent
    {
        public int item_uid;
        public bool interactable;

        public ItemSetInteractableEvent(int item_uid, bool interactable, string name = "ItemSetInteractableEvent", float delay = 0f) : base(name, delay)
        {
            this.item_uid = item_uid;
            this.interactable = interactable;
        }
    }
}