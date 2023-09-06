#if TENGINE_NET
using TEngine.Core.Network;
using TEngine.Core;

namespace TEngine.Logic;

public class H_C2M_MessageHandler : Addressable<Unit1,H_C2M_Message>
{
    protected override async FTask Run(Unit1 unit, H_C2M_Message message)
    {
        Log.Debug($"接收到一个Address消息 Unit:{unit.Id} message:{message.ToJson()}");
        await FTask.CompletedTask;
    }
}
#endif