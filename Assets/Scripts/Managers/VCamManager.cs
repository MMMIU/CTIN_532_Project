using Cinemachine;
using Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Managers
{
    public class VCamManager : MonoBehaviour
    {
        [SerializeField]
        private string defaultVCam;

        [SerializeField]
        private Pair<string, CinemachineVirtualCamera> currentVCam;

        public static VCamManager Instance { get; private set; }

        public List<Pair<string, CinemachineVirtualCamera>> vCams;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DisableAll();
            EnableVCam(defaultVCam);
        }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe<VCamChangeEvent>(OnVCamChange);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe<VCamChangeEvent>(OnVCamChange);
        }

        private void OnVCamChange(VCamChangeEvent e)
        {
            if(string.IsNullOrEmpty(e.vCamName))
            {
                DisableAll();
            }
            else
            {
                EnableVCam(e.vCamName);
            }
        }

        public void DisableAll()
        {
            currentVCam = null;
            foreach (var pair in vCams)
            {
                pair.Second.gameObject.SetActive(false);
            }
        }

        public void EnableVCam(string vCamName)
        {
            // if the vCamName is the same as the current vCam, do nothing
            if (currentVCam != null && currentVCam.First == vCamName)
            {
                return;
            }

            // if current vCam is not null, disable it
            currentVCam?.Second.gameObject.SetActive(false);

            // find the vCam with the given name and enable it
            currentVCam = vCams.Find(pair => pair.First == vCamName);
            if (currentVCam != null)
            {
                currentVCam.Second.gameObject.SetActive(true);
                //if(vCamName.Equals("overview"))
                //{
                //    // set the position of the vCam to player's position, only change the X and Z axis
                //    currentVCam.Second.transform.position = new Vector3(GameManager.Instance.LocalPlayer.transform.position.x, currentVCam.Second.transform.position.y, GameManager.Instance.LocalPlayer.transform.position.z);
                //    // set the rotation Y of the vCam to player's rotation Y, only change the Y axis
                //    currentVCam.Second.transform.rotation = Quaternion.Euler(90, GameManager.Instance.LocalPlayer.transform.rotation.eulerAngles.y, 0);
                //}
            }
        }
    }
}