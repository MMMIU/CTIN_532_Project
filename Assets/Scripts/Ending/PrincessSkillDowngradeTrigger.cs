using Events;
using Players;
using Quest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrincessSkillDowngradeTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out Player player) && player.playerType == Items.ItemAccessbility.princess)
        {
            Debug.Log("PrincessSkillDowngradeTrigger");
            new PrincessSkillDowngradeEvent();
            if (player.IsLocalPlayer)
            {
                if (TryGetComponent(out QuestProgressModifier questProgressModifier))
                {
                    questProgressModifier.AddProgress();
                }
            }
        }
    }
}
