using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP.WebSocket;
using System;

public class SocketBase  
{
    protected WebSocket socket;
    protected Uri webSocketUrl = new Uri("ws://127.0.0.1:7777");
    public virtual void InitSocket()
    {
        socket = new WebSocket(webSocketUrl);


        this.socket.OnOpen += OnOpen;
        this.socket.OnMessage += OnMessageReceived;
        this.socket.OnBinary += OnBinaryReceived;
        this.socket.OnClosed += OnClosed;
        this.socket.OnError += OnError;
        socket.Open();
    }
    protected virtual void OnOpen(WebSocket ws)
    {
        Debuger.Log("WebSocket Open!");
        socket.Send("你好啊 服务器，我是客户端");
    }

    protected virtual void OnMessageReceived(WebSocket ws, string message)
    {
        Debuger.Log(string.Format("Message received: <color=yellow>{0}</color>", message));
    }
    protected virtual void OnBinaryReceived(WebSocket webSocket, byte[] data)
    {

        Protocal protocal = ProtoBuffSerialize.DeSerializeProtocal(data);
        byte[] contentBytes = ProtoBuffSerialize.DeSerializePacketContent(data);
        Debuger.Log(string.Format("Message received: <color=yellow>{0}  protocal:{1}</color>", data.Length,protocal.ToString()));
        ReceivePacket(protocal,contentBytes);
    }
    protected virtual void OnClosed(WebSocket ws, UInt16 code, string message)
    {
        Debuger.Log(string.Format("WebSocket closed! Code: {0} Message: {1}", code, message));

        socket = null;

    }
    protected virtual void OnError(WebSocket ws, string error)
    {
        Debuger.Log(string.Format("An error occured: <color=red>{0}</color>", error));

        socket = null;

    }
    /// <summary>
    /// 接收数据包
    /// </summary>
    /// <param name="protocal"></param>
    /// <param name="packet"></param>
    protected virtual void ReceivePacket(Protocal protocal, byte[] packet)
    {

    }
    /// <summary>
    /// 发送数据包
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="protocal"></param>
    /// <param name="data"></param>
    public virtual void SendPacket<T>(Protocal protocal, T data)
    {
        SendPacket(ProtoBuffSerialize.Serializer(protocal, data));
    }

    private void SendPacket(byte[] data)
    {
        socket.Send(data);
        Debuger.Log("Send Packet To Server Data.length:" + data.Length);
    }

    protected virtual void OnDestroy()
    {
        if (this.socket != null)
        {
            this.socket.Close();
            this.socket = null;
        }
    }
}
