using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;
using DG.Tweening;

namespace GameLogic
{
    public class HudDamageTipsPool : IPoolObject
    {
        const string RES_NAME = "common_DamageTips";


        protected CancellationTokenSource m_CancellationTokenSource;

        GameObject m_HudObj;
        RectTransform mRectTrans;
        AssetOperationHandle handle;
        TMP_Text m_DamageText;

        public long Id { get; set ; }
        public bool IsFromPool { get ; set ; }

        public void Init()
        {
            m_HudObj = GameModule.Resource.LoadAsset<GameObject>(RES_NAME,out handle);
            mRectTrans = m_HudObj.GetComponent<RectTransform>();
            m_DamageText= m_HudObj.GetComponent<TMP_Text>();
            m_HudObj.transform.SetParent(BattleWordNodes.Instance.DamageUHD);
            m_HudObj.SetActive(false);
            m_HudObj.transform.localPosition = Vector3.zero;
            m_HudObj.transform.localRotation = Quaternion.identity;
            m_HudObj.transform.localScale = Vector3.one;
           
        }

        public void OnInit()
        {
            
        }

        public void OnRelease()
        {

        }
        public void AdjustPos(int Value,Vector3 TargetPos, BuffConfig buff)
        {
            int TopDoValue = 100;
            string BuffSr = "";
            if (buff!=null)
            {
                BuffSr = buff.buffName;
                m_DamageText.color = Color.green;
            }
            else
            {
                TopDoValue = 140;
                m_DamageText.color = Color.red;
            }
            m_DamageText.text = BuffSr+(Value > 0 ? "-" : "+") + Mathf.Abs(Value);
            m_HudObj.SetActive(true);
            Vector3 tmpVec3 = RectTransformUtility.WorldToScreenPoint(BattleWordNodes.Instance.Camera3D, TargetPos);

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(mRectTrans, tmpVec3, BattleWordNodes.Instance.UiCamera, out tmpVec3))
            {
                m_HudObj.transform.position = tmpVec3;
                mRectTrans.DOAnchorPosY(mRectTrans.anchoredPosition.y + TopDoValue, 0.75F).OnComplete(() =>
                {
                    m_HudObj.SetActive(false);
                });
            }
        }
        public void OnSpawn()
        {
            
        }

        public void OnUnspawn()
        {
            m_CancellationTokenSource.Cancel();
            m_CancellationTokenSource.Dispose();
            m_CancellationTokenSource = null;
            handle?.Release();

        }
    }
}
