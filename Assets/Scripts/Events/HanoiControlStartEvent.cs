using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class HanoiControlStartEvent : EventBase
    {

        public HanoiControlStartEvent(string name = nameof(HanoiControlStartEvent), float delay = 0f) : base(name, delay)
        {
        }
    }
}