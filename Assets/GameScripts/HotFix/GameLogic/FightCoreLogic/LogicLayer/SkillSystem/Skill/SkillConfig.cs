using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if CLIENT_LOGIC
[CreateAssetMenu(fileName = "SkillConfig", menuName = "SkillConfig", order = 0)]
public class SkillConfig : ScriptableObject
#else
public class SkillConfig
#endif
{
    [HideInInspector]
    public  bool hideBullet = true;
    [HideInInspector]
    public bool hideDamagePercentage = true;



    [LabelText("技能图标"), LabelWidth(0.1f), PreviewField(70, ObjectFieldAlignment.Left), SuffixLabel("技能图标")]
    [JsonIgnore]
    public Sprite skillIcon=null;                 //技能图标

    [LabelText("技能ID") ]
    public int skillid;                     //技能ID //
    [LabelText("技能名称")]
    public string skillName;                //技能名称 


 
    [LabelText("技能所需怒气值"), ProgressBar(0, 150, 1f, 0.8f, 0)]
    public int needRageValuel = 100;        //技能所需怒气值
    [LabelText("技能前摇时间ms"),Tooltip("比如：英雄从当前位置移动到目标位置 或技能动画播放到某个位置 需要的时间")]
    public int skillShakeBeforeTimeMS;      //技能前摇时间  比如：英雄从当前位置移动到目标位置 或技能动画播放到某个位置 需要的时间
    [LabelText("技能攻击时间ms"), Tooltip("比如：英雄放了一个连击动画，这个攻击时长就表示连击攻击动画的时长，或子弹飞行的时间")]
    public int skillAttackDurationMS;       //技能攻击时间  比如：英雄放了一个连击动画，这个攻击时长就表示连击攻击动画的时长，或子弹飞行的时间
    [LabelText("技能后摇时间ms"), Tooltip("比如：英雄从目标位置回到原始位置 或技能动画从某个位置播放完成 需要的时间")]
    public int skillShakeAfterTimeMS;       //技能后摇时间  比如：英雄从目标位置回到原始位置 或技能动画从某个位置播放完成 需要的时间
    [LabelText("技能类型"), OnValueChanged("SkillAttackTypeChanged")]
    public SkillType skillType;             //技能类型   
    [LabelText("技能作用目标")]
    public RoleTargetType roleTragetType=RoleTargetType.Enemy;   //技能作用目标
    [LabelText("技能攻击类型")]
    public SkillAttackType skillAttackType; //技能攻击类型
    [LabelText("子弹"),HideIf("hideBullet")]
    public string bullet;                   //子弹
    [LabelText("伤害类型"), OnValueChanged("DamageTypeChanged")]
    public DamageType damageType;           //伤害类型
    [LabelText("伤害百分比"), HideIf("hideDamagePercentage"), ProgressBar(0, 500, 0.8f, 0, 0)]
    public int damagePercentage;            //伤害百分比

    [LabelText("技能动画"), TitleGroup("技能表现", "所有表现数据会在技能开始释放时触发")]
    public string skillAnim;                //技能动画
    [LabelText("技能音效"), TitleGroup("技能表现", "所有表现数据会在技能开始释放时触发")]
    [JsonIgnore]
    public AudioClip skillAudio;               //技能音效
    [LabelText("技能特效名称"), TitleGroup("技能表现", "所有表现数据会在技能开始释放时触发")]
    public string skillEffect;              //技能特效
    [LabelText("技能击中特效"), TitleGroup("技能表现", "所有表现数据会在技能开始释放时触发")]
    public string skillHitEffect;           //技能击中特效


    [/*Title("技能附加Buffs"),*/TitleGroup("附加Buff", "技能生效的一瞬间，附加目标指定的多个buff")]
    public int[] addBuffs;                  //添加的buff

    [Title("技能描述:"), MultiLineProperty(4),HideLabel]
    public string skillDes;                 //技能描述
    public   void SkillAttackTypeChanged(SkillType type)
    {
        Debuger.Log(type);
        hideBullet = type != SkillType.Ballistic;
    }
    public void DamageTypeChanged(DamageType type)
    {
        Debuger.Log(type);
        hideDamagePercentage =  type!= DamageType.AtkPercentage&&type != DamageType.HPPercentage;
    }
}

 
public enum DamageType  //伤害类型，所有伤害都是基于攻击力
{
    [LabelText("无伤害")] None,               //无伤害
    [LabelText("普通伤害")] NomalDamage,        //普通伤害
    [LabelText("真实伤害 (无视护盾、减伤)")] RealDamage,         //真实伤害 (无视护盾、减伤)
    [LabelText("攻击力百分比伤害")] AtkPercentage,      //攻击力百分比伤害
    [LabelText("生命值白分比伤害")] HPPercentage,       //生命值白分比伤害
}

public enum SkillType
{
    [LabelText("普攻型技能")] MoveToAttack,       //普攻型技能
    [LabelText("移动到敌人中心位置攻击")] MoveToEnemyConter,  //移动到敌人中心位置
    [LabelText("移动到中心位置攻击")] MoveToConter,       //移动到中心位置
    [LabelText("吟唱型技能")] Chant,              //吟唱型技能
    [LabelText("弹道型技能")] Ballistic,          //弹道型技能
}
public enum RoleTargetType//作用目标
{
    [LabelText("未配置")] None,
    [LabelText("队友")] Teammate,   //队友
    [LabelText("敌人")] Enemy,      //敌人
}
public enum SkillAttackType //技能攻击方式
{
    [LabelText("单体目标")] SingTarget,     //单体目标
    [LabelText("所有英雄")] AllHero,        //所有英雄
    [LabelText("后排英雄")] BackRowHero,    //后排英雄
    [LabelText("前排英雄")] ForntRowHero,   //前排英雄
    [LabelText("同列英雄")] SameColumnHero, //同一列英雄,同一列全部阵亡后攻击另一列，最后是中间
}