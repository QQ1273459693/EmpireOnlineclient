using ProtoBuf;
using Unity.Mathematics;
using System.Collections.Generic;
using TEngine.Core.Network;
#pragma warning disable CS8618

namespace TEngine
{
	[ProtoContract]
	public partial class ItemData : AProto
	{
		[ProtoMember(1)]
		public int count { get; set; }
		[ProtoMember(2)]
		public Item item { get; set; }
	}
	[ProtoContract]
	public partial class Item : AProto
	{
		[ProtoMember(1)]
		public int itemId { get; set; }
		[ProtoMember(2)]
		public EquipData equipData { get; set; }
	}
	[ProtoContract]
	public partial class EquipData : AProto
	{
		[ProtoMember(1)]
		public int slv { get; set; }
	}
}
