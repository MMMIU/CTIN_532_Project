using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Quest
{
    public class QuestProgressModifier : MonoBehaviour
    {
        // quest pairs
        [SerializeField]
        List<Pair<int, int>> quests;

        public void AddProgress(int amout = 1)
        {
            foreach(var quest in quests)
            {
                QuestManager.Instance.AddProgressServerRpc(quest.First, quest.Second, amout);
            }
        }

        public void DecreaseProgress(int amout = 1)
        {
            foreach (var quest in quests)
            {
                QuestManager.Instance.DecreaseProgressServerRpc(quest.First, quest.Second, amout);
            }
        }

        public void Assign()
        {
            foreach (var quest in quests)
            {
                QuestManager.Instance.AssignTaskServerRpc(quest.First, quest.Second);
            }
        }

    }
}