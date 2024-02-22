using Events;
using Managers;
using Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UI
{
    [UILayer(UIPanelLayer.Fixed)]
    public class UIPlayerInGamePanel : UIBase
    {
        [SerializeField]
        UIPlayerInGamePanel_PlayerStats playerStats;

        public override void OnUIEnable()
        {
            base.OnUIEnable();
            playerStats.UpdatePlayerImage(GameManager.Instance.LocalPlayer.playerSprite);
            //OnPlayerDataUpdate(null, GameManager.Instance.LocalPlayer.playerData.Value);
            EventManager.Instance.Subscribe<PlayerDataUpdateEvent>(OnPlayerDataUpdate);
        }

        public override void OnUIDisable()
        {
            EventManager.Instance.Unsubscribe<PlayerDataUpdateEvent>(OnPlayerDataUpdate);
            base.OnUIDisable();
        }

        public void OnPlayerDataUpdate(PlayerDataUpdateEvent e)
        {
            playerStats.UpdatePlayerStats(GameManager.Instance.LocalPlayer.playerData.Value.playerHealth, GameManager.Instance.LocalPlayer.playerData.Value.playerMaxHealth, GameManager.Instance.LocalPlayer.playerData.Value.playerEnergy, GameManager.Instance.LocalPlayer.playerData.Value.playerMaxEnergy);
        }

        //private void Update()
        //{
        //    playerStats.UpdatePlayerStats(GameManager.Instance.LocalPlayer.playerData.Value.playerHealth, GameManager.Instance.LocalPlayer.playerData.Value.playerMaxHealth, GameManager.Instance.LocalPlayer.playerData.Value.playerEnergy, GameManager.Instance.LocalPlayer.playerData.Value.playerMaxEnergy);
        //}
    }
}