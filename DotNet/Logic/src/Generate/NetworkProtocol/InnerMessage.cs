using ProtoBuf;
using Unity.Mathematics;
using System.Collections.Generic;
using TEngine.Core.Network;
#pragma warning disable CS8618

namespace TEngine
{
	/// <summary>
	///  Gate跟Map服务器进行通讯、注册Address协议
	/// </summary>
	[ProtoContract]
	public partial class I_G2M_LoginAddressRequest : AProto, IRouteRequest
	{
		[ProtoIgnore]
		public I_M2G_LoginAddressResponse ResponseType { get; set; }
		public uint OpCode() { return 1000000; }
		public long RouteTypeOpCode() { return CoreRouteType.Route; }
		[ProtoMember(1)]
		public long AddressableId { get; set; }
		[ProtoMember(2)]
		public long GateRouteId { get; set; }
	}
	[ProtoContract]
	public partial class I_M2G_LoginAddressResponse : AProto, IRouteResponse
	{
		public uint OpCode() { return 100000; }
		[ProtoMember(91, IsRequired = true)]
		public uint ErrorCode { get; set; }
		[ProtoMember(1)]
		public long AddressableId { get; set; }
	}
}
