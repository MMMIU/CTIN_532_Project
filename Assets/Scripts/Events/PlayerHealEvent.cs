using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class PlayerHealEvent : EventBase
    {
        public ItemAccessbility playerType;
        public float amount = 0;
        public PlayerHealEvent(ItemAccessbility playerType, float amount, float delay = 0f, string name = nameof(PlayerHealEvent)) : base(name, delay)
        {
            this.playerType = playerType;
            this.amount = amount;
        }
    }
}