using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemies;
using Quest;
using Players;
using Items;
namespace Events {
    public class EnemyAttackEvent : BaseEvent
    {
        public ItemAccessbility playerType;
        public int damage;

        public EnemyAttackEvent(ItemAccessbility playerType, int damage = 10, string name = nameof(EnemyAttackEvent), float delay = 0f) : base(name, delay)
        {
            this.playerType = playerType;
            this.damage = damage;
            Debug.Log("EnemyAttackEvent::" + playerType + " " + damage);
        }
    }
}