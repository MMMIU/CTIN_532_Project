using Managers;
using Players;
using System;
using UnityEngine;

namespace Events
{
    public class SpawnPlayerEvent : BaseEvent
    {
        public ulong playerId;

        public SpawnPlayerEvent(ulong playerId, string name = nameof(SpawnPlayerEvent), float delay = 0f) : base(name, delay)
        {
            this.playerId = playerId;
        }
    }
}