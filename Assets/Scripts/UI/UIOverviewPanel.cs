using Events;
using Inputs;
using Manager;
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

        private ClickableBase clickableObj;

        private bool isActive = false;
        private float startTime;

        public override void SetData(object data)
        {
            base.SetData(data);
            Debug.Log("UIOverviewPanel SetData: " + (Vector3)data);
            new VCamChangeEvent("overview", true, (Vector3)data);
        }

        public override void OnUIEnable()
        {
            base.OnUIEnable();
            isActive = false;
            new ClickableHintEvent(true);
            inputReader.CloseUIPanelEvent += Close;
            inputReader.MiddleClickEvent += ShowHideHintImage;
            inputReader.LeftClickStartEvent += DoClickStartEvent;
            inputReader.LeftClickEndEvent += DoClickEndEvent;
            inputReader.PointEvent += OnPointEvent;
            StartCoroutine(StartCountDown());
        }

        IEnumerator StartCountDown()
        {
            yield return new WaitForSeconds(2);
            startTime = TimeManager.Instance.GetTimeUnScaled();
            isActive = true;
        }

        private void Update()
        {
            if (!isActive)
            {
                return;
            }
            float timeElapsed = TimeManager.Instance.GetTimeUnScaled() - startTime;
            float timeLeft = countDown - timeElapsed;
            if (timeLeft <= 0)
            {
                timeLeft = 0;
                Close();
            }
            //10s Before Exit
            countDownText.text = ((int)timeLeft).ToString() + "s Before Exit";
        }

        public override void OnUIDisable()
        {
            new ClickableHintEvent(false);
            inputReader.CloseUIPanelEvent -= Close;
            inputReader.MiddleClickEvent -= ShowHideHintImage;
            inputReader.LeftClickStartEvent -= DoClickStartEvent;
            inputReader.LeftClickEndEvent -= DoClickEndEvent;
            inputReader.PointEvent -= OnPointEvent;
            base.OnUIDisable();
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
            if (!isActive)
            {
                return;
            }
            isActive = false;
            if (clickableObj != null)
            {
                clickableObj.OnClickEnd();
                clickableObj = null;
            }
            new VCamChangeEvent();
            // delay 2s before close
            StartCoroutine(DelayClose());
        }

        IEnumerator DelayClose()
        {
            yield return new WaitForSeconds(2);
            base.Close();
        }
    }
}
