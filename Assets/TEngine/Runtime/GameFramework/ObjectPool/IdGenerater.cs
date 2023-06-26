using System;

namespace TEngine
{
	public static class IdGenerater
	{
		private static readonly long epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
		public static long AppId { private get; set; }

		private static ushort value;

		public static long GenerateId()
		{
			long time = (DateTime.UtcNow.Ticks - epoch) / 10000000;

			return (AppId << 48) + (time << 16) + ++value;
		}
	}
}