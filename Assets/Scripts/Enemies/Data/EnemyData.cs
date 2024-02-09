using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
namespace Enemies
{
    [Serializable]
    public class EnemyDataItem: INetworkSerializable
    {
        public int enemy_uid;
        public EnemyType enemy_type;
        public int enemy_sub_id;
        public FixedString32Bytes name;
        public FixedString128Bytes desc;
        public int health;
        public FixedString512Bytes award;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref enemy_uid);
            serializer.SerializeValue(ref enemy_type);
            serializer.SerializeValue(ref enemy_sub_id);
            serializer.SerializeValue(ref name);
            serializer.SerializeValue(ref desc);
            serializer.SerializeValue(ref health);
            serializer.SerializeValue(ref award);
        }
    }

    public class EnemyData
    {
        private List<EnemyDataItem> m_enemyDatas;

        public EnemyData()
        {
            m_enemyDatas = new List<EnemyDataItem>();
        }

        public EnemyDataItem GetData(int enemyUid)
        {
            foreach (var enemyData in m_enemyDatas)
            {
                if (enemyData.enemy_uid == enemyUid)
                {
                    return enemyData;
                }
            }
            return null;
        }

        public void LoadAllEnemies()
        {
            m_enemyDatas.Clear();
            EnemyCfg.Instance.LoadCfg();
            foreach (var enemyCfgItem in EnemyCfg.Instance.configs.Values)
            {
                EnemyDataItem enemyDataItem = new EnemyDataItem
                {
                    enemy_uid = enemyCfgItem.enemy_uid,
                    enemy_type = (EnemyType)Enum.Parse(typeof(EnemyType), enemyCfgItem.enemy_type),
                    enemy_sub_id = enemyCfgItem.enemy_sub_id,
                    name = enemyCfgItem.name,
                    desc = enemyCfgItem.desc,
                    health = enemyCfgItem.health,
                    award = enemyCfgItem.award
                };
                m_enemyDatas.Add(enemyDataItem);
            }
        }
    }
}
