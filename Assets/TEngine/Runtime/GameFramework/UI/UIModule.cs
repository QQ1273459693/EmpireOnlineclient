using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace TEngine
{
    /// <summary>
    /// UI模块。
    /// </summary>
    [DisallowMultipleComponent]
    public sealed partial class UIModule : GameFrameworkModuleBase
    {
        private const int DefaultPriority = 0;

        [SerializeField] private Transform m_InstanceRoot = null;

        [SerializeField] private bool m_dontDestroyUIRoot = true;

        [SerializeField] private bool m_enableErrorLog = true;
        
        [SerializeField] private Camera m_UICamera = null;

        private readonly List<UIWindow> _stack = new List<UIWindow>(100);

        public const int LAYER_DEEP = 2000;
        public const int WINDOW_DEEP = 100;
        public const int WINDOW_HIDE_LAYER = 2; // Ignore Raycast
        public const int WINDOW_SHOW_LAYER = 5; // UI

        /// <summary>
        /// UI根节点。
        /// </summary>
        public Transform UIRoot => m_InstanceRoot;

        public static Transform UIRootStatic;

        public static Transform MainRoot;

        public static Transform NormalRoot;

        public static Transform PopupRoot;

        public static Transform TipsRoot;

        public static Transform LoadingRoot;

        /// <summary>
        /// UI根节点。
        /// </summary>
        public Camera UICamera => m_UICamera;

        private void Start()
        {
            RootModule rootModule = GameEntry.GetModule<RootModule>();
            if (rootModule == null)
            {
                Log.Fatal("Base component is invalid.");
                return;
            }

            if (m_InstanceRoot == null)
            {
                m_InstanceRoot = new GameObject("UI Form Instances").transform;
                m_InstanceRoot.SetParent(gameObject.transform);
                m_InstanceRoot.localScale = Vector3.one;
            }
            else if (m_dontDestroyUIRoot)
            {
                DontDestroyOnLoad(m_InstanceRoot.parent !=null ? m_InstanceRoot.parent : m_InstanceRoot);
            }

            m_InstanceRoot.gameObject.layer = LayerMask.NameToLayer("UI");
            UIRootStatic = m_InstanceRoot;
            //加载UI节点
            MainRoot = UIRootStatic.Find("Main");
            NormalRoot= UIRootStatic.Find("Normal");
            PopupRoot= UIRootStatic.Find("Popup");
            TipsRoot= UIRootStatic.Find("Tips");
            LoadingRoot= UIRootStatic.Find("Loading");
            //加载UI节点
            if (m_enableErrorLog)
            {
                ErrorLogger errorLogger = new ErrorLogger();
            }
        }

        private void OnDestroy()
        {
            CloseAll();
        }
        private void Update()
        {
            int count = _stack.Count;
            for (int i = 0; i < _stack.Count; i++)
            {
                if (_stack.Count != count)
                {
                    break;
                }

                var window = _stack[i];
                window.InternalUpdate();
            }
        }
        
        #region 设置安全区域

        /// <summary>
        /// 设置屏幕安全区域（异形屏支持）。
        /// </summary>
        /// <param name="safeRect">安全区域</param>
        public static void ApplyScreenSafeRect(Rect safeRect)
        {
            CanvasScaler scaler = UIRootStatic.GetComponentInParent<CanvasScaler>();
            if (scaler == null)
            {
                Log.Error($"Not found {nameof(CanvasScaler)} !");
                return;
            }

            // Convert safe area rectangle from absolute pixels to UGUI coordinates
            float rateX = scaler.referenceResolution.x / Screen.width;
            float rateY = scaler.referenceResolution.y / Screen.height;
            float posX = (int)(safeRect.position.x * rateX);
            float posY = (int)(safeRect.position.y * rateY);
            float width = (int)(safeRect.size.x * rateX);
            float height = (int)(safeRect.size.y * rateY);

            float offsetMaxX = scaler.referenceResolution.x - width - posX;
            float offsetMaxY = scaler.referenceResolution.y - height - posY;

            // 注意：安全区坐标系的原点为左下角	
            var rectTrans = UIRootStatic.transform as RectTransform;
            if (rectTrans != null)
            {
                rectTrans.offsetMin = new Vector2(posX, posY); //锚框状态下的屏幕左下角偏移向量
                rectTrans.offsetMax = new Vector2(-offsetMaxX, -offsetMaxY); //锚框状态下的屏幕右上角偏移向量
            }
        }

        /// <summary>
        /// 模拟IPhoneX异形屏
        /// </summary>
        public static void SimulateIPhoneXNotchScreen()
        {
            Rect rect;
            if (Screen.height > Screen.width)
            {
                // 竖屏Portrait
                float deviceWidth = 1125;
                float deviceHeight = 2436;
                rect = new Rect(0f / deviceWidth, 102f / deviceHeight, 1125f / deviceWidth, 2202f / deviceHeight);
            }
            else
            {
                // 横屏Landscape
                float deviceWidth = 2436;
                float deviceHeight = 1125;
                rect = new Rect(132f / deviceWidth, 63f / deviceHeight, 2172f / deviceWidth, 1062f / deviceHeight);
            }

            Rect safeArea = new Rect(Screen.width * rect.x, Screen.height * rect.y, Screen.width * rect.width, Screen.height * rect.height);
            ApplyScreenSafeRect(safeArea);
        }

        #endregion

        /// <summary>
        /// 获取所有层级下顶部的窗口名称。
        /// </summary>
        public string GetTopWindow()
        {
            if (_stack.Count == 0)
            {
                return string.Empty;
            }

            UIWindow topWindow = _stack[_stack.Count - 1];
            return topWindow.WindowName;
        }

        /// <summary>
        /// 获取指定层级下顶部的窗口名称。
        /// </summary>
        public string GetTopWindow(int layer)
        {
            UIWindow lastOne = null;
            for (int i = 0; i < _stack.Count; i++)
            {
                if (_stack[i].WindowLayer == layer)
                    lastOne = _stack[i];
            }

            if (lastOne == null)
                return string.Empty;

            return lastOne.WindowName;
        }

        /// <summary>
        /// 是否有任意窗口正在加载。
        /// </summary>
        public bool IsAnyLoading()
        {
            for (int i = 0; i < _stack.Count; i++)
            {
                var window = _stack[i];
                if (window.IsLoadDone == false)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 查询窗口是否存在。
        /// </summary>
        /// <typeparam name="T">界面类型。</typeparam>
        /// <returns>是否存在。</returns>
        public bool HasWindow<T>()
        {
            return HasWindow(typeof(T));
        }

        /// <summary>
        /// 查询窗口是否存在。
        /// </summary>
        /// <param name="type">界面类型。</param>
        /// <returns>是否存在。</returns>
        public bool HasWindow(Type type)
        {
            return IsContains(type.FullName)!=null;
        }

        /// <summary>
        /// 异步打开窗口。
        /// </summary>
        /// <param name="userDatas">用户自定义数据。</param>
        /// <returns>打开窗口操作句柄。</returns>
        public OpenWindowOperation ShowUIAsync<T>() where T : UIWindow
        {
            return ShowUIAsync(typeof(T));
        }

        /// <summary>
        /// 异步打开窗口。
        /// </summary>
        /// <param name="type">界面类型。</param>
        /// <param name="userDatas">用户自定义数据。</param>
        /// <returns>打开窗口操作句柄。</returns>
        public OpenWindowOperation ShowUIAsync(Type type)
        {
            string windowName = type.FullName;

            // 如果窗口已经存在
            if (IsContains(windowName)!=null)
            {
                UIWindow window = GetWindow(windowName);
                window.InternalRefresh();
                window.TryInvoke(null);
                return null;
            }
            else
            {
                UIWindow window = CreateInstance(type);
                Push(window); //首次压入
                window.InternalLoad(window.AssetName, OnWindowPrepare);
                var operation = new OpenWindowOperation(window.Handle);
                YooAssets.StartOperation(operation);
                return operation;
            }
        }
        /// <summary>
        /// 同步打开窗口。
        /// </summary>
        /// <typeparam name="T">窗口类。</typeparam>
        /// <param name="userDatas">用户自定义数据。</param>
        /// <returns>打开窗口操作句柄。</returns>
        //public OpenWindowOperation ShowUI<T>() where T : UIWindow
        //{
        //    var operation = ShowUIAsync(typeof(T));
        //    if (operation != null)
        //    {
        //        operation.WaitForAsyncComplete();
        //    }
        //    return operation;
        //}
        #region 新窗口打开
        /// <summary>
        /// 多类型进入
        /// </summary>
        /// <returns></returns>
        public OpenWindowOperation ShowUI<T>() where T : UIWindow
        {
            var operation = ShowWindow<T>();
            if (operation != null)
            {
                operation.WaitForAsyncComplete();
            }
            return operation;
        }
        public OpenWindowOperation ShowUI<T,A>(A a) where T : UIWindow
        {
            var operation = ShowWindow<T,A>(a);//TestInitShow(typeof(T));
            if (operation != null)
            {
                operation.WaitForAsyncComplete();
            }
            return operation;
        }
        public OpenWindowOperation ShowUI<T,A,B>(A a,B b) where T : UIWindow
        {
            var operation = ShowWindow<T, A,B>(a,b);
            if (operation != null)
            {
                operation.WaitForAsyncComplete();
            }
            return operation;
        }
        public OpenWindowOperation ShowUI<T, A, B,C>(A a, B b,C c) where T : UIWindow
        {
            var operation = ShowWindow<T, A, B,C>(a, b,c);
            if (operation != null)
            {
                operation.WaitForAsyncComplete();
            }
            return operation;
        }
        OpenWindowOperation ShowWindow<T>() where T : UIWindow
        {
            Type type = typeof(T);
            System.Object[] obj = GetUIWindowData(type,true);
            OpenWindowOperation operation = (OpenWindowOperation)obj[1];
            
            // 如果窗口已经存在
            if (operation != null)
            {
                YooAssets.StartOperation(operation);
            }
            return operation;
        }
        OpenWindowOperation ShowWindow<T, A>(A a) where T : UIWindow
        {
            Type type = typeof(T);
            System.Object[] obj = GetUIWindowData(type);
            UIWindow window= (UIWindow)obj[0];
            OpenWindowOperation operation=(OpenWindowOperation)obj[1];
            // 如果窗口已经存在
            var tmpInitData = window as UIWindow.IInitData<A>;

            if (null != tmpInitData)
            {
                tmpInitData.InitData(a);
            }
            window.InternalRefresh();
            if (operation!=null)
            {
                YooAssets.StartOperation(operation);
            }
            return operation;
        }
        OpenWindowOperation ShowWindow<T, A,B>(A a,B b) where T : UIWindow
        {
            Type type = typeof(T);
            System.Object[] obj = GetUIWindowData(type);
            UIWindow window = (UIWindow)obj[0];
            OpenWindowOperation operation = (OpenWindowOperation)obj[1];
            // 如果窗口已经存在
            var tmpInitData = window as UIWindow.IInitData2<A,B>;

            if (null != tmpInitData)
            {
                tmpInitData.InitData(a,b);
            }
            window.InternalRefresh();
            if (operation != null)
            {
                YooAssets.StartOperation(operation);
            }
            return operation;
        }
        OpenWindowOperation ShowWindow<T, A, B, C>(A a, B b, C c) where T : UIWindow
        {
            Type type = typeof(T);
            System.Object[] obj = GetUIWindowData(type);
            UIWindow window = (UIWindow)obj[0];
            OpenWindowOperation operation = (OpenWindowOperation)obj[1];
            // 如果窗口已经存在
            var tmpInitData = window as UIWindow.IInitData3<A, B, C>;

            if (null != tmpInitData)
            {
                tmpInitData.InitData(a, b, c);
            }
            window.InternalRefresh();
            if (operation != null)
            {
                YooAssets.StartOperation(operation);
            }
            return operation;
        }
        /// <summary>
        /// 从这里获取窗口和YooAsset操作,布尔是 是否是仅仅show单个不带参数
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        System.Object[] GetUIWindowData(Type type,bool isSingeWindow=false)
        {
            string windowName = type.FullName;
            UIWindow window;
            OpenWindowOperation operation;


            window = IsContains(windowName);
            // 如果窗口已经存在
            if (window != null)
            {
                if (window.WindowLayer==1||window.WindowLayer==0)
                {
                    HideNormalWindow();
                }
                if (isSingeWindow)
                {
                    window.InternalRefresh();
                }
                //window.InternalRefresh()
                //window.Visible = true;
                operation = null;
            }
            else
            {
                window = CreateInstance(type);
                if (window.WindowLayer == 1 || window.WindowLayer == 0)
                {
                    HideNormalWindow();
                }
                Push(window); //首次压入
                window.InternalLoad(window.AssetName, OnWindowPrepare);
                operation = new OpenWindowOperation(window.Handle);
            }
            return new object[] { window,operation };
        }

        #endregion
        /// <summary>
        /// 同步打开窗口。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="userDatas"></param>
        /// <returns>打开窗口操作句柄。</returns>
        public OpenWindowOperation ShowUI(Type type)
        {
            var operation = ShowUIAsync(type);
            if(operation != null)
            {
                operation.WaitForAsyncComplete();
            }
            
            return operation;
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public void CloseWindow<T>() where T : UIWindow
        {
            CloseWindow(typeof(T));
        }

        public void CloseWindow(Type type)
        {
            string windowName = type.FullName;
            UIWindow window = GetWindow(windowName);
            if (window == null)
                return;
            window.BeforeClose();
            return;
            window.InternalDestroy();
            Pop(window);
            OnSetWindowVisible();
        }

        /// <summary>
        /// 关闭所有窗口。
        /// </summary>
        public void CloseAll()
        {
            for (int i = 0; i < _stack.Count; i++)
            {
                UIWindow window = _stack[i];
                window.InternalDestroy();
            }

            _stack.Clear();
        }

        private void OnWindowPrepare(UIWindow window)
        {
            window.InternalCreate();
            window.InternalRefresh();
            OnSetWindowVisible();
        }

        private void OnSetWindowVisible()
        {
            bool isHideNext = false;
            for (int i = _stack.Count - 1; i >= 0; i--)
            {
                UIWindow window = _stack[i];
                if (isHideNext == false)
                {
                    window.Visible = true;
                    if (window.IsPrepare && window.FullScreen)
                    {
                        isHideNext = true;
                    }
                }
                else
                {
                    window.Visible = false;
                }
            }
        }

        private UIWindow CreateInstance(Type type)
        {
            UIWindow window = Activator.CreateInstance(type) as UIWindow;
            WindowAttribute attribute = Attribute.GetCustomAttribute(type, typeof(WindowAttribute)) as WindowAttribute;

            if (window == null)
                throw new Exception($"Window {type.FullName} create instance failed.");
            if (attribute == null)
                throw new Exception($"Window {type.FullName} not found {nameof(WindowAttribute)} attribute.");

            string assetName = string.IsNullOrEmpty(attribute.AssetName) ? type.Name : attribute.AssetName;
            window.Init(type.FullName, attribute.WindowLayer,attribute.FullScreen, assetName);
            return window;
        }

        private UIWindow GetWindow(string windowName)
        {
            for (int i = 0; i < _stack.Count; i++)
            {
                UIWindow window = _stack[i];
                if (window.WindowName == windowName)
                {
                    return window;
                }
            }

            return null;
        }
        /// <summary>
        /// 这里是指定隐藏Normal层级1的窗口
        /// </summary>
        private void HideNormalWindow()
        {
            for (int i = 0; i < _stack.Count; i++)
            {
                UIWindow window = _stack[i];
                //Log.Debug($"窗口名:{window.WindowName},层级是:{window.WindowLayer},窗口可见性:{window.Visible}");
                if (window.WindowLayer==1&&window.Visible)
                {
                    //Log.Debug("正在关闭窗口:"+window.WindowName);
                    window.BeforeClose();
                    return;
                }
            }
        }
        private UIWindow IsContains(string windowName)
        {
            for (int i = 0; i < _stack.Count; i++)
            {
                UIWindow window = _stack[i];
                if (window.WindowName == windowName)
                {
                    return window;
                }
            }

            return null;
        }

        private void Push(UIWindow window)
        {
            // 如果已经存在
            if (IsContains(window.WindowName)!=null)
                throw new System.Exception($"Window {window.WindowName} is exist.");

            // 获取插入到所属层级的位置
            int insertIndex = -1;
            for (int i = 0; i < _stack.Count; i++)
            {
                if (window.WindowLayer == _stack[i].WindowLayer)
                {
                    insertIndex = i + 1;
                }
            }

            // 如果没有所属层级，找到相邻层级
            if (insertIndex == -1)
            {
                for (int i = 0; i < _stack.Count; i++)
                {
                    if (window.WindowLayer > _stack[i].WindowLayer)
                    {
                        insertIndex = i + 1;
                    }
                }
            }

            // 如果是空栈或没有找到插入位置
            if (insertIndex == -1)
            {
                insertIndex = 0;
            }

            // 最后插入到堆栈
            _stack.Insert(insertIndex, window);
        }

        private void Pop(UIWindow window)
        {
            // 从堆栈里移除
            _stack.Remove(window);
        }
    }
}