using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class PrincessSkillUpgradeEvent : EventBase
    {

        public PrincessSkillUpgradeEvent(string name = nameof(PrincessSkillUpgradeEvent), float delay = 0f) : base(name, delay)
        {
        }
    }
}
