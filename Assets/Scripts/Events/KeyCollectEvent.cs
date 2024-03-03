using Items;
using Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class KeyCollectEvent : EventBase
    {
        public int keyIndex;
        public ItemAccessbility playerType;
        public KeyCollectEvent(int keyIndex, ItemAccessbility playerType, string name = nameof(KeyCollectEvent), float delay = 0f) : base(name, delay)
        {
            this.keyIndex = keyIndex;
            this.playerType = playerType;
        }
    }
}