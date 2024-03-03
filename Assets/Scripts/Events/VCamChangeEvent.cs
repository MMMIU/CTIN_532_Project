using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class VCamChangeEvent : EventBase
    {
        public string vCamName;
        public bool useCustomPos;
        public Vector3 customPos;

        public VCamChangeEvent(string vCamName = "", bool useCustomPos = false, Vector3 customPos = default, string name = nameof(VCamChangeEvent), float delay = 0f) : base(name, delay)
        {
            this.vCamName = vCamName;
            this.useCustomPos = useCustomPos;
            this.customPos = customPos;
        }
    }
}