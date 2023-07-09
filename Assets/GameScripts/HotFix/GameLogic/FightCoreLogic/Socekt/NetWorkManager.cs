using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetWorkManager : SocketBase 
{
    private static NetWorkManager _intance;
    public static NetWorkManager Instance
    {
        get
        {
            if (_intance==null)
            {
                _intance = new NetWorkManager();
            }
            return _intance;
        } }
    protected override void ReceivePacket(Protocal protocal, byte[] packet)
    {
        Debuger.Log("NetWorkManager ReceivePacket: DispensEvent Protocal:"+protocal.ToString());
        NetEventControl.DispensEvent(protocal, packet);
    }
}
