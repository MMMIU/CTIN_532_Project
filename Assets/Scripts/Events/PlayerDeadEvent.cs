using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class PlayerDeadEvent : EventBase
    {
        ItemAccessbility playerType;
        public PlayerDeadEvent(ItemAccessbility playerType, float delay = 0f, string name = nameof(PlayerDeadEvent)) : base(name, delay)
        {
            this.playerType = playerType;
        }
    }
}