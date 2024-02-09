using Inputs;
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

        private void Start()
        {
            closablePanel.SetActive(false);
            NetConnector.Instance.useInternet = useInternetToggle.isOn;
        }

        public override void OnUIEnable()
        {
            inputReader.OpenDevPanelEvent += ShowHideFloat;
            inputReader.CloseDevPanelEvent += ShowHideFloat;
        }

        public override void OnUIDisable()
        {
            inputReader.OpenDevPanelEvent -= ShowHideFloat;
            inputReader.CloseDevPanelEvent -= ShowHideFloat;
        }

        public async void OnStartClientBtnClick()
        {
            bool result = await NetConnector.Instance.StartClient(joinCodeInputField.text);
            joinCodeText.text = result ? "Client started" : "Client failed to start";
            ShowHideFloat();
        }

        public async void OnStartHostBtnClick()
        {
            string result = await NetConnector.Instance.StartHost();
            joinCodeText.text = "Code: " + result;
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
                if(UIManager.Instance.blockPanelStack.Count == 0)
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

    }

}
