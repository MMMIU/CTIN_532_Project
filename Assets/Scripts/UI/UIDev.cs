using Events;
using Inputs;
using Items;
using Manager;
using Managers;
using Quest;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [UILayer(UIPanelLayer.PopUp)]
    public class UIDev : UIBase
    {
        [SerializeField]
        private InputReader inputReader;

        [SerializeField]
        TMPro.TMP_InputField joinCodeInputField;
        [SerializeField]
        TMPro.TextMeshProUGUI joinCodeText;
        [SerializeField]
        TMPro.TextMeshProUGUI rttText;
        [SerializeField]
        TMPro.TMP_InputField chainInputField;
        [SerializeField]
        TMPro.TMP_InputField subInputField;
        [SerializeField]
        Toggle useInternetToggle;

        [SerializeField]
        private GameObject closablePanel;

        private CursorLockMode previousLockMode;
        private bool previousCursorVisible;

        private void FixedUpdate()
        {
            if (NetworkManager.Singleton.IsClient && NetworkManager.Singleton.IsConnectedClient)
            {
                rttText.text = "RTT: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetCurrentRtt(NetworkManager.Singleton.NetworkConfig.NetworkTransport.ServerClientId).ToString();
            }
        }


        public override void OnUIEnable()
        {
            closablePanel.SetActive(false);
            NetConnector.Instance.useInternet = useInternetToggle.isOn;
            EventManager.Instance.Subscribe(nameof(JoinCodeAssignEvent), OnJoinCodeAssign);
            inputReader.OpenDevPanelEvent += ShowHideFloat;
            inputReader.CloseDevPanelEvent += ShowHideFloat;
        }

        public override void OnUIDisable()
        {
            inputReader.OpenDevPanelEvent -= ShowHideFloat;
            inputReader.CloseDevPanelEvent -= ShowHideFloat;
            EventManager.Instance.Unsubscribe(nameof(JoinCodeAssignEvent), OnJoinCodeAssign);
        }


        private void OnJoinCodeAssign(EventBase e)
        {
            JoinCodeAssignEvent realEvent = e as JoinCodeAssignEvent;
            joinCodeText.text = realEvent.joinCode;
        }

        public async void OnStartClientBtnClick()
        {
            await NetConnector.Instance.StartClient(joinCodeInputField.text);
            ShowHideFloat();
        }

        public async void OnStartHostBtnClick()
        {
            await NetConnector.Instance.StartHost();
            ShowHideFloat();
        }

        public void OnShutdownNetworkBtnClick()
        {
            NetConnector.Instance.ShutdownNetwork();
        }

        private void ShowHideFloat()
        {
            closablePanel.SetActive(!closablePanel.activeSelf);
            if (closablePanel.activeSelf)
            {
                previousLockMode = Cursor.lockState;
                previousCursorVisible = Cursor.visible;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                inputReader.EnableUIInput();
            }
            else
            {
                Cursor.lockState = previousLockMode;
                Cursor.visible = previousCursorVisible;
                if (UIManager.Instance.blockPanelStack.Count == 0)
                {
                    inputReader.EnablePlayerInput();
                }
            }
        }

        public void AssignTask()
        {
            int chain = chainInputField.text == "" ? 0 : int.Parse(chainInputField.text);
            int sub = subInputField.text == "" ? 0 : int.Parse(subInputField.text);
            Debug.Log("AssignTask " + chain + "-" + sub);
            QuestManager.Instance.AssignTaskServerRpc(chain, sub);
        }

        public void StartTaskAssign()
        {
            Debug.Log("StartTaskAssign");
            QuestManager.Instance.StartQuestSequenceServerRpc();
        }

        public void AddProgress()
        {
            int chain = chainInputField.text == "" ? 0 : int.Parse(chainInputField.text);
            int sub = subInputField.text == "" ? 0 : int.Parse(subInputField.text);
            Debug.Log("AddProgress to " + chain + "-" + sub);
            QuestManager.Instance.AddProgressServerRpc(chain, sub, 1);
        }

        public void CompleteTask()
        {
            int chain = chainInputField.text == "" ? 0 : int.Parse(chainInputField.text);
            int sub = subInputField.text == "" ? 0 : int.Parse(subInputField.text);
            Debug.Log("CompleteTask " + chain + "-" + sub);
            QuestManager.Instance.GetAwardServerRpc(chain, sub);
        }

        public void GetAllTasks()
        {
            QuestManager.Instance.GetAllTasks();
        }

        public void RemoveAllTasks()
        {
            QuestManager.Instance.ClearAllTasksServerRpc();
        }

        public void QuitGame()
        {
            GameManager.Instance.QuitGame();
        }

        public void SetUseInternet()
        {
            NetConnector.Instance.useInternet = useInternetToggle.isOn;
        }

        public void HurtLocalPlayer()
        {
            GameManager.Instance.LocalPlayer.PlayerTakeDamageServerRpc(10);
        }

        public void HealLocalPlayer()
        {
            GameManager.Instance.LocalPlayer.PlayerHealServerRpc(10);
        }

        public void RespawnLocalPlayer()
        {
            GameManager.Instance.LocalPlayer.PlayerRespawnServerRpc();
        }

        public void SendPuzzleKngightSpawnEvent()
        {
            new PuzzleEnemyAttackEvent(ItemAccessbility.knight);
        }

        public void SendPuzzlePrincessSpawnEvent()
        {
            new PuzzleEnemyAttackEvent(ItemAccessbility.princess);
        }
    }

}
