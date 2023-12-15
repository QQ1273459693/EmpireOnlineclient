using System;
using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GabrielBigardi.SpriteAnimator;
using YooAsset;
using System.Net.NetworkInformation;
using Codice.Client.BaseCommands;
using UniFramework.Pooling;

namespace GameLogic
{
    public class FightUnitBox : IPoolObject
    {
        public GameObject GO;
        Transform m_FightTrs;
        Vector3 m_Position;
        //��UIHUD���
        Image AttackUiTips;
        Image SkillUiTips;
        Text m_DamageText;
        Text m_HpText;
        //�������
        Image HpImg;
        AssetOperationHandle FightUnitHandle;
        AssetOperationHandle SpriteAnimationHandle;
        SpriteAnimator animator;
        Action m_Action;
        public Vector3 m_HudPos;

        //ս����λ����
        int m_MAXHP;
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
            FightUnitHandle = GameModule.Resource.LoadAssetGetOperation<GameObject>("Fight_FightUnit");
            GO = FightUnitHandle.InstantiateSync(parntObj);
            if (GO==null)
            {
                Log.Error("����!");
            }
            m_FightTrs = GO.transform.Find("FightRenderAnimation");
            m_Position = m_FightTrs.position;
            animator = m_FightTrs.GetComponent<SpriteAnimator>();
            HpImg = GO.transform.Find("HP/Image").GetComponent<Image>();
            m_HpText= GO.transform.Find("HP/Image/Num").GetComponent<Text>();
            AttackUiTips = GO.transform.Find("AttackTips").GetComponent<Image>();
            SkillUiTips = GO.transform.Find("SkillUITips").GetComponent<Image>();
            m_DamageText = GO.transform.Find("DamageText").GetComponent<Text>();
            AttackUiTips.gameObject.SetActive(false);
            SkillUiTips.gameObject.SetActive(false);
        }
        /// <summary>
        /// ˢ������
        /// </summary>
        public void RefreshData(string SpineResName, bool isFlii,int CurHP,int MAXHP)
        {
            m_MAXHP=MAXHP;
            HpImg.fillAmount=(float)CurHP/(float)MAXHP;
            Log.Debug("������Դ�ļ���:"+ SpineResName);
            SpriteAnimationHandle = GameModule.Resource.LoadAssetGetOperation<SpriteAnimationObject>(SpineResName);
            SpriteAnimationObject spriteAnimation = SpriteAnimationHandle.AssetObject as SpriteAnimationObject;
            animator.ChangeAnimationObject(spriteAnimation);
            animator.enabled = true;
            m_FightTrs.localRotation = new Quaternion(0, isFlii ? 180 : 0, 0, 0);
        }
        
        public void PlayAnim(string AnimName, Vector3 TargetPos,Action DamageAction,bool isLoop = false)
        {
            if (AnimName == "BeAttack"|| animator==null)
            {
                return;
            }

            m_Action = DamageAction;
            //animator.Play("Move");
            //�ƶ���Ŀ��λ�����ͷż�����,�ٻ���
            //m_HeroTrs.DOMove(TargetPos, 0.4F).SetEase(Ease.Linear).OnComplete(() =>
            //{

            //    animator.Play(AnimName).OnAnimationComplete+= AttackBackIdle;


            //    AttackUI.gameObject.SetActive(true);
            //    AttackUiSprite.color = Color.white;
            //    AttackUiSprite.DOBlendableColor(Color.clear, 1F);
            //    AttackUI.DOLocalJump(new Vector3(0, 3.5F), 2, 1, 0.9F).OnComplete(() =>
            //    {
            //        AttackUI.gameObject.SetActive(false);
            //    });
            //});


            //if (TargetPos!=null)
            //{
            //    animator.Play("Move");
            //    //�ƶ���Ŀ��λ�����ͷż�����,�ٻ���
            //    m_HeroTrs.DOMove(TargetPos, 1F).OnComplete(() =>
            //    {

            //        animator.Play(AnimName).OnAnimationComplete+= AttackBackIdle;

            //        AttackUI.gameObject.SetActive(true);
            //        AttackUiSprite.color = Color.white;
            //        AttackUiSprite.DOBlendableColor(Color.clear, 1F);
            //        AttackUI.DOLocalJump(new Vector3(0, 3.5F), 2, 1, 0.9F).OnComplete(() =>
            //        {
            //            AttackUI.gameObject.SetActive(false);
            //        });
            //    });
            //}
            //else
            //{
            //    Log.Debug("������:" + AnimName);
            //    if (AnimName != "Idle")
            //    {
            //        //��ȫ������Idle����
            //        animator.Play(AnimName).OnAnimationComplete += IdleLoopPlay;
            //    }
            //    else
            //    {

            //        animator.Play(AnimName);
            //    }

            //    AttackUI.gameObject.SetActive(true);
            //    AttackUiSprite.color = Color.white;
            //    AttackUiSprite.DOBlendableColor(Color.clear, 1F);
            //    AttackUI.DOLocalJump(new Vector3(0, 3.5F), 2, 1, 0.9F).OnComplete(() =>
            //    {
            //        AttackUI.gameObject.SetActive(false);
            //    });
            //}


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
        /// ����Hp���HUD��ʾ
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="hpRateValue"></param>
        public void UpdateHp_Hud(int damage, float hpRateValue)
        {
            m_HpText.text= hpRateValue.ToString();
            m_DamageText.text = (damage > 0 ? "-" : "+") + Mathf.Abs(damage);
            HpImg.fillAmount = (float)hpRateValue / (float)m_MAXHP;
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
