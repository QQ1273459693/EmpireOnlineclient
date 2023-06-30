using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HeroHUDComponent : MonoBehaviour
{
    private HeroRender mHeroRender;
    public Queue<BuffConfig> buffQueue = new Queue<BuffConfig>();//Buff 队列 用来处理buff名称的有序提示
    public List<BuffIconItem> buffItemList = new List<BuffIconItem>();
    public Slider hpSlider;
    public Slider hpDamageAnimSlider;
    public Slider angerSlider;
    public Transform buffParent;
    public float lastDequeTime;
    public void Init(HeroRender heroRender)
    {
        mHeroRender = heroRender;
    }
    public void Update()
    {
        if (buffQueue.Count > 0&& Time.realtimeSinceStartup-lastDequeTime>0.15f)
        {
            return;
            BuffConfig buffcfg = buffQueue.Dequeue();
            BuffTextItem buffText = null;//ResourcesManager.Instance.LoadObject<BuffTextItem>("Prefabs/HUD/" + (buffcfg.buffType == BuffType.DeBuff ? "DeBuffItemText" : "BuffItemText"), transform);
            Vector2 pos = buffText.transform.localPosition;
            pos.y -= 100;
            buffText.transform.localPosition = pos;
            buffText.PlayAnim(buffcfg);
            lastDequeTime = Time.realtimeSinceStartup;
        }
    }
    /// <summary>
    /// 更新血量进度条
    /// </summary>
    /// <param name="value"></param>
    public void UpdateHPSlider(float value)
    {
        hpSlider.value = value;
        hpDamageAnimSlider.DOValue(value, 0.5f).SetDelay(0.4f);
        if (value <= 0)
        {
            gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 更新怒气值进度条
    /// </summary>
    /// <param name="value"></param>
    public void UpdateAngerSlider(float value)
    {
        angerSlider.value = value;
        angerSlider.gameObject.SetActive(value != 0);
    }
    /// <summary>
    /// 添加buff 图标
    /// </summary>
    /// <param name="buffcfg"></param>
    public void AddBuffIcon(BuffConfig buffcfg)
    {
        return;
        BuffIconItem item = null;//ResourcesManager.Instance.LoadObject<BuffIconItem>("Prefabs/HUD/BuffIconItem", buffParent);
        item.bufficon.sprite = buffcfg.buffIcon;
        buffItemList.Add(item);
        buffQueue.Enqueue(buffcfg);
    }
    /// <summary>
    /// 移除buff图标
    /// </summary>
    /// <param name="sprite"></param>
    public void RemoveBuffIcon(Sprite sprite)
    {
        for (int i = 0; i < buffItemList.Count; i++)
        {
            if (buffItemList[i].bufficon.sprite == sprite)
            {
                GameObject.Destroy(buffItemList[i].gameObject);
                buffItemList.Remove(buffItemList[i]);

                break;
            }
        }
    }
    public void Release()
    {
        for (int i = 0; i < buffItemList.Count; i++)
        {
           GameObject.Destroy(buffItemList[i].gameObject);
           buffItemList.Remove(buffItemList[i]);
        }
        Destroy(gameObject);
    }
}
