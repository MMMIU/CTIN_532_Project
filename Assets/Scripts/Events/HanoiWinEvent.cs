using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class HanoiWinEvent : EventBase
    {

        public HanoiWinEvent(string name = nameof(HanoiWinEvent), float delay = 0f) : base(name, delay)
        {
        }
    }
}
