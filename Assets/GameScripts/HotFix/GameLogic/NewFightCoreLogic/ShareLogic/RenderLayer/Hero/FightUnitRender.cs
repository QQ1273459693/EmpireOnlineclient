#if CLIENT_LOGIC
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameLogic;
using TEngine;
using System;

public class FightUnitRender : RenderObject
{
    public FightUnitData HeroData { get; private set; }
    public FightUnitTeamEnum heroTeam;
    //SpineAnimBox spineAnimBox;//骨骼模型
    FightUnitBox pixelHeroBox;//像素模型
    public Vector3 m_Vector3Pos;

    public void SetHeroData(FightUnitData heroData, FightUnitTeamEnum heroTeam)
    {
        this.HeroData = heroData;
        this.heroTeam = heroTeam;
        Initlizate();
    }
    
    public void Initlizate()
    {
        //注意 这里的GameObject是UI格子位置,非战斗单位gameobject
        pixelHeroBox= GameModule.ObjectPool.GetObjectPool<FightUnitBox>().Spawn();
        pixelHeroBox.Initialize(gameObject.transform);
        pixelHeroBox.RefreshData(HeroData.SeatId, heroTeam,HeroData.ResName,HeroData.Hp, HeroData.MaxHp);
        //gameObject = pixelHeroBox.GO;//这里才是真的战斗单位渲染OBJ
        m_Vector3Pos = gameObject.transform.position;
        //spineAnimBox = GameModule.ObjectPool.GetObjectPool<SpineAnimBox>().Spawn();
        //spineAnimBox.IntObj(gameObject);
        //spineAnimBox.RefreshData(ConfigLoader.Instance.Tables.TbEnemySpine.DataMap[HeroData.id].SpineResName, heroTeam == HeroTeamEnum.Self);
        //UpdateHP_HUD(0, HeroData.hp, 1);
    }
    public override void Update()
    {
        base.Update();
        //position = transform.position;


    }

    public void PlayAnim(string animName,Action AnimFinishAction)
    {
        pixelHeroBox.PlayAnim(animName, AnimFinishAction);
        //spineAnimBox.PlayAnim(animName);
    }
    public void PlayDeath()
    {
        pixelHeroBox.DeathAnim(() =>
        {
            Log.Info("隐藏的单位是:"+ gameObject.name);
            gameObject.SetActive(false);
        });
        //spineAnimBox.PlayAnim(animName);
    }
    public void PlayUITips(int Type)
    {
        pixelHeroBox.ShowUIAction(Type);
    }
    public void NewPlayAnim(string animName,Action DamageAction)
    {
        pixelHeroBox.PlayAnim(animName,DamageAction);
    }
    public void FightAnimMovePos(Vector3 TargetPos,Action MoveFinish)
    {
        pixelHeroBox.FightAnimMovePos(TargetPos, MoveFinish);
    }
    public void SetAnimState(AnimState state)
    {
        //mAnimator.speed = state == AnimState.StopAnim ? 0 : 1;
    }

    public void UpdateHP_HUD(int damage, float hpRateValue, float Hpfill,BuffConfig buffCfg=null)
    {
        pixelHeroBox.UpdateHp_Hud(damage, hpRateValue);
        //spineAnimBox.UpdateHp_Hud(damage, hpRateValue);

        //生成Buff伤害提示
        //if (buffCfg != null)
        //{
        //    BuffTextItem buffDamageTex = null;//ResourcesManager.Instance.LoadObject<BuffTextItem>("Prefabs/HUD/DeBuffItemText", BattleWordNodes.Instance.HUDWindow);
        //    //buffDamageTex.transform.localPosition = new Vector3(pos.x, pos.y  );
        //    //buffDamageTex.transform.localScale = Vector3.one;
        //    //buffDamageTex.PlaybuffDamageAnim(buffCfg);
        //}
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
        pixelHeroBox.UpdateHp_Hud(0,0);
        //spineAnimBox.UpdateHp_Hud(0, 0);
        //m_HpFill.DOFillAmount(0, 0.45F).OnComplete(() =>
        //{
        //    m_HpFill.transform.parent.gameObject.SetActive(false);
        //    gameObject.SetActive(false);
        //});
    }

    public override void OnRelease()
    {
        base.OnRelease();
        pixelHeroBox.OnUnspawn();
        //spineAnimBox.OnUnspawn();
    }
}
#endif