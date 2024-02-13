using Events;
using Items;
using Manager;
using Players;
using Quest;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Managers
{

    public class GameManager : MonoBehaviour
    {
        public bool DevMode = true;
        // player
        [SerializeField]
        Player localPlayer;
        [SerializeField]
        string localPlayerName;
        public string LocalPlayerName { get => localPlayerName; set => localPlayerName = value; }
        public Player LocalPlayer { get => localPlayer; set => localPlayer = value; }

        private static GameManager instance;
        public static GameManager Instance { get => instance; }

        public float timeElapsed = 0f;
        public float timeScale = 1f;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                UIManager.Instance.DummyInit();
            }
        }

        private void Start()
        {
            if (DevMode)
            {
                UIManager.Instance.OpenPanel<UIDev>();
            }
            UIManager.Instance.OpenPanel<UIStartMenu>();
        }

        private void Update()
        {
            Time.timeScale = timeScale;
            timeElapsed = TimeManager.Instance.GetTime();
            // time manager and event manager does not affected to time scale
            TimeManager.Instance.Tick();
            EventManager.Instance.Tick();
        }

        public void SetTimeScale(float scale)
        {
            timeScale = scale;
        }

        public void StarGame()
        {
            NetConnector.Instance.acceptIncomingConnections = false;
            QuestManager.Instance.StartQuestSequenceServerRpc();
        }

        public void QuitGame()
        {
            // shutdown network
            NetConnector.Instance.ShutdownNetwork();
            // quit
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }

    }

}