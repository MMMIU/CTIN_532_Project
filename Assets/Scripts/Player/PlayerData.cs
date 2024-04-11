using Items;
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
        private ForceNetworkSerializeByMemcpy<FixedString32Bytes> playerName;
        public ItemAccessbility playerType;
        public float playerHealth;
        public float playerMaxHealth;
        public float playerEnergy;
        public float playerMaxEnergy;
        public bool playerDead;
        public bool playerUnstoppable;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref playerName);
            serializer.SerializeValue(ref playerType);
            serializer.SerializeValue(ref playerHealth);
            serializer.SerializeValue(ref playerMaxHealth);
            serializer.SerializeValue(ref playerEnergy);
            serializer.SerializeValue(ref playerMaxEnergy);
            serializer.SerializeValue(ref playerDead);
            serializer.SerializeValue(ref playerUnstoppable);
        }

        public string PlayerName { set { playerName = new FixedString32Bytes(value); } get { return playerName.Value.ToString(); } }
    }
}