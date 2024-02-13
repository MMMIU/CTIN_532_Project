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
using TMPro;
using Unity.Collections;
using Events;

namespace Players
{
    public partial class Player : NetworkBehaviour
    {
        [SerializeField]
        private InputReader inputReader;
        public NetworkVariable<PlayerData> playerData = new(new PlayerData());

        [SerializeField]
        private CinemachineVirtualCamera cinemachineVirtualCamera;

        [SerializeField]
        private TextMeshProUGUI playerNameText;

        public ItemAccessbility playerType;

        public Sprite playerSprite;


        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsClient && IsOwner)
            {
                InitPlayerServerRpc(GameManager.Instance.LocalPlayerName);
                GameManager.Instance.LocalPlayer = this;
                cinemachineVirtualCamera.Priority = 10;
                inputReader.DisableAllInput();
                RegisterInputEvents();
                StartCoroutine(EnablePlayerInput());
                playerNameText.text = GameManager.Instance.LocalPlayerName;
            }
            else
            {
                cinemachineVirtualCamera.Priority = 0;
                playerNameText.text = playerData.Value.PlayerName;
            }
        }

        IEnumerator EnablePlayerInput()
        {
            yield return new WaitForSeconds(2);
            inputReader.EnablePlayerInput();
            UIManager.Instance.OpenPanel<UIPlayerInGamePanel>();
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
            inputReader.OpenQuestPanelEvent += ShowQuestPanel;
            inputReader.SpecialSkillOneEvent += UseSpecialSkillOne;
            inputReader.EnablePlayerInput();
        }

        private void UnregisterInputEvents()
        {
            inputReader.OpenQuestPanelEvent -= ShowQuestPanel;
            inputReader.SpecialSkillOneEvent -= UseSpecialSkillOne;
            inputReader.DisableAllInput();
        }

        private void ShowQuestPanel()
        {
            UIManager.Instance.OpenPanel<UIQuestPanel>();
        }

        private void UseSpecialSkillOne()
        {
            Debug.Log("UseSpecialSkill");
            if (playerType == ItemAccessbility.princess)
            {
                PrincessSkillOne();
            }
        }
    }
}