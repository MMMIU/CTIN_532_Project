using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class WayPointLightUpEvent : EventBase
    {
        public string ptName;
        public bool lightUp;
        public WayPointLightUpEvent(string ptName, bool lightUp, string name = nameof(WayPointLightUpEvent), float delay = 0f) : base(name, delay)
        {
            this.ptName = ptName;
            this.lightUp = lightUp;
        }
    }
}