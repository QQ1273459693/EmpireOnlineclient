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
        var Rect= GameModule.Resource.LoadAsset<GameObject>("common_DamageTips").GetComponent<RectTransform>();
        Rect.SetParent(BattleWorldNodes.Instance.DamageUHD);
        Rect.anchoredPosition = World3DToCanvasPos(spineAnimBox.GO.transform.position);
        Rect.gameObject.SetActive(true);
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
    /// <summary>
    /// 世界3D坐标转换为UGUI本地坐标
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    public Vector3 World3DToCanvasPos(Vector3 targetPos)
    {
        Vector2 uguiWorldPos;
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(BattleWorldNodes.Instance.Camera3D, targetPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(BattleWorldNodes.Instance.HUDWindowTrans as RectTransform, screenPos,
            BattleWorldNodes.Instance.UICamera, out uguiWorldPos);
        return uguiWorldPos;
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
