using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class PrincessSkillDowngradeEvent : EventBase
    {

        public PrincessSkillDowngradeEvent(string name = nameof(PrincessSkillDowngradeEvent), float delay = 0f) : base(name, delay)
        {
        }
    }
}
