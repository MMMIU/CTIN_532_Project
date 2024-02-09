using Quest;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Players
{
    [Serializable]
    public class PlayerData : INetworkSerializable
    {
        // dev: reset on awake
        public bool resetAwake = true;

        public FixedString32Bytes playerName;
        public ulong networkClientID;
        //public List<TaskDataItem> taskList;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref playerName);
            serializer.SerializeValue(ref networkClientID);
            //int length = 0;
            //if (serializer.IsReader)
            //{
            //    serializer.SerializeValue(ref length);
            //    taskList = new List<TaskDataItem>(length);
            //    for (int i = 0; i < length; i++)
            //    {
            //        TaskDataItem taskDataItem = new ();
            //        serializer.SerializeValue(ref taskDataItem);
            //        taskList.Add(taskDataItem);
            //    }
            //}
            //else
            //{
            //    length = taskList.Count;
            //    serializer.SerializeValue(ref length);
            //    for (int i = 0; i < length; i++)
            //    {
            //        taskList[i].NetworkSerialize(serializer);
            //    }
            //}
        }

        public void ResetAll(bool reset)
        {
            if (!reset) return;
            playerName = "Player";
            networkClientID = 10086;
            //taskList = new List<TaskDataItem>();
        }
    }
}