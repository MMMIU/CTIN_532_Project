using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class EnemyLogic
    {
        private EnemyData m_itemData;
        private static EnemyLogic instance;
        public static EnemyLogic Instance
        {
            get
            {
                if (null == instance)
                    instance = new EnemyLogic();
                return instance;
            }
        }

        public EnemyLogic()
        {
            m_itemData = new EnemyData();
            m_itemData.LoadAllEnemies();
        }

        public EnemyDataItem GetEnemyData(int itemUid)
        {
            return m_itemData.GetData(itemUid);
        }
    }
}