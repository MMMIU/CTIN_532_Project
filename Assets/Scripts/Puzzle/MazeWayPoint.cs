using Events;
using Inputs;
using Items;
using Players;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Puzzle
{
    public class MazeWayPoint : NetworkBehaviour
    {
        [SerializeField]
        InputReader inputReader;

        [SerializeField]
        GameObject wayPointOn;

        [SerializeField]
        GameObject canvasObj;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            wayPointOn.SetActive(false);
            canvasObj.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && other.TryGetComponent(out Player p) && p.IsLocalPlayer)
            {
                canvasObj.SetActive(true);
                inputReader.InteractionEvent += OnInteract;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && other.TryGetComponent(out Player p) && p.IsLocalPlayer)
            {
                canvasObj.SetActive(false);
                inputReader.InteractionEvent -= OnInteract;
            }
        }

        private void OnInteract()
        {
            if(!wayPointOn.activeSelf)
            {
                SFXManager.Instance.PlaySFX("interaction");
            }
            SetWayPointServerRpc(!wayPointOn.activeSelf);
            new WayPointLightUpEvent(name, !wayPointOn.activeSelf);
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetWayPointServerRpc(bool active)
        {
            SetWayPointClientRpc(active);
        }

        [ClientRpc]
        public void SetWayPointClientRpc(bool active)
        {
            wayPointOn.SetActive(active);
        }
    }
}