using System;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
using Object = UnityEngine.Object;

namespace TEngine
{
    public abstract class UIWindow : UIBase
    {
        #region Propreties
        private System.Action<UIWindow> _prepareCallback;

        private bool _isCreate = false;

        private GameObject _panel;

        //private Canvas _canvas;

        //private Canvas[] _childCanvas;

        //private GraphicRaycaster _raycaster;

        //private GraphicRaycaster[] _childRaycaster;

        public override UIBaseType BaseType => UIBaseType.Window;

        /// <summary>
        /// 窗口矩阵位置组件。
        /// </summary>
        public override RectTransform rectTransform => _panel.transform as RectTransform;

        /// <summary>
        /// 窗口的实例资源对象。
        /// </summary>
        public override GameObject gameObject => _panel;

        /// <summary>
        /// 窗口名称。
        /// </summary>
        public string WindowName { private set; get; }

        /// <summary>
        /// 窗口层级。
        /// </summary>
        public int WindowLayer { private set; get; }

        /// <summary>
        /// 资源定位地址。
        /// </summary>
        public string AssetName { private set; get; }

        /// <summary>
        /// 是否为全屏窗口
        /// </summary>
        public bool FullScreen { private set; get; }

        /// <summary>
        /// 窗口可见性
        /// </summary>
        public bool Visible
        {
            get
            {
                return gameObject.activeSelf;
            }

            set
            {
                //gameObject.SetActive(value);

                // 虚函数
                //if (_isCreate)
                //{
                //    OnSetVisible(value);
                //}
            }
        }
        public interface IInitData<A>
        {
            void InitData(A a);
        }

        public interface IInitData2<A, B>
        {
            void InitData(A a, B b);
        }

        public interface IInitData3<A, B, C>
        {
            void InitData(A a, B b, C c);
        }
        /// <summary>
        /// 是否加载完毕。
        /// </summary>
        internal bool IsLoadDone => Handle.IsDone;
        #endregion

        public void Init(string name, int layer,bool fullScreen, string assetName)
        {
            WindowName = name;
            WindowLayer = layer;
            FullScreen = fullScreen;
            AssetName = assetName;
        }
        
        internal void TryInvoke(System.Action<UIWindow> prepareCallback)
        {
            if (IsPrepare)
            {
                prepareCallback?.Invoke(this);
            }
            else
            {
                _prepareCallback = prepareCallback;
            }
        }

        internal void InternalLoad(string location, System.Action<UIWindow> prepareCallback)
        {
            if (Handle != null)
            {
                return;
            }

            _prepareCallback = prepareCallback;
            Handle = YooAssets.LoadAssetAsync<GameObject>(location);
            Handle.Completed += Handle_Completed;
        }

        internal void InternalCreate()
        {
            if (_isCreate == false)
            {
                _isCreate = true;
                ScriptGenerator();
                BindMemberProperty();
                RegisterEvent();
                OnCreate();
            }
        }

        internal void InternalRefresh()
        {
            if (gameObject!=null)
            {
                AfterShow();
                Visible = true;
            }           
        }
        internal void InternalBeforeClose()
        {
            BeforeClose();
        }

        internal bool InternalUpdate()
        {
            if (!IsPrepare ||!Visible)
            {
                return false;
            }
            
            List<UIWidget> listNextUpdateChild = null;
            if (ListChild != null && ListChild.Count > 0)
            {
                listNextUpdateChild = m_listUpdateChild;
                var updateListValid = m_updateListValid;
                List<UIWidget> listChild = null;
                if (!updateListValid)
                {
                    if (listNextUpdateChild == null)
                    {
                        listNextUpdateChild = new List<UIWidget>();
                        m_listUpdateChild = listNextUpdateChild;
                    }
                    else
                    {
                        listNextUpdateChild.Clear();
                    }

                    listChild = ListChild;
                }
                else
                {
                    listChild = listNextUpdateChild;
                }

                for (int i = 0; i < listChild.Count; i++)
                {
                    var uiWidget = listChild[i];

                    TProfiler.BeginSample(uiWidget.name);
                    var needValid = uiWidget.InternalUpdate();
                    TProfiler.EndSample();

                    if (!updateListValid && needValid)
                    {
                        listNextUpdateChild.Add(uiWidget);
                    }
                }

                if (!updateListValid) 
                {
                    m_updateListValid = true;
                }
            }

            TProfiler.BeginSample("OnUpdate");

            bool needUpdate = false;
            if (listNextUpdateChild == null || listNextUpdateChild.Count <= 0)
            {
                HasOverrideUpdate = true;
                OnUpdate();
                needUpdate = HasOverrideUpdate;
            }
            else
            {
                OnUpdate();
                needUpdate = true;
            }
            TProfiler.EndSample();
            
            return needUpdate;
        }

        internal void InternalDestroy()
        {
            _isCreate = false;

            RemoveAllUIEvent();

            for (int i = 0; i < ListChild.Count; i++)
            {
                var uiChild = ListChild[i];
                uiChild.OnDestroy();
                uiChild.OnDestroyWidget();
            }

            // 注销回调函数
            _prepareCallback = null;

            // 卸载面板资源
            if (Handle != null)
            {
                Handle.Release();
                Handle = null;
            }

            // 销毁面板对象
            if (_panel != null)
            {
                OnDestroy();
                Object.Destroy(_panel);
                _panel = null;
            }
        }

        /// <summary>
        /// 处理资源加载完成回调。
        /// </summary>
        /// <param name="handle"></param>
        /// <exception cref="Exception"></exception>
        private void Handle_Completed(AssetOperationHandle handle)
        {
            if (handle.AssetObject == null)
            {
                return;
            }

            // 实例化对象
            switch (WindowLayer)
            {
                case 0:
                    _panel = handle.InstantiateSync(UIModule.MainRoot);
                    break;
                case 1:
                    _panel = handle.InstantiateSync(UIModule.NormalRoot);
                    break;
                case 2:
                    _panel = handle.InstantiateSync(UIModule.PopupRoot);
                    break;
                case 3:
                    _panel = handle.InstantiateSync(UIModule.TipsRoot);
                    break;
                case 4:
                    _panel = handle.InstantiateSync(UIModule.LoadingRoot);
                    break;
            }
            
            _panel.transform.localPosition = Vector3.zero;
            
            // 绑定引用
            AssetReference = AssetReference.BindAssetReference(_panel);

            // 获取组件
            //_canvas = _panel.GetComponent<Canvas>();
            //if (_canvas == null)
            //{
            //    throw new Exception($"Not found {nameof(Canvas)} in panel {WindowName}");
            //}

            //_canvas.overrideSorting = true;
            //_canvas.sortingOrder = 0;
            //_canvas.sortingLayerName = "Default";

            // 通知UI管理器
            IsPrepare = true;
            _prepareCallback?.Invoke(this);
        }
        
        
        protected virtual void Close()
        {
            GameModule.UI.CloseWindow(this.GetType());
        }
    }
}