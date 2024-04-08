using Events;
using Inputs;
using Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;

namespace Hanoi
{
    public class HanoiGameController : MonoBehaviour
    {
        public bool isStarted = false;
        [SerializeField]
        LayerMask playerLayer;
        [SerializeField]
        [Tooltip("Disks in size order, higher index, larger disk")]
        HanoiDisk[] disks;
        [SerializeField]
        LayerMask diskLayer;
        [SerializeField]
        HanoiDisk freeDisk;

        [SerializeField]
        HanoiTower startTower;
        [SerializeField]
        HanoiTower middleTower;
        [SerializeField]
        HanoiTower endTower;

        [SerializeField]
        InputReader inputReader;

        [SerializeField]
        float diskCameraDistance;
        [SerializeField]
        float diskCameraDistanceModifier = 1f;
        [SerializeField]
        float diskCameraDistanceMax = 2f;
        [SerializeField]
        float diskCameraDistanceMin = 0.5f;
        [SerializeField]
        float diskCameraDistanceSpeed = 0.1f;

        [SerializeField]
        float totalTime = 60f;
        [SerializeField]
        TextMeshProUGUI timerText;
        float startTime;
        [SerializeField]
        int totalMoves = 20;
        [SerializeField]
        TextMeshProUGUI moveLeftText;
        public int movesLeft;

        //[SerializeField]
        //GameObject winPanelGO;
        //[SerializeField]
        //GameObject losePanelGO;

        HanoiDisk selectedDisk;

        [SerializeField]
        Collider gameAreaCollider;

        private void OnTriggerExit(Collider other)
        {
            // if player is out of game area, game over
            if (other.CompareTag("Player"))
            {
                GameOver();
            }
        }

        private void Start()
        {
            EventManager.Instance.Subscribe<HanoiControlStartEvent>(OnHanoiControlStartEvent);
        }

        private void OnHanoiControlStartEvent(HanoiControlStartEvent e)
        {
            UnregisterEvents();
            StartGame();
        }

        public void StartGame()
        {
            //winPanelGO.SetActive(false);
            //losePanelGO.SetActive(false);
            for (int i = 0; i < disks.Length; i++)
            {
                disks[i].transform.position = new Vector3(startTower.transform.position.x, startTower.transform.position.y + 0.1f + 0.5f * i, startTower.transform.position.z);
                startTower.Push(disks[i]);
                disks[i].Movable = false;
                disks[i].SetCurrentTower(startTower);
                disks[i].SetTargetTower(null);
            }
            disks[^1].Movable = true;
            inputReader.PlayerRightClickStartEvent += OnRightClickStartEvent;
            inputReader.PlayerRightClickEndEvent += OnRightClickEndEvent;
            startTower.OnPushSccessfulEvent += OnPushSccessfulEvent;
            middleTower.OnPushSccessfulEvent += OnPushSccessfulEvent;
            endTower.OnPushSccessfulEvent += OnPushSccessfulEvent;
            startTime = TimeManager.Instance.GetTimeUnScaled();
            timerText.text = totalTime.ToString("F0");
            movesLeft = totalMoves;
            moveLeftText.text = movesLeft.ToString();
            isStarted = true;
        }

        private void OnPushSccessfulEvent()
        {
            moveLeftText.text = (--movesLeft).ToString();
            if (movesLeft <= 0 || endTower.DiskCount == disks.Length)
            {
                GameOver();
            }
        }

        public void GameOver()
        {
            if (!isStarted)
            {
                return;
            }
            // show player layer
            Camera.main.cullingMask |= playerLayer;
            if(selectedDisk!=null||freeDisk!=null)
            {
                UIManager.Instance.Close<UIAim>();
            }

            if (endTower.DiskCount == disks.Length)
            {
                Debug.Log("Game Over, You Win");
                //winPanelGO.SetActive(true);
                timerText.text = "You";
                moveLeftText.text = "Win";
                new HanoiWinEvent();
            }
            else
            {
                Debug.Log("Game Over, You Lose " + endTower.DiskCount);
                //losePanelGO.SetActive(true);
                timerText.text = "You";
                moveLeftText.text = "Lose";
                if (freeDisk != null)
                {
                    freeDisk.SetAsFreeDisk(false, false);
                    freeDisk = null;
                }
            }
            isStarted = false;
            for (int i = 0; i < disks.Length; i++)
            {
                disks[i].Movable = false;
            }
            UnregisterEvents();
        }

        public void UnregisterEvents()
        {
            inputReader.PlayerRightClickStartEvent -= OnRightClickStartEvent;
            inputReader.PlayerRightClickEndEvent -= OnRightClickEndEvent;
            inputReader.PlayerPointEvent -= OnPointEvent;
            startTower.OnPushSccessfulEvent -= OnPushSccessfulEvent;
            middleTower.OnPushSccessfulEvent -= OnPushSccessfulEvent;
            endTower.OnPushSccessfulEvent -= OnPushSccessfulEvent;
            startTower.Clear();
            middleTower.Clear();
            endTower.Clear();
            freeDisk = null;
            if (selectedDisk != null)
            {
                selectedDisk.SetSelected(false);
            }
            selectedDisk = null;
        }

        private void OnRightClickStartEvent()
        {
            Debug.Log("RightClickStartEvent");
            // hide player layer
            Camera.main.cullingMask &= ~playerLayer;
            if (freeDisk == null)
            {
                UIManager.Instance.OpenPanel<UIAim>();
                inputReader.PlayerPointEvent += OnPointEvent;
            }
        }

        private void OnPointEvent(Vector2 point)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 0f);
            HanoiDisk oldSelectedDisk = selectedDisk;
            selectedDisk = null;
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, diskLayer))
            {
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.TryGetComponent(out HanoiDisk disk))
                    {
                        if (disk.Movable)
                        {
                            selectedDisk = disk;
                            //calc distance between disk and camera
                            diskCameraDistance = Vector3.Distance(Camera.main.transform.position, disk.transform.position);
                        }
                    }
                }
            }
            if (oldSelectedDisk != selectedDisk)
            {
                if (oldSelectedDisk != null)
                {
                    oldSelectedDisk.SetSelected(false);
                }
                if (selectedDisk != null)
                {
                    selectedDisk.SetSelected(true);
                }
            }
        }

        private void OnScrollWheelEvent(Vector2 mv)
        {
            if (freeDisk != null)
            {
                // adjust diskCameraDistance
                if (mv.y > 0)
                {
                    diskCameraDistanceModifier += diskCameraDistanceSpeed;
                }
                else if (mv.y < 0)
                {
                    diskCameraDistanceModifier -= diskCameraDistanceSpeed;
                }
                diskCameraDistanceModifier = Mathf.Clamp(diskCameraDistanceModifier, diskCameraDistanceMin, diskCameraDistanceMax);
            }
        }

        private void OnRightClickEndEvent()
        {
            Debug.Log("RightClickEndEvent");
            if (freeDisk == null)
            {
                inputReader.PlayerPointEvent -= OnPointEvent;
            }
            else
            {
                inputReader.PlayerScrollWheelEvent -= OnScrollWheelEvent;
                freeDisk.SetAsFreeDisk(false);
                freeDisk = null;
            }
            // if selected disk is movable, set free disk to selected disk
            if (selectedDisk != null)
            {
                if (selectedDisk.Movable)
                {
                    freeDisk = selectedDisk;
                    freeDisk.SetAsFreeDisk(true);
                    diskCameraDistanceModifier = 1f;
                    inputReader.PlayerScrollWheelEvent += OnScrollWheelEvent;
                    if (selectedDisk != null)
                    {
                        selectedDisk.SetSelected(false);
                    }
                    selectedDisk = null;
                }
            }
            if (freeDisk == null)
            {
                UIManager.Instance.Close<UIAim>();
            }
        }

        private void Update()
        {
            if (!isStarted)
            {
                return;
            }
            if (freeDisk != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 0f);
                freeDisk.SetPosition(ray.origin + diskCameraDistance * diskCameraDistanceModifier * ray.direction);
            }
            float timeLeft = totalTime - (TimeManager.Instance.GetTimeUnScaled() - startTime);
            timerText.text = timeLeft.ToString("F0");
            if (timeLeft <= 0)
            {
                GameOver();
            }
        }
    }
}