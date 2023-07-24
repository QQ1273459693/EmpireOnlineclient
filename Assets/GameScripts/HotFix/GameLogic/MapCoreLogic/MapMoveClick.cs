using PolyNav;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TEngine;
using GabrielBigardi.SpriteAnimator;
using Spine;

namespace GameLogic
{
    [RequireComponent(typeof(PolyNavAgent))]
    public class MapMoveClick : MonoBehaviour
    {
        private PolyNavAgent _agent;
        private PolyNavAgent agent
        {
            get { return _agent != null ? _agent : _agent = GetComponent<PolyNavAgent>(); }
        }
        public SpriteAnimator animator;
        float m_LastPosX;
        Vector2 m_CurClickMousPos;
        public Transform camrme;
        void OnEnable()
        {
            agent.OnDestinationReached += OnDestinationReached;
            agent.OnNavigationStarted += OnDestinationStart;
        }

        void OnDisable()
        {
            agent.OnDestinationReached -= OnDestinationReached;
            agent.OnNavigationStarted -= OnDestinationStart;
        }
        private void Awake()
        {
            Application.targetFrameRate = 60;
            m_LastPosX = transform.position.x;
        }
        /// <summary>
        /// Ѱ·��������Ŀ�ĵ�
        /// </summary>
        void OnDestinationReached()
        {
            animator.Play("MoveEnd").OnComplete(() =>
            {
                animator.Play("Idle");
            });
            Debug.Log("Click Destination Reached");
        }
        /// <summary>
        /// ��ʼѰ·
        /// </summary>
        void OnDestinationStart()
        {
            Debug.Log("����Yֵ:"+ transform.localRotation);
            //int MoveIndex = 0;//0������ϴ�λ���ޱ仯
            bool CanPlay = false;
            if (m_CurClickMousPos.x > transform.position.x&& transform.localRotation.y==1)
            {
                CanPlay = true;
            }else if (m_CurClickMousPos.x < transform.position.x && transform.localRotation.y == 0)
            {
                CanPlay = true;
            }
            if (CanPlay)
            {
                
                animator.PlayIfNotPlaying("Trun").OnComplete(() =>
                {
                    if (m_CurClickMousPos.x > transform.position.x)
                    {
                        transform.localRotation = new Quaternion(0, 0, 0, 0);
                    }
                    else if (m_CurClickMousPos.x < transform.position.x)
                    {
                        transform.localRotation = new Quaternion(0, 180F, 0, 0);
                    }


                    if (!animator.AnimationPlaying("Moveing"))
                    {
                        //����������ƶ���
                        animator.Play("Moveing");
                    }
                });
            }
            else
            {
                if (!animator.AnimationPlaying("Moveing"))
                {
                    animator.Play("BegingMove").OnComplete(() =>
                    {
                        animator.Play("Moveing");
                    });
                    //����������ƶ���
                    
                }
            }

            //if (m_CurClickMousPos.x> transform.position.x)
            //{
            //    transform.localRotation = new Quaternion(0,0,0,0);
            //}else if (m_CurClickMousPos.x < transform.position.x)
            //{
            //    transform.localRotation = new Quaternion(0, 180F, 0, 0);
            //}


            
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                m_CurClickMousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //Debug.Log($"�����λ��:{Camera.main.ScreenToWorldPoint(Input.mousePosition)}");
                agent.SetDestination(m_CurClickMousPos);
            }
        }
        private void LateUpdate()
        {
            camrme.position = transform.position;
        }
    }
}
