using ProtoBuf;
using Unity.Mathematics;
using System.Collections.Generic;
using TEngine.Core.Network;
#pragma warning disable CS8618

namespace TEngine
{	
	/// <summary>
	/// 角色详细数据
	/// </summary>
	[ProtoContract]
	public partial class CharacterData : AProto
	{
		[ProtoMember(1)]
		public string Name { get; set; }
		[ProtoMember(2)]
		public int Gold { get; set; }
		[ProtoMember(3)]
		public int Diamond { get; set; }
		[ProtoMember(4)]
		public int Level { get; set; }
		[ProtoMember(5)]
		public int SkillPoints { get; set; }
		[ProtoMember(6)]
		public UnitAttr PlayerAttribute { get; set; }
		[ProtoMember(7)]
		public int Exp { get; set; }
		[ProtoMember(8)]
		public List<CharEquipSlotData> EquipslotDat = new List<CharEquipSlotData>();
		[ProtoMember(9)]
		public List<SkillData> PassiveSkills = new List<SkillData>();
		[ProtoMember(10)]
		public List<SkillData> ActiveSkills = new List<SkillData>();
		[ProtoMember(11)]
		public List<SkillData> AutoSkills = new List<SkillData>();
	}
	/// <summary>
	/// 角色技能数据
	/// </summary>
	[ProtoContract]
	public partial class SkillData : AProto
	{
		[ProtoMember(1)]
		public int SkID { get; set; }
		[ProtoMember(2)]
		public int ReceID { get; set; }
		[ProtoMember(3)]
		public int SkillType { get; set; }
		[ProtoMember(4)]
		public int Lv { get; set; }
	}
	/// <summary>
	/// 角色装备栏
	/// </summary>
	[ProtoContract]
	public partial class CharEquipSlotData : AProto
	{
		[ProtoMember(1)]
		public int Pos { get; set; }
		[ProtoMember(2)]
		public Slot slot { get; set; }
	}
	[ProtoContract]
	public partial class Slot : AProto
	{
		[ProtoMember(1)]
		public long idx { get; set; }
		[ProtoMember(2)]
		public ItemData itemData { get; set; }
	}
	/// <summary>
	/// 单位属性
	/// </summary>
	[ProtoContract]
	public partial class UnitAttr : AProto
	{
		[ProtoMember(1)]
		public int Hp { get; set; }
		[ProtoMember(2)]
		public int Mp { get; set; }
		[ProtoMember(3)]
		public int MaxHp { get; set; }
		[ProtoMember(4)]
		public int MaxMp { get; set; }
		[ProtoMember(5)]
		public int MeleeAk { get; set; }
		[ProtoMember(6)]
		public int RangeAk { get; set; }
		[ProtoMember(7)]
		public int MagicAk { get; set; }
		[ProtoMember(8)]
		public int MeDEF { get; set; }
		[ProtoMember(9)]
		public int RGDEF { get; set; }
		[ProtoMember(10)]
		public int MGDEF { get; set; }
		[ProtoMember(11)]
		public int ELMRES { get; set; }
		[ProtoMember(12)]
		public int CurseMgRES { get; set; }
		[ProtoMember(13)]
		public int Shield { get; set; }
		[ProtoMember(14)]
		public int PhysicalHit { get; set; }
		[ProtoMember(15)]
		public int EleMagicHit { get; set; }
		[ProtoMember(16)]
		public int CurseMagicHit { get; set; }
		[ProtoMember(17)]
		public int MagicPenetration { get; set; }
		[ProtoMember(18)]
		public int Evade { get; set; }
		[ProtoMember(19)]
		public int Speed { get; set; }
		[ProtoMember(20)]
		public int CriticalHit { get; set; }
		[ProtoMember(21)]
		public int MixDamage { get; set; }
		[ProtoMember(22)]
		public int MaxDamage { get; set; }
		[ProtoMember(23)]
		public int Tough { get; set; }
		[ProtoMember(24)]
		public int ArmorBreakingAT { get; set; }
		[ProtoMember(25)]
		public int SwordDamageAdd { get; set; }
		[ProtoMember(26)]
		public int KnifeBreakingAT { get; set; }
	}
}
