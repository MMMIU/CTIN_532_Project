using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class PlayerDataUpdateEvent : BaseEvent
    {
        public PlayerDataUpdateEvent(float delay = 0f, string name = nameof(PlayerDataUpdateEvent)) : base(name, delay)
        {
        }
    }
}