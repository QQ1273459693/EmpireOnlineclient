using System;
using System.Collections;
using System.Collections.Generic;
using TEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GabrielBigardi.SpriteAnimator;
using Spine;

namespace GameLogic
{
    public class PixelHeroBox : IPoolObject
    {
        public GameObject GO;

        //非UIHUD组件
        TextMeshPro HpText;
        Transform AttackUI;
        SpriteRenderer AttackUiSprite;

        //数据组件
        SpriteAnimator animator;

        public Vector3 m_HudPos;
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
            GO = GameModule.Resource.LoadAsset<GameObject>("PixelHeroRoot");
            animator = GO.GetComponent<SpriteAnimator>();
            m_HudPos = new Vector3(parntObj.position.x, parntObj.position.y-1);
            GO.transform.SetParent(parntObj,false);
        }
        /// <summary>
        /// 刷新数据
        /// </summary>
        public void RefreshData(string SpineResName, bool isFlii)
        {
            SpriteAnimationObject spriteAnimation = GameModule.Resource.LoadAsset<SpriteAnimationObject>(SpineResName);
            animator.ChangeAnimationObject(spriteAnimation);
            animator.enabled = true;
            GO.transform.localRotation = new Quaternion(0, isFlii ? 0 : 180, 0, 0);
            HpText = GO.transform.Find("HpText").GetComponent<TextMeshPro>();
            AttackUI = GO.transform.Find("AttackTipsPos/AttackUITips").transform;
            AttackUiSprite = AttackUI.GetComponent<SpriteRenderer>();
            AttackUI.gameObject.SetActive(false);
        }
        public void PlayAnim(string AnimName, bool isLoop = false)
        {
            if (AnimName == "BeAttack"|| animator==null)
            {
                return;
            }
            Log.Debug("动画名:"+ AnimName);
            if (AnimName!="Idle")
            {
                //则全部返回Idle动画
                animator.Play(AnimName).OnAnimationComplete+= IdleLoopPlay;
            }
            else
            {

                animator.Play(AnimName);
            }
            
            AttackUI.gameObject.SetActive(true);
            AttackUiSprite.color = Color.white;
            AttackUiSprite.DOBlendableColor(Color.clear,1F);
            AttackUI.DOLocalJump(new Vector3(0, 3.5F), 2, 1, 0.9F).OnComplete(() =>
            {
                AttackUI.gameObject.SetActive(false);
            });
        }
        void IdleLoopPlay()
        {
            animator.Play("Idle");
        }
        /// <summary>
        /// 更新Hp相关HUD显示
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="hpRateValue"></param>
        public void UpdateHp_Hud(int damage, float hpRateValue)
        {
            //DamageText.text = (damage > 0 ? "-" : "+") + Mathf.Abs(damage);
            HpText.text = $"Hp:{hpRateValue}";
        }
        public void OnSpawn()
        {
            
        }

        public void OnUnspawn()
        {
           
        }
    }
}
