using Events;
using Players;
using Quest;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Items
{
    public class ItemPedalSwitch : ItemBase
    {
        [SerializeField]
        GameObject emissionPlne;

        [SerializeField]
        ParticleSystem particles;

        [SerializeField]
        bool accessableByAll = false;

        bool isActive = false;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Debug.Log("ItemPedalSwitch OnNetworkSpawn");
            emissionPlne.SetActive(false);
            EventManager.Instance.Subscribe<TaskCompleteEvent>(OnTaskComplete);

            if (item_uid == 8 || item_uid == 22)
            {
                emissionPlne.SetActive(true);
                SetInteractableServerRpc(true);
            }
        }

        public override void OnNetworkDespawn()
        {
            EventManager.Instance.Unsubscribe<TaskCompleteEvent>(OnTaskComplete);
            base.OnNetworkDespawn();
        }

        Color planeColor;
        Color particleColor;

        private void OnTriggerStay(Collider other)
        {
            if (isActive)
            {
                return;
            }

            if (Interactable.Value)
            {
                if (accessableByAll || (other.TryGetComponent<Player>(out var p) && (itemDataItem.accessbility == ItemAccessbility.both || p.playerType == itemDataItem.accessbility)))
                {
                    isActive = true;

                    Debug.Log("ItemPedalSwitch OnTriggerStay: " + item_uid);
                    // set particle color to green
                    var main = particles.main;
                    particleColor = particles.main.startColor.color;
                    main.startColor = new Color(0, 1, 0, 1);
                    // set emission map to green
                    Material emissionMaterial = emissionPlne.GetComponent<MeshRenderer>().material;
                    planeColor = emissionMaterial.GetColor("_EmissionColor");
                    emissionMaterial.SetColor("_EmissionColor", new Color(0, 1, 0, 1));
                    // set emission plane material to green
                    emissionPlne.GetComponent<MeshRenderer>().material = emissionMaterial;

                    // hard coded for now
                    if (item_uid == 8 || item_uid == 22)
                    {
                        SetInteractableServerRpc(false);
                        if (item_uid == 8 && !GameObject.Find("Pedal_Switch_Maze_princess").GetComponent<ItemPedalSwitch>().Interactable.Value)
                        {
                            if (TryGetComponent<ItemInteractableModifier>(out var imf))
                            {
                                imf.SetInteractable(true);
                            }
                        }
                        else if (item_uid == 22 && !GameObject.Find("Pedal_Switch_Maze").GetComponent<ItemPedalSwitch>().Interactable.Value)
                        {
                            if (TryGetComponent<ItemInteractableModifier>(out var imf))
                            {
                                imf.SetInteractable(true);
                            }
                        }
                        return;
                    }

                    if (other.TryGetComponent(out Player player) && player.IsLocalPlayer)
                    {
                        if (TryGetComponent<QuestProgressModifier>(out var qpm))
                        {
                            qpm.AddProgress();
                        }
                        if (TryGetComponent<ItemInteractableModifier>(out var imf))
                        {
                            imf.SetInteractable(true);
                        }

                    }
                    else
                    {
                        if (TryGetComponent<ItemInteractableModifier>(out var imf))
                        {
                            imf.SetInteractable(true);
                        }
                    }
                }

            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (Interactable.Value)
            {
                bool allowTrigger = accessableByAll;
                if (!allowTrigger && other.TryGetComponent<Player>(out var p))
                {
                    allowTrigger = itemDataItem.accessbility == ItemAccessbility.both || p.playerType == itemDataItem.accessbility;
                }

                if (allowTrigger)
                {
                    isActive = false;
                    var main = particles.main;
                    main.startColor = particleColor;
                    Material emissionMaterial = emissionPlne.GetComponent<MeshRenderer>().material;
                    emissionMaterial.SetColor("_EmissionColor", planeColor);
                    if (TryGetComponent<QuestProgressModifier>(out var qpm))
                    {
                        qpm.DecreaseProgress();
                    }
                    if (TryGetComponent<ItemInteractableModifier>(out var imf))
                    {
                        imf.SetInteractable(false);
                    }
                }
            }

        }

        [ClientRpc]
        protected override void SetInteractableClientRpc(bool interactable)
        {
            Debug.Log("ItemPedalSwitch SetInteractableClientRpc: " + item_uid + " " + interactable);
            if (interactable)
            {
                emissionPlne.SetActive(true);
                particles.Play();
            }
            else
            {
                if(item_uid == 8 || item_uid == 22)
                {
                    var main = particles.main;
                    particleColor = particles.main.startColor.color;
                    main.startColor = new Color(0, 1, 0, 1);
                    // set emission map to green
                    Material emissionMaterial = emissionPlne.GetComponent<MeshRenderer>().material;
                    planeColor = emissionMaterial.GetColor("_EmissionColor");
                    emissionMaterial.SetColor("_EmissionColor", new Color(0, 1, 0, 1));
                    // set emission plane material to green
                    emissionPlne.GetComponent<MeshRenderer>().material = emissionMaterial;
                }
            }
        }

        private void OnTaskComplete(TaskCompleteEvent e)
        {
            // hard coded for now
            if (item_uid == 8 || item_uid == 22)
            {
                return;
            }
            if (e.taskDataItem.task_chain_id == 1 && e.taskDataItem.task_sub_id == 2)
            {
                SetInteractableServerRpc(false);                    // set particle color to green
                var main = particles.main;
                particleColor = particles.main.startColor.color;
                main.startColor = new Color(0, 1, 0, 1);
                // set emission map to green
                Material emissionMaterial = emissionPlne.GetComponent<MeshRenderer>().material;
                planeColor = emissionMaterial.GetColor("_EmissionColor");
                emissionMaterial.SetColor("_EmissionColor", new Color(0, 1, 0, 1));
            }
        }
    }
}