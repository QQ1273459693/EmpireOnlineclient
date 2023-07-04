using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if CLIENT_LOGIC
[CreateAssetMenu(fileName = "BuffConfig", menuName = "BuffConfig", order = 0)]
public class BuffConfig : ScriptableObject
#else   
public class BuffConfig
#endif
{

    [LabelText("Buff图标"), LabelWidth(0.1f), PreviewField(70, ObjectFieldAlignment.Left), SuffixLabel("Buff图标")]
    [JsonIgnore]
    public Sprite buffIcon;                 //buff图标//
   
    public int buffid;                      //Buff id
    [LabelText("Buff名称")]
    public string buffName;                 //Buff 名称
   
  
    [LabelText("最大叠加层数")]
    public int maxStackingNum;              //最大叠加层数
    [LabelText("持续回合")]
    public int buffDurationRound;         //Buff持续回合
    [LabelText("持续时间ms")]
    public int buffDurationTimeMs;          //buff持续时间
    [LabelText("触发间隔ms")]
    public int buffTriggerIntervalMs;         //Buff触发间隔
    [LabelText("触发概率")]
    public int buffTriggerProbability;      //Buff触发概率
    [LabelText("Buff类型")]
    public BuffType buffType;               //Buff类型
    [LabelText("Buff状态")]
    public BuffState buffState;             //buff状态
    [LabelText("触发方式")]
    public BuffTriggerType buffTriggerType;     //触发方式
    [LabelText("伤害方式")]
    public BuffDamageType damageType;       //伤害方式
    [LabelText("伤害或百分比"), ProgressBar(0, 500, 0.8f, 0, 0)]
    public int damageOrPercentage;          //伤害或百分比

    [LabelText("Buff触发特效"),TitleGroup("Buff表现", "所有表现数据会在Buff开始释放或触发时生效")]
    public string buffEffect;               //Buff 特效
    [LabelText("Buff触发音效"), TitleGroup("Buff表现", "所有表现数据会在Buff开始释放或触发时生效")]
    [JsonIgnore]
    public AudioClip buffAudio;               //Buff 音效
    [LabelText("Buff触发动画"),TitleGroup("Buff表现", "所有表现数据会在Buff开始释放或触发时生效")]
    public string buffAnim;               //Buff Anim

    [Title("Buff描述:"), MultiLineProperty(4), HideLabel]
    public string buffDes;                 //Buff 名称

}
public enum BuffState
{
    [LabelText("无配置")] None,                                 
    [LabelText("沉默")] Silent,                     //沉默
    [LabelText("晕眩")] Dizzy,                      //晕眩
    [LabelText("冰冻")] Frozen,                     //冰冻
    [LabelText("灼烧")] Durn,                       //灼烧
    [LabelText("净化")] Purify,                     //净化
    [LabelText("百分比减伤")] PercentageReduceDamage,     //百分比减伤
    [LabelText("伤害加深")] DamageDeepening,            //伤害加深
    [LabelText("护盾增加")] ShieldIncrease,             //护盾增加
    [LabelText("生命回复增加")] HPRecoveryIncrease,         //生命回复增加
    [LabelText("生命回复减少")] HPRecoveryReduce,           //生命回复减少

}
public enum BuffType
{
    [LabelText("伤害Buff")] DamageBuff,
    [LabelText("增益Buff")] Buff,           //增益Buff
    [LabelText("减益Buff")] DeBuff,         //减益Buff
    [LabelText("控制buff")] Control,        //控制buff
}
//Buff触发类型
public enum BuffTriggerType
{
    [LabelText("即时性 一次性伤害")] OneDamage_RealTime,               //即时性 一次性伤害
    [LabelText("即时性 多段伤害")] MultisegmentDamage_RealTime,        //即时性 多段伤害
    [LabelText("行动开始时伤害")] Damage_RoundStart,                   //回合开始时伤害
    [LabelText("行动结束时伤害")] Damage_ActionEnd,                    //回合开始时伤害
}
public enum BuffDamageType
{
    [LabelText("无伤害，增加buff")] None,//无伤害，增加buff
    [LabelText("普通伤害")] NomalAttackDamage,  //普通伤害  (普通攻击)
    [LabelText("真实伤害")] RealDamage,         //真实伤害 (无视护盾、减伤)
    [LabelText("攻击力百分比伤害")] AtkPercentage,      //攻击力百分比伤害
    [LabelText("生命值白分比伤害")] HPPercentage,       //生命值白分比伤害
    [LabelText("无伤害控制")] NoneDamageControl,  //无伤害控制
}