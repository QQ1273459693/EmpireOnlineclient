using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HeroHUDComponet : MonoBehaviour
{
    private HeroRender mHeroRender;
    public Slider hpSlider;

    public Slider hpDamageAnimSlider;//血量过度动画Slider
    public Slider angerSlider;//血量过度动画slider
    public Transform buffParent;//怒气值slider
  
    public void Init(HeroRender heroRendr)
    {
        mHeroRender = heroRendr;
    }
    public void UpdateHPSlider(float value)
    {
        hpSlider.value = value;
        hpDamageAnimSlider.DOValue(value,0.5F).SetDelay(0.4F);
        if (value<=0)
        {
            gameObject.SetActive(false);
        }
    }
    public void UpdateAngerSlider(float value)
    {
        angerSlider.value = value;
        angerSlider.gameObject.SetActive(value!=0);
    }
    public void Release()
    {

    }
}
