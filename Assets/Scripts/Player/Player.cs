using Cinemachine;
using Manager;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

using Inputs;
using UI;
using Quest;
using Managers;
using Items;

namespace Players
{
    public class Player : NetworkBehaviour
    {
        [SerializeField]
        private InputReader inputReader;

        public NetworkVariable<PlayerData> playerData;

        [SerializeField]
        private CinemachineVirtualCamera cinemachineVirtualCamera;

        public ItemAccessbility playerType;


        public override void OnNetworkSpawn()
        {
            if (IsLocalPlayer)
            {
                GameManager.Instance.LocalPlayer = this;
                cinemachineVirtualCamera.Priority = 10;
                RegisterInputEvents();
            }
            else
            {
                cinemachineVirtualCamera.Priority = 0;
            }
            base.OnNetworkSpawn();
        }

        public override void OnNetworkDespawn()
        {
            if (IsClient && IsOwner)
            {
                UnregisterInputEvents();
            }
            base.OnNetworkDespawn();
        }

        private void RegisterInputEvents()
        {
            inputReader.EscEvent += ShowPausePanel;
            inputReader.OpenQuestPanelEvent += ShowQuestPanel;
            inputReader.EnablePlayerInput();
        }

        private void UnregisterInputEvents()
        {
            inputReader.EscEvent -= ShowPausePanel;
            inputReader.OpenQuestPanelEvent -= ShowQuestPanel;
            inputReader.DisableAllInput();
        }

        private void ShowPausePanel()
        {
            UIManager.Instance.OpenPanel<UIPauseMenu>();
        }

        private void ShowQuestPanel()
        {
            UIManager.Instance.OpenPanel<UIQuestPanel>();
        }
    }
}