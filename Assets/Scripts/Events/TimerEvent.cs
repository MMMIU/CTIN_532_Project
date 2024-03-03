using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class TimerEvent : EventBase
    {
        public TimerEvent(string name = nameof(TimerEvent), float delay = 0f) : base(name, delay)
        {
        }
    }
}