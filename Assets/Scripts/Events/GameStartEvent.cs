using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class GameStartEvent : EventBase
    {

        public GameStartEvent(string name = nameof(GameStartEvent), float delay = 0f) : base(name, delay)
        {
        }
    }
}
