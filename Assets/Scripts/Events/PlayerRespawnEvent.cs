using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class PlayerRespawnEvent : EventBase
    {
        public ItemAccessbility playerType;

        public PlayerRespawnEvent(ItemAccessbility playerType, float delay = 0f, string name = nameof(PlayerRespawnEvent)) : base(name, delay)
        {
            this.playerType = playerType;
        }
    }
}