using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System;
using Unity.Netcode;
using Players;
using Managers;
using System.Linq;
using Unity.Collections;
using Events;
using System.Security.Claims;

namespace Quest
{
    /// <summary>
    /// ��������
    /// </summary>
    [Serializable]
    public class TaskDataItem : INetworkSerializable
    {
        // chain id
        public int task_chain_id;
        // task sub id
        public int task_sub_id;
        // progress
        public int progress;
        // 0 = not get, 1 = get
        public int completed;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref task_chain_id);
            serializer.SerializeValue(ref task_sub_id);
            serializer.SerializeValue(ref progress);
            serializer.SerializeValue(ref completed);
        }

        public bool HasAward
        {
            get
            {
                var cfg = TaskCfg.Instance.GetCfgItem(task_chain_id, task_sub_id);
                return !string.IsNullOrEmpty(cfg.award) && cfg.award != "/";
            }
        }
    }

    [Serializable]
    public class TaskData
    {
        private List<TaskDataItem> m_taskDatas;
        public List<TaskDataItem> taskDatas
        {
            get { return m_taskDatas; }
        }

        public TaskData()
        {
            m_taskDatas = new List<TaskDataItem>();
        }

        /// <summary>
        /// add or update task data
        /// </summary>
        public void AddOrUpdateData(TaskDataItem itemData, int delayRound)
        {
            bool isUpdate = false;
            for (int i = 0, cnt = m_taskDatas.Count; i < cnt; ++i)
            {
                var item = m_taskDatas[i];
                if (itemData.task_chain_id == item.task_chain_id && itemData.task_sub_id == item.task_sub_id)
                {
                    // not new data
                    m_taskDatas[i] = itemData;
                    isUpdate = true;
                    break;
                }
            }
            // if new data
            if (!isUpdate)
            {
                Debug.Log("AddOrUpdateData " + itemData.task_chain_id + " " + itemData.task_sub_id);
                m_taskDatas.Add(itemData);
                var cfg = TaskCfg.Instance.GetCfgItem(itemData.task_chain_id, itemData.task_sub_id);
                new TaskAssignEvent(cfg, delayRound);
            }
            // sort, ensure main chain is first
            m_taskDatas.Sort((a, b) =>
            {
                return a.task_chain_id.CompareTo(b.task_chain_id);
            });
        }

        /// <summary>
        /// Get task data
        /// </summary>
        /// <param name="chainId">chain id</param>
        /// <param name="subId">task sub id</param>
        /// <returns></returns>
        public TaskDataItem GetData(int chainId, int subId)
            => m_taskDatas.FirstOrDefault(item => item.task_chain_id == chainId && item.task_sub_id == subId);


        /// <summary>
        /// remove task data
        /// </summary>
        /// <param name="chainId">chain id</param>
        /// <param name="subId">task sub id</param>
        public void RemoveData(int chainId, int subId)
        {
            for (int i = 0, cnt = m_taskDatas.Count; i < cnt; ++i)
            {
                var item = m_taskDatas[i];
                if (chainId == item.task_chain_id && subId == item.task_sub_id)
                {
                    m_taskDatas.Remove(item);
                    return;
                }
            }
        }

        public void ResetData()
        {
            m_taskDatas.Clear();
        }
    }
}