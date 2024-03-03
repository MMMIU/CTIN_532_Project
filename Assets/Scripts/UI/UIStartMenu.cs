using Events;
using Inputs;
using Manager;
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

        public async void OnStartAsKnight()
        {
            bool result = await NetConnector.Instance.StartHost();
            if (result)
            {
                GameManager.Instance.LocalPlayerName = string.IsNullOrEmpty(playerNameInput.text) ? "Knight" : playerNameInput.text;
                Close();
            }
        }

        public async void OnStartAsPrincess()
        {
            bool result = await NetConnector.Instance.StartClient(joinCodeInputField.text);
            if (result)
            {
                GameManager.Instance.LocalPlayerName = string.IsNullOrEmpty(playerNameInput.text) ? "Princess" : playerNameInput.text;
                Close();
            }
        }

        public void SetUseInternet()
        {
            NetConnector.Instance.useInternet = useInternetToggle.isOn;
        }
    }
}
