using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemies;
using Quest;
using Players;
namespace Events {
    public class KnightAttackEvent : BaseEvent
    {
        public GameObject other;
        public int damage;

        public KnightAttackEvent(GameObject other, int demage = 1, string name = nameof(KnightAttackEvent), float delay = 0f) : base(name, delay)
        {
            this.other = other;
            this.damage = demage;
        }
    }
}