using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemies;
using Quest;
using Players;
using Items;
namespace Events {
    public class PuzzleEnemyAttackEvent : EventBase
    {
        public ItemAccessbility playerType;
        public int damage;

        public PuzzleEnemyAttackEvent(ItemAccessbility playerType, string name = nameof(PuzzleEnemyAttackEvent), float delay = 0f) : base(name, delay)
        {
            this.playerType = playerType;
            Debug.Log("PuzzleEnemyAttackEvent::" + playerType);
        }
    }
}