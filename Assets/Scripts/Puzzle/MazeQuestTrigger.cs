using Players;
using Quest;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Puzzle
{
    public class MazeQusetTrigger : NetworkBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("MazeQusetTrigger::OnTriggerEnter::" + other.tag);
            if (other.CompareTag("Player"))
            {
                Player p = other.GetComponent<Player>();
                if (p.IsLocalPlayer)
                {
                    if (TryGetComponent(out QuestProgressModifier questProgressModifier))
                    {
                        questProgressModifier.Assign();
                    }
                }
            }
        }
    }
}