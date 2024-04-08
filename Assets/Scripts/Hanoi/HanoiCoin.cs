using Events;
using Players;
using Quest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanoi
{
    public class HanoiCoin : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && other.TryGetComponent(out Player player) )
            {
                new PrincessSkillUpgradeEvent();
                if(player.IsLocalPlayer)
                {
                    if (TryGetComponent(out QuestProgressModifier questProgressModifier))
                    {
                        questProgressModifier.AddProgress();
                    }
                    gameObject.SetActive(false);
                }
            }
        }
    }
}