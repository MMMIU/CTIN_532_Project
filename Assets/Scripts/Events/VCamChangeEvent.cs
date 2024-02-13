using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class VCamChangeEvent : BaseEvent
    {
        public string vCamName;
        public VCamChangeEvent(string vCamName, string name = nameof(VCamChangeEvent), float delay = 0f) : base(name, delay)
        {
            this.vCamName = vCamName;
        }
    }
}