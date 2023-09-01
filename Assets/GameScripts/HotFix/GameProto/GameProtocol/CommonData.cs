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
		public uint OpCode() { return OuterOpcode.CharacterData; }
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
	}
	/// <summary>
	/// 单位属性
	/// </summary>
	[ProtoContract]
	public partial class UnitAttr : AProto
	{
		public uint OpCode() { return OuterOpcode.UnitAttr; }
		[ProtoMember(1)]
		public int Hp { get; set; }
		[ProtoMember(2)]
		public int Mp { get; set; }
		[ProtoMember(3)]
		public int MaxHp { get; set; }
		[ProtoMember(4)]
		public int MaxMp { get; set; }
		[ProtoMember(5)]
		public int Attack { get; set; }
		[ProtoMember(6)]
		public int Defense { get; set; }
		[ProtoMember(7)]
		public int Shield { get; set; }
		[ProtoMember(8)]
		public int PhysicalHit { get; set; }
		[ProtoMember(9)]
		public int MagicPenetration { get; set; }
		[ProtoMember(10)]
		public int Evade { get; set; }
		[ProtoMember(11)]
		public int Speed { get; set; }
		[ProtoMember(12)]
		public int CriticalHit { get; set; }
	}
}
