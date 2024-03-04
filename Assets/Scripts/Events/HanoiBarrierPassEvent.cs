using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class HanoiBarrierPassEvent : EventBase
    {

        public HanoiBarrierPassEvent(string name = nameof(HanoiBarrierPassEvent), float delay = 0f) : base(name, delay)
        {
        }
    }
}
