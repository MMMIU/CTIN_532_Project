namespace TelemetryManagerExamples
{
    using USCG.Core.Telemetry;

    using UnityEngine;
    using Events;
    using System.Collections.Generic;
    using Utils;

    public class RecordMetrics : MonoBehaviour
    {
        // Keep a reference to the metrics we create.
        private MetricId _mazeTime = default;
        private MetricId _hanoiCount = default;
        private MetricId _wayPointLightUp = default;
        private MetricId _playerHurt = default;

        // tasks start time, key = task_chain_id, value = Pair<task_sub_id, start_time>
        Dictionary<int, List<Pair<int, float>>> taskStartTime = new();
        int hanoiCount = 0;

        private void Start()
        {
            // Create all metrics in Start().
            _mazeTime = TelemetryManager.instance.CreateSampledMetric<string>("mazeTimeMetric");
            _hanoiCount = TelemetryManager.instance.CreateSampledMetric<int>("hanoiCountMetric");
            _wayPointLightUp = TelemetryManager.instance.CreateSampledMetric<string>("wayPointLightUpMetric");
            _playerHurt = TelemetryManager.instance.CreateSampledMetric<string>("playerHurtMetric");
            EventManager.Instance.Subscribe<TaskAssignEvent>(OnTaskAssignEvent);
            EventManager.Instance.Subscribe<TaskCompleteEvent>(OnTaskCompleteEvent);
            EventManager.Instance.Subscribe<HanoiControlStartEvent>(OnHanoiControlStartEvent);
            EventManager.Instance.Subscribe<WayPointLightUpEvent>(OnWayPointLightUpEvent);
            EventManager.Instance.Subscribe<EnemyAttackEvent>(OnPlayerHurtEvent);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe<TaskAssignEvent>(OnTaskAssignEvent);
            EventManager.Instance.Unsubscribe<TaskCompleteEvent>(OnTaskCompleteEvent);
            EventManager.Instance.Unsubscribe<HanoiControlStartEvent>(OnHanoiControlStartEvent);
            EventManager.Instance.Unsubscribe<WayPointLightUpEvent>(OnWayPointLightUpEvent);
            EventManager.Instance.Unsubscribe<EnemyAttackEvent>(OnPlayerHurtEvent);
        }

        private void OnTaskAssignEvent(TaskAssignEvent e)
        {
            int task_chain_id = e.taskDataItem.task_chain_id;
            int task_sub_id = e.taskDataItem.task_sub_id;
            float startTime = Time.time;
            // check if the task_chain_id and task_sub_id exists in the dictionary
            if (!taskStartTime.ContainsKey(task_chain_id))
            {
                taskStartTime.Add(task_chain_id, new List<Pair<int, float>>());
                taskStartTime[task_chain_id].Add(new Pair<int, float>(task_sub_id, startTime));
            }
            else
            {
                // check if the task_sub_id exists in the list
                bool found = false;
                foreach (Pair<int, float> pair in taskStartTime[task_chain_id])
                {
                    if (pair.First == task_sub_id)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    taskStartTime[task_chain_id].Add(new Pair<int, float>(task_sub_id, startTime));
                }
            }

        }

        private void OnTaskCompleteEvent(TaskCompleteEvent e)
        {
            // Calculate the time taken to complete the task.
            int task_chain_id = e.taskDataItem.task_chain_id;
            int task_sub_id = e.taskDataItem.task_sub_id;
            float endTime = Time.time;
            float startTime = 0;
            foreach (Pair<int, float> pair in taskStartTime[task_chain_id])
            {
                if (pair.First == task_sub_id)
                {
                    startTime = pair.Second;
                    break;
                }
            }
            float timeTaken = endTime - startTime;
            if (timeTaken > 0)
            {
                TelemetryManager.instance.AddMetricSample(_mazeTime, task_chain_id + "-" + task_sub_id + ": " + timeTaken.ToString());
            }

            if (e.taskDataItem.task_chain_id == 5 && e.taskDataItem.task_sub_id == 1)
            {
                TelemetryManager.instance.AddMetricSample(_hanoiCount, hanoiCount);
            }

        }

        private void OnHanoiControlStartEvent(HanoiControlStartEvent e)
        {
            hanoiCount++;
        }

        private void OnWayPointLightUpEvent(WayPointLightUpEvent e)
        {
            TelemetryManager.instance.AddMetricSample(_wayPointLightUp, e.ptName + " " + e.lightUp);
        }

        private void OnPlayerHurtEvent(EnemyAttackEvent e)
        {
            TelemetryManager.instance.AddMetricSample(_playerHurt, Time.time + " " + e.playerType.ToString() + " " + e.damage.ToString());
        }
    }
}
