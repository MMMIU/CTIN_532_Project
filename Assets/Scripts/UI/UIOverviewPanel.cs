using Cinemachine;
using Events;
using Inputs;
using Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [UIBlock]
    [UILayer(UIPanelLayer.Normal)]
    public class UIOverviewPanel : UIBase
    {
        [SerializeField]
        private InputReader inputReader;

        [SerializeField]
        private LayerMask clickableLayer;

        [SerializeField]
        private TextMeshProUGUI countDownText;

        [SerializeField]
        private float countDown = 10f;

        [SerializeField]
        private ClickableBase clickableObj;

        [SerializeField]
        private float startTime = -1;

        public override void SetData(object data)
        {
            base.SetData(data);
            Debug.Log("UIOverviewPanel SetData: " + (Vector3)data);
            new VCamChangeEvent("overview", true, (Vector3)data);
        }

        public override void OnUIEnable()
        {
            base.OnUIEnable();
            new ClickableHintEvent(true);
            inputReader.CloseUIPanelEvent += Close;
            inputReader.MiddleClickEvent += ShowHideHintImage;
            inputReader.LeftClickStartEvent += DoClickStartEvent;
            inputReader.LeftClickEndEvent += DoClickEndEvent;
            inputReader.PointEvent += OnPointEvent;
            // set mouse position to center but not locked
            StartCoroutine(CenterCursor());
            startTime = -1;
            countDownText.text = "";
            StartCoroutine(StartCountDown());
        }

        IEnumerator CenterCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            yield return null;
            Cursor.lockState = CursorLockMode.None;
        }

        IEnumerator StartCountDown()
        {
            yield return new WaitForSeconds(2);
            startTime = TimeManager.Instance.GetTimeUnScaled();
            inputReader.VCamMoveEvent += RotateVCam;
        }

        private void Update()
        {
            if (startTime < 0)
            {
                countDownText.text = "";
                return;
            }
            float timeElapsed = TimeManager.Instance.GetTimeUnScaled() - startTime;
            float timeLeft = countDown - timeElapsed;
            if (timeLeft <= 0)
            {
                timeLeft = -1;
                Debug.LogWarning("Time's up!");
                countDownText.text = "Time's up!";
                Close();
            }
            else
            {
                countDownText.text = ((int)timeLeft).ToString() + "s Before Exit";
            }
        }

        public override void OnUIDisable()
        {
            new ClickableHintEvent(false);
            base.OnUIDisable();
        }

        private void UnRegisterEvents()
        {
            inputReader.CloseUIPanelEvent -= Close;
            inputReader.MiddleClickEvent -= ShowHideHintImage;
            inputReader.LeftClickStartEvent -= DoClickStartEvent;
            inputReader.LeftClickEndEvent -= DoClickEndEvent;
            inputReader.PointEvent -= OnPointEvent;
            inputReader.VCamMoveEvent -= RotateVCam;
        }

        private void OnPointEvent(Vector2 point)
        {
            bool isHover = false;
            Ray ray = Camera.main.ScreenPointToRay(inputReader.MouseCurrPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, clickableLayer))
            {
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.TryGetComponent(out ClickableBase clickable))
                    {
                        inputReader.SetCursorHover();
                        isHover = true;
                    }
                }
            }
            if (!isHover)
            {
                inputReader.SetCursorDefault();
            }
        }

        private void DoClickStartEvent()
        {
            Debug.Log("DoClickStartEvent");
            Ray ray = Camera.main.ScreenPointToRay(inputReader.MouseCurrPosition);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, clickableLayer))
            {
                if (hit.collider != null)
                {
                    Debug.Log("LeftClick Raycast hit: " + hit.collider);
                    if (hit.collider.gameObject.TryGetComponent(out ClickableBase clickable))
                    {
                        clickable.OnClickStart();
                        clickableObj = clickable;
                    }
                }
            }
        }

        private void DoClickEndEvent()
        {
            Debug.Log("DoClickEndEvent");
            if (clickableObj != null)
            {
                clickableObj.OnClickEnd();
                clickableObj = null;
            }
        }

        private void ShowHideHintImage()
        {
            GameObject hintImage = GameObject.Find("HintImage");
            MeshRenderer hintImageRenderer = hintImage.GetComponent<MeshRenderer>();
            hintImageRenderer.enabled = !hintImageRenderer.enabled;
        }

        public override void Close()
        {
            startTime = -1;
            UnRegisterEvents();
            new VCamChangeEvent();
            if (clickableObj != null)
            {
                clickableObj.OnClickEnd();
                clickableObj = null;
            }
            StartCoroutine(DelayClose());
            // delay 2s before close
        }

        IEnumerator DelayClose()
        {
            yield return new WaitForSeconds(2);
            base.Close();
        }

        private void RotateVCam(Vector2 delta)
        {
            CinemachineVirtualCamera vcam = VCamManager.Instance.CurrentVCam;
            if (vcam == null)
            {
                return;
            }

            var rotation = vcam.transform.localRotation.eulerAngles;

            float rotationSpeed = 0.1f;

            float yaw = rotation.y + delta.x * rotationSpeed;
            float pitch = rotation.x - delta.y * rotationSpeed;

            pitch = Mathf.Clamp(pitch, 60f, 89f);

            vcam.transform.localRotation = Quaternion.Euler(pitch, yaw, 0);
        }
    }
}
