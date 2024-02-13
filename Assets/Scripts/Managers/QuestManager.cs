using Events;
using Items;
using LitJson;
using Players;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Quest
{
    public class QuestManager : NetworkBehaviour
    {
        private static QuestManager instance;
        public static QuestManager Instance
        {
            get => instance;
        }

        public override void OnNetworkSpawn()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private TaskData m_taskData = new TaskData();

        public TaskData TaskData
        {
            get => m_taskData;
        }

        [ServerRpc(RequireOwnership = false)]
        public void StartQuestSequenceServerRpc()
        {
            Debug.Log("QuestManager.StartQuestSequenceServerRpc()");
            AssignTaskServerRpc(1, 1);
        }

        public void GetAllTasks()
        {
            foreach (var item in m_taskData.taskDatas)
            {
                Debug.Log("Task: " + item.task_chain_id + " " + item.task_sub_id + " " + item.progress);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void AssignTaskServerRpc(int chainId, int subId, int delayRound = 0)
        {
            Debug.Log("QuestManager.AssignTask(" + chainId + ", " + subId + ")");
            var newCfg = TaskCfg.Instance.GetCfgItem(chainId, subId);
            if (newCfg != null)
            {
                TaskDataItem dataItem = new();
                dataItem.task_chain_id = newCfg.task_chain_id;
                dataItem.task_sub_id = newCfg.task_sub_id;
                dataItem.progress = 0;
                dataItem.completed = 0;
                DoStartActionClientRpc(newCfg.start_action.ToString());

                // add or update task data
                AddOrUpdateTaskDataServerRpc(dataItem, delayRound);
            }
        }

        [ServerRpc]
        public void AddOrUpdateTaskDataServerRpc(TaskDataItem taskDataItem, int delayRond = 0)
        {
            Debug.Log("QuestManager.AddOrUpdateTaskDataServerRpc(" + taskDataItem.task_chain_id + ", " + taskDataItem.task_sub_id + ")");
            if (taskDataItem != null)
            {
                m_taskData.AddOrUpdateData(taskDataItem, delayRond);
                AddOrUpdateTaskDataClientRpc(taskDataItem, delayRond);
            }
        }

        [ClientRpc]
        private void AddOrUpdateTaskDataClientRpc(TaskDataItem taskDataItem, int delayRound)
        {
            Debug.Log("QuestManager.AddOrUpdateTaskDataClientRpc(" + taskDataItem.task_chain_id + ", " + taskDataItem.task_sub_id + ")");
            if (taskDataItem != null)
            {
                m_taskData.AddOrUpdateData(taskDataItem, delayRound);
            }
        }

        [ServerRpc]
        public void RemoveTaskDataServerRpc(int chainId, int subId)
        {
            Debug.Log("QuestManager.RemoveTaskDataServerRpc(" + chainId + ", " + subId + ")");
            var data = m_taskData.GetData(chainId, subId);
            if (data != null)
            {
                m_taskData.RemoveData(chainId, subId);
                RemoveTaskDataClientRpc(data);
            }
        }

        [ClientRpc]
        private void RemoveTaskDataClientRpc(TaskDataItem taskDataItem)
        {
            Debug.Log("QuestManager.RemoveTaskDataClientRpc(" + taskDataItem.task_chain_id + ", " + taskDataItem.task_sub_id + ")");
            if (taskDataItem != null)
            {
                m_taskData.RemoveData(taskDataItem.task_chain_id, taskDataItem.task_sub_id);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chainId"></param>
        /// <param name="subId"></param>
        /// <param name="deltaProgress"></param>
        /// <returns>1 = task complete; 0 = task not complete; -1 = error</returns>
        [ServerRpc(RequireOwnership = false)]
        public void AddProgressServerRpc(int chainId, int subId, int deltaProgress)
        {
            Debug.Log("QuestManager.AddProgress(" + chainId + ", " + subId + ", " + deltaProgress + ")");
            var data = m_taskData.GetData(chainId, subId);
            if (data == null)
            {
                Debug.LogWarning("TaskData.GetData(" + chainId + ", " + subId + ") is not on list.");
                return;
            }
            data.progress += deltaProgress;

            // add or update task data
            AddOrUpdateTaskDataServerRpc(data);

            var cfg = TaskCfg.Instance.GetCfgItem(data.task_chain_id, data.task_sub_id);
            if (cfg != null)
            {
                Debug.Log("Task progress: " + data.task_chain_id + " " + data.task_sub_id + " " + data.progress + "/" + cfg.target_amount);
                if (data.progress >= cfg.target_amount)
                {
                    Debug.Log("Task complete: " + data.task_chain_id + " " + data.task_sub_id);
                    // if data has no award, process to completion
                    if (!data.HasAward)
                        GetAwardServerRpc(data.task_chain_id, data.task_sub_id);
                }
            }
            else
            {
                Debug.LogError("TaskCfg.instance.GetCfgItem(" + data.task_chain_id + ", " + data.task_sub_id + ") is null");
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void DecreaseProgressServerRpc(int chainId, int subId, int deltaProgress)
        {
            Debug.Log("QuestManager.DecreaseProgress(" + chainId + ", " + subId + ", " + deltaProgress + ")");
            var data = m_taskData.GetData(chainId, subId);
            if (data == null)
            {
                Debug.LogWarning("TaskData.GetData(" + chainId + ", " + subId + ") is not on list.");
                return;
            }
            data.progress -= deltaProgress;
            if (data.progress < 0)
            {
                data.progress = 0;
            }
            // add or update task data
            AddOrUpdateTaskDataServerRpc(data);
        }

        [ServerRpc(RequireOwnership = false)]
        public void GetAwardServerRpc(int chainId, int subId)
        {
            Debug.Log("QuestManager.GetAward(" + chainId + ", " + subId + ")");
            var data = m_taskData.GetData(chainId, subId);
            if (data == null)
            {
                Debug.LogError("TaskData.GetData(" + chainId + ", " + subId + ") is null");
            }
            if (data.completed != 0)
            {
                Debug.LogError("award_is_get is not 0");
            }
            data.completed = 1;
            AddOrUpdateTaskDataServerRpc(data);
            GoNextServerRpc(chainId, subId);
            GetAwardClientRpc(data.task_chain_id, data.task_sub_id);
        }

        [ClientRpc]
        public void GetAwardClientRpc(int chain, int sub)
        {
            Debug.Log("QuestManager.GetAwardClientRpc(" + chain + ", " + sub + ")");
            var cfg = TaskCfg.Instance.GetCfgItem(chain, sub);
            if (cfg == null)
            {
                Debug.LogError("TaskCfg.instance.GetCfgItem(" + chain + ", " + sub + ") is null");
            }
            else
            {
                DoEndActionClientRpc(cfg.end_action);
                new TaskCompleteEvent(cfg);
            }
        }

        [ClientRpc]
        private void DoStartActionClientRpc(string start_action_json)
        {
            if (string.IsNullOrEmpty(start_action_json))
            {
                return;
            }
            var startAction = JsonMapper.ToObject(start_action_json);
            if (startAction.IsArray)
            {
                for (int i = 0, len = startAction.Count; i < len; ++i)
                {
                    var action = startAction[i];
                    if (action.IsObject)
                    {
                        var actionType = action["type"].ToString();
                        if (actionType == "item")
                        {
                            var item_uid = int.Parse(action["uid"].ToString());
                            var interactable = bool.Parse(action["interactable"].ToString());
                            new ItemSetInteractableEvent(item_uid, interactable);
                        }
                    }
                }
            }

        }

        [ClientRpc]
        private void DoEndActionClientRpc(string end_action_json)
        {
            if (string.IsNullOrEmpty(end_action_json))
            {
                return;
            }
            var endAction = JsonMapper.ToObject(end_action_json);
            if (endAction.IsArray)
            {
                for (int i = 0, len = endAction.Count; i < len; ++i)
                {
                    var action = endAction[i];
                    if (action.IsObject)
                    {
                        var actionType = action["type"].ToString();
                        if (actionType == "item")
                        {
                            var item_uid = int.Parse(action["uid"].ToString());
                            var interactable = bool.Parse(action["interactable"].ToString());
                            new ItemSetInteractableEvent(item_uid, interactable);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// trigger next task and chain task
        /// </summary>
        /// <param name="chainId">chain id</param>
        /// <param name="subId">task sub id</param>
        [ServerRpc]
        private void GoNextServerRpc(int chainId, int subId)
        {
            var data = m_taskData.GetData(chainId, subId);
            var cfg = TaskCfg.Instance.GetCfgItem(data.task_chain_id, data.task_sub_id);

            if (data.completed == 1)
            {
                // remove tasks that are already completed
                RemoveTaskDataServerRpc(data.task_chain_id, data.task_sub_id);

                // assign next task
                AssignTaskServerRpc(data.task_chain_id, data.task_sub_id + 1, 1);

                // assign new chain task
                string openChain = cfg.open_chain.ToString();
                if (!string.IsNullOrEmpty(openChain) && openChain != "/")
                {
                    Debug.Log("Assigning new chain task: " + cfg.open_chain);
                    // open_chain: "1|1,2|1,3|1"
                    var chains = cfg.open_chain.ToString().Split(',');
                    for (int i = 0, len = chains.Length; i < len; ++i)
                    {
                        var task = chains[i].Split('|');
                        int chain = int.Parse(task[0]);
                        int sub = int.Parse(task[1]);
                        AssignTaskServerRpc(chain, sub, 1);
                    }
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void ClearAllTasksServerRpc()
        {
            m_taskData.taskDatas.Clear();
            ClearAllTasksClientRpc();
        }

        [ClientRpc]
        private void ClearAllTasksClientRpc()
        {
            m_taskData.taskDatas.Clear();
        }
    }
}