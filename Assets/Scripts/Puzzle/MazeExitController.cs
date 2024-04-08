using Items;
using Players;
using Quest;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Puzzle
{
    public class MazeExitController : NetworkBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if(other.TryGetComponent(out Player player) && player.IsLocalPlayer)
                {
                    if (TryGetComponent(out QuestProgressModifier questProgressModifier))
                    {
                        questProgressModifier.AddProgress();
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (other.TryGetComponent(out Player player) && player.IsLocalPlayer)
                {
                    if (TryGetComponent(out QuestProgressModifier questProgressModifier))
                    {
                        questProgressModifier.DecreaseProgress();
                    }
                }
            }
        }

    }
}