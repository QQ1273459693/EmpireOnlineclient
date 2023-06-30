#if CLIENT_LOGIC
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroRender : RenderObject
{
 
    public Animator mAnimator;
    private HeroHUDComponent mHeroHUD;
    public Transform mHUDParent;
    public HeroTeamEnum heroTeam;
    public Vector3 position;
    public void SetHeroData(HeroTeamEnum heroTeam)
    {
        this.heroTeam = heroTeam;
        Initlizate();
    }
    
    public void Initlizate()
    {
        mHeroHUD = null;//ResourcesManager.Instance.LoadObject<HeroHUDComponent>("Prefabs/HUD/HPObject" + heroTeam.ToString(), BattleWordNodes.Instance.HUDWindow);
        mHeroHUD.Init(this);
    }
    public override void Update()
    {
        base.Update();
        UpdateHeroHUD();
        position = transform.position;


    }

    public void PlayAnim(string animName)
    {
        mAnimator.SetTrigger(animName);
    }
    public void SetAnimState(AnimState state)
    {
        mAnimator.speed = state == AnimState.StopAnim ? 0 : 1;
    }

    public void UpdateHeroHUD()
    {
        if (mHeroHUD != null && LogicObject != null && mHUDParent != null)
        {
            mHeroHUD.transform.localPosition = World3DToCanvasPos(mHUDParent.position);
        }
    }

    public void UpdateHP_HUD(int damage, float hpRateValue, BuffConfig buffCfg=null)
    {
       
        GameObject damageTextObj = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(damage > 0 ? "Prefabs/HUD/DamageText" : "Prefabs/HUD/RestoreHPText"), BattleWordNodes.Instance.HUDWindow);
        Vector2 pos = World3DToCanvasPos(transform.position);
        damageTextObj.transform.localPosition = new Vector3(pos.x, pos.y + 40);
        damageTextObj.transform.localScale = Vector3.one;

        damageTextObj.GetComponent<Text>().text = (damage > 0 ? "-" : "+") + Mathf.Abs(damage);
        damageTextObj.transform.DOLocalMoveY(damageTextObj.transform.localPosition.y + 100, 1f);
        damageTextObj.GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetDelay(1.2f);
        mHeroHUD.UpdateHPSlider(hpRateValue);
        Destroy(damageTextObj, 3);

        //生成Buff伤害提示
        if (buffCfg != null)
        {
            BuffTextItem buffDamageTex = null;//ResourcesManager.Instance.LoadObject<BuffTextItem>("Prefabs/HUD/DeBuffItemText", BattleWordNodes.Instance.HUDWindow);
            buffDamageTex.transform.localPosition = new Vector3(pos.x, pos.y  );
            buffDamageTex.transform.localScale = Vector3.one;
            buffDamageTex.PlaybuffDamageAnim(buffCfg);
        }
    }
    public void UpdateAnger_HUD( float rageRateValue)
    {
        if (mHeroHUD!=null)
        {
            mHeroHUD.UpdateAngerSlider(rageRateValue);
        }

    }
    public void Add_BuffIcon(BuffConfig buffcfg)
    {
        mHeroHUD.AddBuffIcon(buffcfg);
    }
    public void Remove_BuffIcon(Sprite sprite)
    {
        mHeroHUD.RemoveBuffIcon(sprite);
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
        PlayAnim("Death");
        mHeroHUD.gameObject.SetActive(false);
        AnimatorStateInfo info = mAnimator.GetCurrentAnimatorStateInfo(0);
        LogicTimeManager.Instance.DelayCall((VInt)(info.length + 1), () =>
        {
            gameObject.SetActive(false);
        });

    }

    public override void OnRelease()
    {
        mHeroHUD.Release();
        base.OnRelease();
    }
}
#endif