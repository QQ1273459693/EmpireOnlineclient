#if CLIENT_LOGIC
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameLogic;
using UnityEditor.Experimental.GraphView;

public class HeroRender : RenderObject
{
    public HeroData HeroData { get; private set; }
    public HeroTeamEnum heroTeam;
    SpineAnimBox spineAnimBox;
    Image m_HpFill;

    public void SetHeroData(HeroData heroData, HeroTeamEnum heroTeam)
    {
        this.HeroData = heroData;
        this.heroTeam = heroTeam;
        Initlizate();
    }
    
    public void Initlizate()
    {
        spineAnimBox = GameModule.ObjectPool.GetObjectPool<SpineAnimBox>().Spawn();
        spineAnimBox.IntObj(gameObject);
        spineAnimBox.RefreshData(ConfigLoader.Instance.Tables.TbEnemySpine.DataMap[HeroData.id].SpineResName, heroTeam == HeroTeamEnum.Self);
        UpdateHP_HUD(0, HeroData.hp, 1);
    }
    public override void Update()
    {
        base.Update();
        //position = transform.position;


    }

    public void PlayAnim(string animName)
    {
        spineAnimBox.PlayAnim(animName);
    }
    public void SetAnimState(AnimState state)
    {
        //mAnimator.speed = state == AnimState.StopAnim ? 0 : 1;
    }

    public void UpdateHP_HUD(int damage, float hpRateValue, float Hpfill,BuffConfig buffCfg=null)
    {
        if (damage != 0)
        {
            var DamageBox = GameModule.ObjectPool.GetObjectPool<HudDamageTipsPool>().Spawn();
            DamageBox.Init();
            //Debuger.LogError($"看下物体名称:{spineAnimBox.GO.name},目标位置:{spineAnimBox.m_HudPos}");
            DamageBox.AdjustPos(damage, spineAnimBox.m_HudPos, buffCfg);
            m_HpFill.DOFillAmount(Hpfill, 0.45F);
        }
        else
        {
            //第一次加载HUD
            Vector3 tmpVec31 = RectTransformUtility.WorldToScreenPoint(BattleWordNodes.Instance.Camera3D, spineAnimBox.m_HudPos);

            RectTransform HpObj = heroTeam == HeroTeamEnum.Self ? BattleWordNodes.Instance.heroUIHpFill[HeroData.seatid] : BattleWordNodes.Instance.enemyUIHpFill[HeroData.seatid];

            m_HpFill = HpObj.transform.Find("Image").GetComponent<Image>();
            m_HpFill.fillAmount = 1;
            HpObj.gameObject.SetActive(true);

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(HpObj, tmpVec31, BattleWordNodes.Instance.UiCamera, out tmpVec31))
            {
                HpObj.transform.position = tmpVec31;
            }
        }

        spineAnimBox.UpdateHp_Hud(damage, hpRateValue);

        //生成Buff伤害提示
        if (buffCfg != null)
        {
            BuffTextItem buffDamageTex = null;//ResourcesManager.Instance.LoadObject<BuffTextItem>("Prefabs/HUD/DeBuffItemText", BattleWordNodes.Instance.HUDWindow);
            //buffDamageTex.transform.localPosition = new Vector3(pos.x, pos.y  );
            //buffDamageTex.transform.localScale = Vector3.one;
            //buffDamageTex.PlaybuffDamageAnim(buffCfg);
        }
    }
    public void Add_BuffIcon(BuffConfig buffcfg)
    {
        //mHeroHUD.AddBuffIcon(buffcfg);
    }
    public void Remove_BuffIcon(Sprite sprite)
    {
        //mHeroHUD.RemoveBuffIcon(sprite);
    }
    public Vector2 World3DToCanvasPos(Vector3 target3DPos)
    {
        Vector2 uguiWorldPos;
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(BattleWordNodes.Instance.Camera3D, target3DPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(BattleWordNodes.Instance.CanvasRect, screenPos, BattleWordNodes.Instance.UiCamera, out uguiWorldPos);
        return uguiWorldPos;
    }

    public void HeroDeath()
    {
        spineAnimBox.UpdateHp_Hud(0, 0);
        m_HpFill.DOFillAmount(0, 0.45F).OnComplete(() =>
        {
            m_HpFill.transform.parent.gameObject.SetActive(false);
            gameObject.SetActive(false);
        });

    }

    public override void OnRelease()
    {
        base.OnRelease();
        spineAnimBox.OnUnspawn();
    }
}
#endif