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
        public NetworkVariable<PlayerData> playerData = new();

        [SerializeField]
        private CinemachineVirtualCamera cinemachineVirtualCamera;

        [SerializeField]
        private TextMeshProUGUI playerNameText;

        public ItemAccessbility playerType;

        public Sprite playerSprite;


        public override void OnNetworkSpawn()
        {
            if (IsClient && IsOwner)
            {
                InitPlayerServerRpc(GameManager.Instance.LocalPlayerName);
                GameManager.Instance.LocalPlayer = this;
                cinemachineVirtualCamera.Priority = 10;
                inputReader.DisableAllInput();
                RegisterInputEvents();
                playerNameText.text = GameManager.Instance.LocalPlayerName;
                RegisterEvents();
                inputReader.DisableAllInput();
                StartCoroutine(EnablePlayerOps());
            }
            else
            {
                cinemachineVirtualCamera.Priority = 0;
                StartCoroutine(RefreshName());
            }
            base.OnNetworkSpawn();
        }

        IEnumerator RefreshName()
        {
            yield return new WaitForSeconds(1f);
            playerNameText.text = playerData.Value.PlayerName;
        }

        public override void OnNetworkDespawn()
        {
            if (IsClient && IsOwner)
            {
                UnregisterInputEvents();
                UnRegisterEvents();
            }
            base.OnNetworkDespawn();
        }

        private void RegisterEvents()
        {
            playerData.OnValueChanged += OnPlayerDataChanged;
            EventManager.Instance.Subscribe<EnemyAttackEvent>(OnEnemyAttack);
            EventManager.Instance.Subscribe<PlayerHealEvent>(OnPlayerHeal);
        }

        private void UnRegisterEvents()
        {
            playerData.OnValueChanged -= OnPlayerDataChanged;
            EventManager.Instance.Unsubscribe<EnemyAttackEvent>(OnEnemyAttack);
            EventManager.Instance.Unsubscribe<PlayerHealEvent>(OnPlayerHeal);
        }

        private void OnPlayerDataChanged(PlayerData oldValue, PlayerData newValue)
        {
            if (newValue.playerType != playerType)
            {
                return;
            }
            Debug.Log("OnPlayerDataChanged: " + newValue.playerType);
            playerNameText.text = newValue.PlayerName;
            new PlayerDataUpdateEvent();
        }

        IEnumerator EnablePlayerOps()
        {
            yield return new WaitForSeconds(2);
            inputReader.EnablePlayerInput();
            UIManager.Instance.OpenPanel<UIPlayerInGamePanel>();
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