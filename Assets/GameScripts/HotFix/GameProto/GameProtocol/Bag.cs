using ProtoBuf;
using Unity.Mathematics;
using System.Collections.Generic;
using TEngine.Core.Network;
#pragma warning disable CS8618

namespace TEngine
{
	[ProtoContract]
	public partial class C2L_BagInfo : AProto, IRequest
	{
		[ProtoIgnore]
		public L2C_BagInfo ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.C2L_BagInfo; }
	}
	[ProtoContract]
	public partial class L2C_BagInfo : AProto, IResponse
	{
		public uint OpCode() { return OuterOpcode.L2C_BagInfo; }
		[ProtoMember(91, IsRequired = true)]
		public uint ErrorCode { get; set; }
		[ProtoMember(1)]
		public List<Slot> slot = new List<Slot>();
	}
	[ProtoContract]
	public partial class L2C_BagUpdate : AProto, IMessage
	{
		public uint OpCode() { return OuterOpcode.L2C_BagUpdate; }
		[ProtoMember(1)]
		public int UpdateType { get; set; }
		[ProtoMember(2)]
		public List<Slot> info = new List<Slot>();
	}
	[ProtoContract]
	public partial class C2L_EquipWear : AProto, IRequest
	{
		[ProtoIgnore]
		public L2C_EquipWear ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.C2L_EquipWear; }
		[ProtoMember(1)]
		public bool OnWear { get; set; }
		[ProtoMember(2)]
		public long idx { get; set; }
		[ProtoMember(3)]
		public int WearIdx { get; set; }
	}
	[ProtoContract]
	public partial class L2C_EquipWear : AProto, IResponse
	{
		public uint OpCode() { return OuterOpcode.L2C_EquipWear; }
		[ProtoMember(91, IsRequired = true)]
		public uint ErrorCode { get; set; }
	}
}
