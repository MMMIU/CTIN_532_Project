using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class ClickableHintEvent : EventBase
    {
        public bool show;

        public ClickableHintEvent(bool show, string name = nameof(ClickableHintEvent), float delay = 0f) : base(name, delay)
        {
            this.show = show;
        }
    }
}