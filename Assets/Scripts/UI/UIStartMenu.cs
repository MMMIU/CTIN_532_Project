using Events;
using Inputs;
using Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [UIBlock]
    [UILayer(UIPanelLayer.Normal)]
    public class UIStartMenu : UIBase
    {
        [SerializeField]
        private InputReader inputReader;

        [SerializeField]
        private TMP_InputField playerNameInput;

        [SerializeField]
        private TMP_InputField joinCodeInputField;

        [SerializeField]
        private Toggle useInternetToggle;

        [SerializeField]
        TextMeshProUGUI joinCodeText;

        [SerializeField]
        GameObject joinCodePanel;

        public override void OnUIEnable()
        {
            base.OnUIEnable();
            NetConnector.Instance.useInternet = useInternetToggle.isOn;
            EventManager.Instance.Subscribe<GameStartEvent>(OnGameStart);
            EventManager.Instance.Subscribe<JoinCodeAssignEvent>(OnJoinCode);
        }

        public override void OnUIDisable()
        {
            EventManager.Instance.Unsubscribe<GameStartEvent>(OnGameStart);
            EventManager.Instance.Unsubscribe<JoinCodeAssignEvent>(OnJoinCode);
            base.OnUIDisable();
        }

        private void OnJoinCode(JoinCodeAssignEvent e)
        {
            joinCodePanel.SetActive(true);
            joinCodeText.text = e.joinCode;
        }

        private void OnGameStart(GameStartEvent e)
        {
            UIManager.Instance.Destroy(this);
        }

        public async void OnStartAsKnight()
        {
            bool result = await NetConnector.Instance.StartHost();
            if (result)
            {
                GameManager.Instance.LocalPlayerName = string.IsNullOrEmpty(playerNameInput.text) ? "Knight" : playerNameInput.text;
            }
        }

        public async void OnStartAsPrincess()
        {
            bool result = await NetConnector.Instance.StartClient(joinCodeInputField.text);
            if (result)
            {
                GameManager.Instance.LocalPlayerName = string.IsNullOrEmpty(playerNameInput.text) ? "Princess" : playerNameInput.text;
            }
        }

        public void SetUseInternet()
        {
            NetConnector.Instance.useInternet = useInternetToggle.isOn;
        }
    }
}
