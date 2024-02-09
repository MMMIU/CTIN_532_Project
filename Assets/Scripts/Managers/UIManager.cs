using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using UI;
using UnityEngine.Rendering.Universal;
using Inputs;
using static UnityEngine.Rendering.DebugUI;

namespace Manager
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private InputReader inputReader;

        public List<UIBase> blockPanelStack = null;

        public Dictionary<Type, UIBase> panelDic = null;

        public Canvas Canvas { get; private set; }

        private static UIManager instance = null;

        public static UIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    var obj = Resources.Load("Prefabs/UI/UIRoot");
                    if (obj == null)
                    {
                        Debug.LogError("Prefab not found: UIRoot");
                        return null;
                    }
                    instance = Instantiate(obj).GetComponent<UIManager>();
                    instance.name = nameof(UIManager);
                    DontDestroyOnLoad(Instance.gameObject);
                    instance.RealInit();
                }
                return instance;
            }
        }

        public Camera UICamera { get; private set; } = null;

        private Dictionary<UIPanelLayer, RectTransform> layers = null;

        public RectTransform GetLayer(UIPanelLayer layer)
        {
            return layers[layer];
        }

        public void DummyInit()
        {
            return;
        }

        private void RealInit()
        {
            blockPanelStack = new List<UIBase>();
            panelDic = new Dictionary<Type, UIBase>();
            Canvas = instance.GetComponentInChildren<Canvas>();
            UICamera = instance.GetComponentInChildren<Camera>();

            // if main camera is not null, add UICamera to stack
            if (Camera.main != null)
            {
                var cameraData = Camera.main.GetUniversalAdditionalCameraData();
                cameraData.cameraStack.Add(UICamera);
            }

            // get all layers
            layers = new Dictionary<UIPanelLayer, RectTransform>();
            foreach (UIPanelLayer layer in Enum.GetValues(typeof(UIPanelLayer)))
            {
                var layerName = layer.ToString();
                layers.Add(layer, Canvas.transform.Find(layerName) as RectTransform);
            }

            Debug.Log("UIManager Init");
        }

        private void OnDestroy()
        {
            if (Camera.main != null)
            {
                var cameraData = Camera.main.GetUniversalAdditionalCameraData();
                cameraData.cameraStack.Remove(UICamera);
            }
        }

        public T OpenPanel<T>(object data = null) where T : UIBase
        {
            void OpenP(UIBase panel)
            {
                panel.transform.SetAsLastSibling();
                panel.SetData(data);
                var objects = typeof(T).GetCustomAttributes(typeof(UIBlockAttribute), true);
                if (objects?.Length > 0)
                {
                    var blockAttribute = objects[0] as UIBlockAttribute;
                    if (blockAttribute.block)
                    {
                        blockPanelStack.Add(panel);
                    }
                }
                panel.OnUIEnable();
            }

            UIBase CloneP(out bool saveToDic)
            {
                GameObject prefab = Resources.Load<GameObject>($"Prefabs/UI/{typeof(T).Name}");
                if (prefab == null)
                {
                    Debug.LogError($"Prefab not found: {typeof(T).Name}");
                    saveToDic = false;
                    return null;
                }

                RectTransform layer = GetLayer(UIPanelLayer.Normal);
                var objects = typeof(T).GetCustomAttributes(typeof(UILayerAttribute), true);
                if (objects?.Length > 0)
                {
                    var layerAttribute = objects[0] as UILayerAttribute;
                    layer = GetLayer(layerAttribute.layer);
                }
                
                var panelObject = Instantiate(prefab, layer);
                var panel = panelObject.GetComponent<T>();
                panel.name = typeof(T).Name;

                objects = typeof(T).GetCustomAttributes(typeof(UISaveDicAttribute), true);
                if(objects?.Length > 0)
                {
                    saveToDic = (objects[0] as UISaveDicAttribute).saveDic;
                }
                else
                {
                    saveToDic = true;
                }
                return panel;
            }

            var type = typeof(T);
            UIBase panelInstance;

            if (panelDic.TryGetValue(type, out var existingPanel))
            {
                Debug.Log("Panel already exists");
                panelInstance = existingPanel;
                OpenP(panelInstance);
            }
            else
            {
                Debug.Log("Panel does not exist, Cloning");
                panelInstance = CloneP(out bool saveToDic);
                if (panelInstance != null)
                {
                    if (saveToDic)
                    {
                        panelDic.Add(type, panelInstance);
                    }
                    panelInstance.OnUIAwake();
                    // delay one frame
                    StartCoroutine(Invoke(panelInstance.OnUIStart));
                    OpenP(panelInstance);
                }
            }


            if (blockPanelStack.Count > 0)
            {
                Debug.Log("Block UI panels are opened");
                inputReader.EnableUIInput();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            return instance == null ? null : panelInstance as T;
        }

        public void Close<T>() where T : UIBase
        {
            var type = typeof(T);
            if (!panelDic.TryGetValue(type, out var panel))
            {
                Debug.LogError($"Panel not found: {type.Name}");
            }
            else
            {
                Close(panel);
            }
        }

        public void Close(UIBase panel)
        {
            panel?.OnUIDisable();
            if (blockPanelStack.Contains(panel))
            {
                blockPanelStack.Remove(panel);
                if (blockPanelStack.Count == 0)
                {
                    Debug.Log("All UI panels are closed");
                    inputReader.EnablePlayerInput();
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }

        public void CloseAll()
        {
            foreach (var panel in panelDic.Values)
            {
                Close(panel);
            }
        }

        public void Destroy<T>() where T : UIBase
        {
            var type = typeof(T);
            if (!panelDic.TryGetValue(type, out var panel))
            {
                Debug.LogError($"Panel not found: {type.Name}");
            }
            else
            {
                Destroy(panel);
            }
        }

        public void Destroy(UIBase panel)
        {
            panel?.OnUIDestroy();
            if (panelDic.ContainsValue(panel))
            {
                panelDic.Remove(panel.GetType());
            }
        }

        public void CloseAndDestroyAll()
        {
            foreach (var panel in panelDic.Values)
            {
                Close(panel);
                Destroy(panel);
            }
            panelDic.Clear();
        }

        public T Get<T>() where T : UIBase
        {
            var type = typeof(T);
            if (!panelDic.TryGetValue(type, out var panel))
            {
                Debug.LogError($"Panel not found: {type.Name}");
                return default;
            }
            else
            {
                return panel as T;
            }
        }

        private IEnumerator Invoke(Action callback)
        {
            yield return new WaitForEndOfFrame();
            callback?.Invoke();
        }

        public void OpenNotification()
        {

        }
    }

}
