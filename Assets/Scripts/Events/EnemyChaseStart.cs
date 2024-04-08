using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class EnemyChaseStart : EventBase
    {
        public EnemyChaseStart(float delay = 0f, string name = nameof(EnemyChaseStart)) : base(name, delay)
        {
        }
    }
}