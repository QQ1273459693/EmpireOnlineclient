using ProtoBuf;
using Unity.Mathematics;
using System.Collections.Generic;
using TEngine.Core.Network;
#pragma warning disable CS8618

namespace TEngine
{
	/// <summary>
	/// 进入游戏
	/// </summary>
	[ProtoContract]
	public partial class L2C_EnterGame : AProto, IMessage
	{
		public uint OpCode() { return OuterOpcode.L2C_EnterGame; }
		[ProtoMember(1)]
		public CharacterData characterData { get; set; }
		[ProtoMember(2)]
		public List<CurrencyIcon> currency = new List<CurrencyIcon>();
	}
	[ProtoContract]
	public partial class CurrencyIcon : AProto
	{
		public uint OpCode() { return OuterOpcode.CurrencyIcon; }
		[ProtoMember(1)]
		public int ctype { get; set; }
		[ProtoMember(2)]
		public int count { get; set; }
	}
	/// <summary>
	/// 更新游戏币
	/// </summary>
	[ProtoContract]
	public partial class L2C_UpdateCurrency : AProto, IMessage
	{
		public uint OpCode() { return OuterOpcode.L2C_UpdateCurrency; }
		[ProtoMember(1)]
		public List<CurrencyIcon> currency = new List<CurrencyIcon>();
	}
	/// <summary>
	/// 玩家属性更新
	/// </summary>
	[ProtoContract]
	public partial class L2C_PlayerNotifyUpdate : AProto, IMessage
	{
		public uint OpCode() { return OuterOpcode.L2C_PlayerNotifyUpdate; }
		[ProtoMember(1)]
		public CharacterData characterData { get; set; }
	}
}
