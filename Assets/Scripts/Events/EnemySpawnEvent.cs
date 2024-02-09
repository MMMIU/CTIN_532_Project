using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using Quest;
using Manager;
namespace Events {
    public class EnemySpawnEvent : BaseEvent
    {
        public int enemyId;
        public int spawnPlace;
        public EnemySpawnEvent(int enemyId = -1, int spawnPlace = -1, float delay = 0f, string name = "EnemySpawnEvent") : base(name, delay)
        {
            this.enemyId = enemyId;
            this.spawnPlace = spawnPlace;
        }
    }
}