using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GameLogic;
using TMPro;

public class HeroRender : RenderObject
{
    public HeroData HeroData { get; private set; }
    public HeroTeamEnum HeroTeam { get; private set; }

    SpineAnimBox spineAnimBox;
    public void SetHeroData(HeroData heroData,HeroTeamEnum heroTeam)
    {
        HeroData = heroData;
        HeroTeam = heroTeam;
        InitIizate();
        UpdateHp_Hud(0,heroData.hp);
    }
    public void InitIizate()
    {
        spineAnimBox= GameModule.ObjectPool.GetObjectPool<SpineAnimBox>().Spawn();
        spineAnimBox.IntObj(gameObject);
        spineAnimBox.RefreshData(ConfigLoader.Instance.Tables.TbEnemySpine.DataMap[HeroData.id].SpineResName,HeroTeam== HeroTeamEnum.Self);
    }
    public void HeroDeath()
    {
        //PlayAnim("Death");
        gameObject.SetActive(false);
    }
    public void PlayAnim(string Anim)
    {
        spineAnimBox.PlayAnim(Anim);
    }
    public void UpdateHp_Hud(int damage, float hpRateValue)
    {
        spineAnimBox.UpdateHp_Hud(damage, hpRateValue);
    }
    /// <summary>
    /// 更新英雄怒气值
    /// </summary>
    public void UpdateAnger_HUD(float rate)
    {
        //if (mHeroHUD!=null)
        //{
        //    mHeroHUD.UpdateAngerSlider(rate);
        //}
    }
    public override void Update()
    {
        base.Update();
    }
    public override void OnRelease()
    {
        base.OnRelease();
        spineAnimBox.OnUnspawn();
    }
}
