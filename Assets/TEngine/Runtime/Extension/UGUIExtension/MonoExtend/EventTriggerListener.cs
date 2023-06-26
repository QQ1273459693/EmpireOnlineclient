using UnityEngine;
using UnityEngine.EventSystems;

namespace TEngine
{
    public delegate void UIEventHandle<T>(GameObject go, T eventData) where T : BaseEventData;

    public class EventTriggerListener : MonoBehaviour,IPointerClickHandler,IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler,IPointerUpHandler
    {
        public class EventHandle<T> where T : BaseEventData
        {
            private event UIEventHandle<T> m_Handle;

            public void AddListener(UIEventHandle<T> handle)
            {
                m_Handle = handle;
            }

            public void RemoveListener(UIEventHandle<T> handle)
            {
                m_Handle -= handle;
            }

            public void RemoveAllListener()
            {
                m_Handle -= m_Handle;
                m_Handle = null;
            }

            public void Invoke(GameObject go, T eventData)
            {
                m_Handle?.Invoke(go, eventData);
            }
        }

        public EventHandle<PointerEventData> onDragLeftAndRight = new EventHandle<PointerEventData>();
        public EventHandle<PointerEventData> onClick = new EventHandle<PointerEventData>();
        public EventHandle<PointerEventData> onDoubleClick = new EventHandle<PointerEventData>();
        public EventHandle<PointerEventData> onPress = new EventHandle<PointerEventData>();
        public EventHandle<PointerEventData> onContinuousPress = new EventHandle<PointerEventData>();
        public EventHandle<PointerEventData> onUp = new EventHandle<PointerEventData>();
        public EventHandle<PointerEventData> onDown = new EventHandle<PointerEventData>();
        public EventHandle<PointerEventData> onEnter = new EventHandle<PointerEventData>();
        public EventHandle<PointerEventData> onExit = new EventHandle<PointerEventData>();
        public EventHandle<PointerEventData> onLongPress = new EventHandle<PointerEventData>();//长按

        private float m_ClickTime;
        private PointerEventData m_OnClickEventData;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!m_IsPress)
            {
                m_ClickCount++;
                m_ClickTime= Time.unscaledTime;
                m_OnClickEventData = eventData;
            }         
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onEnter.Invoke(gameObject, eventData);
            if (IsDragLeftAndRight)
            {
                //说明是左右拖动判断
                m_DragStartPos = eventData.position;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onExit.Invoke(gameObject, eventData);
           
        }     

        public void OnPointerDown(PointerEventData eventData)
        {
            m_IsPointDown = true;
            m_IsPress = false;
            m_IsLongPress = true;
            m_CurrDownTime = Time.unscaledTime;
            onDown?.Invoke(gameObject, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            m_IsPointDown = false;
            m_IsPress = false;
            m_IsLongPress = false;
            m_LongClickCount = 0;
            m_CurrLongIntervalTime = 0;
            onUp?.Invoke(gameObject, eventData);
            if (IsDragLeftAndRight)
            {
                //说明是左右拖动判断
                m_DragEndPos = eventData.position;
                onDragLeftAndRight.Invoke(gameObject, eventData);
            }
            
        }

        public static EventTriggerListener Get(GameObject go)
        {
            if (go == null)
                return null;
            EventTriggerListener eventTrigger = go.GetComponent<EventTriggerListener>();
            if (eventTrigger == null) eventTrigger = go.AddComponent<EventTriggerListener>();
            return eventTrigger;
        }

        private const float DOUBLE_CLICK_TIME = 0.001f;
        private const float LONGPRESS_CLICK_TIME = 0.1f;
        private const float LONGPRESS_IntervalCLICK_TIME = 1f;//长按之间的单点击到长按间隔不是真实秒数随帧率变化
        private const float PRESS_TIME = 0.5F;
        private const float CONTINUOUS_PRESS_TIME = 0.3F;
        private float CONTINUOUS_PRESS_CD;

        private Vector2 m_DragStartPos;
        private Vector2 m_DragEndPos;
        private bool m_IsDragLeftAndRight;
        private float m_CurrDownTime;
        private float m_CurrLongDownTime;
        private float m_CurrLongIntervalTime;
        private int m_LongClickCount;
        private bool m_IsPointDown;
        private bool m_IsPress;
        private int m_ClickCount;
        private bool m_IsLongPress;
        private float m_ContinuousPressCD;

        public Vector2 DragStartPos
        {
            get { return m_DragStartPos; }
        }
        public Vector2 DragEndPos
        {
            get { return m_DragEndPos; }
        }
        public bool IsPress
        {
            get { return m_IsPress; }
        }
        public bool IsLongPress
        {
            get { return m_IsLongPress; }
        }
        public bool IsDragLeftAndRight
        {
            set { m_IsDragLeftAndRight = value; }
            get { return m_IsDragLeftAndRight;}
        }

        private void Update()
        {
            if(IsPress){
                m_ContinuousPressCD+=Time.deltaTime;
                if(m_ContinuousPressCD>=CONTINUOUS_PRESS_TIME){
                    m_ContinuousPressCD=.0f;
                    CONTINUOUS_PRESS_CD -= Time.deltaTime;
                    onContinuousPress?.Invoke(gameObject,null);
                }
            }
            if (m_IsLongPress)
            {
                m_CurrLongIntervalTime += Time.deltaTime;
                if (m_LongClickCount == 0&& Time.unscaledTime - m_ClickTime >= DOUBLE_CLICK_TIME)
                {
                    onLongPress?.Invoke(gameObject, null);
                    m_LongClickCount++;
                }
                else
                {
                    //正在长按
                    m_CurrLongDownTime += Time.deltaTime;
                    if (m_CurrLongDownTime >= LONGPRESS_CLICK_TIME)
                    {
                        m_CurrLongDownTime = 0f;
                        if (m_LongClickCount > 0&& m_CurrLongIntervalTime >= LONGPRESS_IntervalCLICK_TIME)
                        {
                            onLongPress?.Invoke(gameObject, null);
                        }

                    }
                }
                
                
            }
            if (m_IsPointDown)
            {
                if (Time.unscaledTime - m_CurrDownTime >= PRESS_TIME)
                {
                    m_IsPress = true;
                    CONTINUOUS_PRESS_CD = .0f;
                    m_IsPointDown = false;
                    m_CurrDownTime = 0f;
                    onPress?.Invoke(gameObject, null);
                }
            }
            if (m_ClickCount>0)
            {
                if (Time.unscaledTime - m_ClickTime >= DOUBLE_CLICK_TIME)
                {
                    if (m_ClickCount < 2)
                    {
                        onClick?.Invoke(gameObject, m_OnClickEventData);
                        m_OnClickEventData = null;
                    }

                    m_ClickCount = 0;
                }

                if (m_ClickCount > 1)
                {
                    onDoubleClick?.Invoke(gameObject, m_OnClickEventData);
                    m_OnClickEventData = null;
                    m_ClickCount = 0;
                }
            }            
        }

        private void OnDestroy()
        {
            RemoveUIListener();
        }

        public void RemoveUIListener()
        {
            onClick.RemoveAllListener();
            onDoubleClick.RemoveAllListener();
            onDown.RemoveAllListener();
            onEnter.RemoveAllListener();
            onExit.RemoveAllListener();
            onUp.RemoveAllListener();
            onPress.RemoveAllListener();
            onDragLeftAndRight.RemoveAllListener();
        }
    }
}