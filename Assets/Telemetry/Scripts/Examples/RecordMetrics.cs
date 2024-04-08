namespace TelemetryManagerExamples
{
    using USCG.Core.Telemetry;

    using UnityEngine;
    using Events;

    public class RecordMetrics : MonoBehaviour
    {
        // Keep a reference to the metrics we create.
        private MetricId _mazeTime = default;
        private MetricId _hanoiCount = default;

        float mazeStartTime = 0;
        int hanoiCount = 0;

        private void Start()
        {
            // Create all metrics in Start().
            _mazeTime = TelemetryManager.instance.CreateSampledMetric<float>("mazeTimeMetric");
            _hanoiCount = TelemetryManager.instance.CreateSampledMetric<int>("hanoiCountMetric");
            EventManager.Instance.Subscribe<TaskAssignEvent>(OnTaskAssignEvent);
            EventManager.Instance.Subscribe<TaskCompleteEvent>(OnTaskCompleteEvent);
            EventManager.Instance.Subscribe<HanoiControlStartEvent>(OnHanoiControlStartEvent);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe<TaskAssignEvent>(OnTaskAssignEvent);
            EventManager.Instance.Unsubscribe<TaskCompleteEvent>(OnTaskCompleteEvent);
        }

        private void OnTaskAssignEvent(TaskAssignEvent e)
        {
            if (e.taskDataItem.task_chain_id == 4 && e.taskDataItem.task_sub_id == 1)
            {
                mazeStartTime = Time.time;
            }
        }

        private void OnTaskCompleteEvent(TaskCompleteEvent e)
        {
            if (e.taskDataItem.task_chain_id == 4 && e.taskDataItem.task_sub_id == 1)
            {
                float time = Time.time - mazeStartTime;
                TelemetryManager.instance.AddMetricSample(_mazeTime, time);
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
    }
}
