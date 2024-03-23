using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using Quest;
using Manager;
using Items;
namespace Events {
    public class EnemySpawnEvent : EventBase
    {
        public ItemAccessbility playerType;
        public int enemyId;
        public int spawnPlace;
        public int spawnerID;
        public EnemySpawnEvent(ItemAccessbility playerType, int enemyId = -1, int spawnerID = -1, int spawnPlace = -1, float delay = 0f, string name = nameof(EnemySpawnEvent)) : base(name, delay)
        {
            this.playerType = playerType;
            this.enemyId = enemyId;
            this.spawnPlace = spawnPlace;
            this.spawnerID = spawnerID;
        }
    }
}