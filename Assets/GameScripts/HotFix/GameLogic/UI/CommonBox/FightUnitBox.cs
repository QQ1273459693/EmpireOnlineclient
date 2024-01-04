using System;
using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GabrielBigardi.SpriteAnimator;
using YooAsset;
using UniFramework.Pooling;
using Spine;

namespace GameLogic
{
    public class FightUnitBox : IPoolObject
    {
        public GameObject GO;
        Transform m_FightTrs;
        //RectTransform m_FightRect;
        Vector3 m_Position;
        //非UIHUD组件
        Image AttackUiTips;
        Image SkillUiTips;
        Image EvadeUiTips;
        Text m_DamageText;
        Text m_HpText;
        //数据组件
        Image HpImg;
        AssetOperationHandle FightUnitHandle;
        AssetOperationHandle SpriteAnimationHandle;
        SpriteAnimator animator;
        Action m_Action;
        public Vector3 m_HudPos;

        //战斗单位数据
        int SeatID;
        int m_MAXHP;
        FightUnitTeamEnum HeroTeam;
        public long Id { get; set; }
        public bool IsFromPool { get; set; }

        public void OnInit()
        {

        }

        public void OnRelease()
        {

        }
        public void Initialize(Transform parntObj)
        {
            FightUnitHandle = GameModule.Resource.LoadAssetGetOperation<GameObject>("Fight_FightUIUnit");
            GO=FightUnitHandle.InstantiateSync(FightRoundWindow.Instance.FightUIRoot);
            if (GO==null)
            {
                Log.Error("出错!");
            }


            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, parntObj.position);
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(FightRoundWindow.Instance.Canvas, screenPoint, FightRoundWindow.Instance.UiCamera, out localPoint))
            {
                GO.GetComponent<RectTransform>().anchoredPosition = localPoint ;
            }


            m_FightTrs = parntObj.transform.Find("Animation");
            //m_FightRect = m_FightTrs.GetComponent<RectTransform>();
            m_Position = parntObj.position;
            animator = m_FightTrs.GetComponent<SpriteAnimator>();
            HpImg = GO.transform.Find("HP/Image").GetComponent<Image>();
            m_HpText= GO.transform.Find("HP/Image/Num").GetComponent<Text>();
            AttackUiTips = GO.transform.Find("AttackTips").GetComponent<Image>();
            SkillUiTips = GO.transform.Find("SkillUITips").GetComponent<Image>();
            EvadeUiTips = GO.transform.Find("EvadeUITips").GetComponent<Image>();
            m_DamageText = GO.transform.Find("DamageText").GetComponent<Text>();
        }
        /// <summary>
        /// 刷新数据
        /// </summary>
        public void RefreshData(int m_SeatID,FightUnitTeamEnum heroTeam,string SpineResName,int CurHP,int MAXHP)
        {
            HeroTeam = heroTeam;
            SeatID = HeroTeam== FightUnitTeamEnum.Self? m_SeatID:5+ m_SeatID;//这里座位按照0-9来算
            m_MAXHP =MAXHP;
            HpImg.fillAmount=(float)CurHP/(float)MAXHP;
            SpriteAnimationHandle = GameModule.Resource.LoadAssetGetOperation<SpriteAnimationObject>(SpineResName);
            SpriteAnimationObject spriteAnimation = SpriteAnimationHandle.AssetObject as SpriteAnimationObject;
            animator.ChangeAnimationObject(spriteAnimation);
            animator.enabled = true;
            //m_FightTrs.localRotation = new Quaternion(0, isFlii ? 180 : 0, 0, 0);
        }
        public void ShowUIAction(int ActionType)
        {
            switch (ActionType)
            {
                case 1://攻击
                    AttackUiTips.color = Color.white;
                    AttackUiTips.rectTransform.anchoredPosition = new Vector2(0, 0);
                    AttackUiTips.rectTransform.DOAnchorPos3DY(100, 0.75F).OnComplete(() =>
                    {
                        AttackUiTips.color = Color.clear;
                    });
                    break;
                case 2://技能
                    SkillUiTips.color = Color.white;
                    SkillUiTips.rectTransform.anchoredPosition = new Vector2(0, 0);
                    SkillUiTips.rectTransform.DOAnchorPos3DY(100, 0.75F).OnComplete(() =>
                    {
                        SkillUiTips.color = Color.clear;
                    });
                    break;
                case 3://闪避
                    EvadeUiTips.color = Color.white;
                    EvadeUiTips.rectTransform.anchoredPosition = new Vector2(0, 0);
                    EvadeUiTips.rectTransform.DOAnchorPos3DY(100, 0.75F).OnComplete(() =>
                    {
                        EvadeUiTips.color = Color.clear;
                    });
                    break;
                case 4://逃跑
                    break;
            }
        }
        public void FightAnimMovePos(Vector3 TargetPos,Action MoveAction)
        {
            if (TargetPos==Vector3.zero)
            {
                //说明不进行位移,仅仅释放技能攻击
                animator.OnKeyFrameComplete(() =>
                {
                    MoveAction?.Invoke();
                });
                animator.Play("Attack").OnAnimationComplete += (() =>
                {
                    animator.Play("Idle");

                });
            }
            else
            {
                animator.Play("Move");
                m_FightTrs.DOMove(TargetPos, 0.5F).OnComplete(() =>
                {
                    animator.OnKeyFrameComplete(() =>
                    {
                        MoveAction?.Invoke();
                    });
                    animator.Play("Attack").OnAnimationComplete += (() =>
                    {
                        animator.Play("Move");
                        m_FightTrs.DOMove(m_Position, 0.45F).OnComplete(() =>
                        {
                            animator.Play("Idle");
                        });

                    });
                });
            }
            
        }
        public void PlayAnim(string AnimName,Action DamageAction)
        {
            if (AnimName == "BeAttack"|| animator==null)
            {
                return;
            }
            animator.OnKeyFrameComplete(() =>
            {
                DamageAction?.Invoke();
            });
            animator.Play(AnimName).OnComplete(() =>
            {
                animator.Play("Idle");
            });
            //m_FightImg.SetNativeSize();
            m_Action = DamageAction;
        }
        public void DeathAnim(Action DamageAction)
        {
            if (animator == null)
            {
                return;
            }
            animator.Play("Death").OnComplete(() =>
            {
                DamageAction?.Invoke();
                GO.SetActive(false);
            });
        }
        void IdleLoopPlay()
        {
            animator.Play("Idle");
        }
        void AttackBackIdle()
        {
            //m_Action?.Invoke();
            //animator.Play("Move");
            //m_HeroTrs.DOMove(m_Position, 0.5F).SetEase(Ease.Linear).OnComplete(() =>
            //{
            //    animator.Play("Idle");
            //});

        }
        /// <summary>
        /// 更新Hp相关HUD显示
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="hpRateValue"></param>
        public void UpdateHp_Hud(int damage, float hpRateValue)
        {
            m_HpText.text = $"{hpRateValue}/{m_MAXHP}";
            m_DamageText.text = (damage > 0 ? "-" : "+") + Mathf.Abs(damage);
            HpImg.fillAmount = (float)hpRateValue / (float)m_MAXHP;
            HUD_DamageTextHelp.GenerHUDText(SeatID,2, damage);
        }
        public void OnSpawn()
        {
            
        }

        public void OnUnspawn()
        {
            FightUnitHandle?.Release();
            SpriteAnimationHandle?.Release();
        }
    }
}
