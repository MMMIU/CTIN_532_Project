using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class EnemyChaseEnd : EventBase
    {
        public EnemyChaseEnd(float delay = 0f, string name = nameof(EnemyChaseEnd)) : base(name, delay)
        {
        }
    }
}