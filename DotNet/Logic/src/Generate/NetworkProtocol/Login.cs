using ProtoBuf;
using Unity.Mathematics;
using System.Collections.Generic;
using TEngine.Core.Network;
#pragma warning disable CS8618

namespace TEngine
{	
	/// <summary>
	///  客户端发送消息请求注册账户
	/// </summary>
	[ProtoContract]
	public partial class C2L_CreateRole : AProto, IRequest
	{
		[ProtoIgnore]
		public L2C_CreateRole ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.C2L_CreateRole; }
		[ProtoMember(1)]
		public string UserName { get; set; }
		[ProtoMember(2)]
		public string Password { get; set; }
		[ProtoMember(3)]
		public uint SDKUID { get; set; }
	}
	[ProtoContract]
	public partial class L2C_CreateRole : AProto, IResponse
	{
		public uint OpCode() { return OuterOpcode.L2C_CreateRole; }
		[ProtoMember(91, IsRequired = true)]
		public uint ErrorCode { get; set; }
		[ProtoMember(1)]
		public uint UID { get; set; }
	}
	/// <summary>
	///  登陆上行
	/// </summary>
	[ProtoContract]
	public partial class C2L_Login : AProto, IRequest
	{
		[ProtoIgnore]
		public L2C_Login ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.C2L_Login; }
		[ProtoMember(1)]
		public string UserName { get; set; }
		[ProtoMember(2)]
		public string Password { get; set; }
	}
	[ProtoContract]
	public partial class L2C_Login : AProto, IResponse
	{
		public uint OpCode() { return OuterOpcode.L2C_Login; }
		[ProtoMember(91, IsRequired = true)]
		public uint ErrorCode { get; set; }
		[ProtoMember(1)]
		public uint UID { get; set; }
	}
}
