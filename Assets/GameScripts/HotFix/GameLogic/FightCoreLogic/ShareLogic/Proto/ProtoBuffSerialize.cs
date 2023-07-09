using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;

public class ProtoBuffSerialize
{
    /// <summary>
    /// 序列化ProtoBuff结构体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] Serializer<T>(T data)
    {
        try
        {
            //涉及格式转换，需要用到流，将二进制序列化到流中
            using (MemoryStream ms = new MemoryStream())
            {
                //使用ProtoBuf工具的序列化方法
                ProtoBuf.Serializer.Serialize(ms, data);
                //定义二级制数组，保存序列化后的结果
                byte[] result = new byte[ms.Length];
                //将流的位置设为0，起始点
                ms.Position = 0;
                //将流中的内容读取到二进制数组中
                ms.Read(result, 0, result.Length);
                return result;
            }
        }
        catch (Exception ex)
        {
            Debuger.LogError("Serializer Fialed" + ex.ToString());
            return new byte[0];
        }
    }

    /// <summary>
    /// 反序列化Protobuff二进制
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    public static T DeSerialize<T>(byte[] msg)
    {
        try
        {
            using (MemoryStream ms = new MemoryStream())
            {
                //将消息写入流中
                ms.Write(msg, 0, msg.Length);
                //将流的位置归0
                ms.Position = 0;
                //使用工具反序列化对象
                T result = ProtoBuf.Serializer.Deserialize<T>(ms);
                return result;
            }
        }
        catch (Exception ex)
        {
            Debuger.LogError("DeSerialize Fiaied" + ex.ToString()) ;
            return default(T);
        }
    }

    /// <summary>
    /// 反序列化协议枚举
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    public static Protocal DeSerializeProtocal(byte[] packet)
    {
        short protocalcode = BitConverter.ToInt16(packet, 0);
        Protocal protocal = (Protocal)protocalcode;
        return protocal;
    }
    /// <summary>
    /// 反序列化消息包内容，不包含协议号
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    public static byte[] DeSerializePacketContent(byte[] packet)
    {
        try
        {
            byte[] contentByets = new byte[packet.Length - 2];
            Array.Copy(packet, 2, contentByets, 0, contentByets.Length);
            return contentByets;
        }
        catch (Exception e)
        {
            Debuger.LogError("DeSerializePacketContent Fiaied" + e.ToString());
            return new byte[0];
        }
     
    }
    public static byte[] Serializer<T>(Protocal protocal, T data)
    {
        byte[] buffer = Serializer<T>(data);
        return ConvertPacket(protocal, buffer);
    }
    /// <summary>
    /// 转换为数据包
    /// </summary>
    /// <param name="protocal"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private static byte[] ConvertPacket(Protocal protocal, byte[] data)
    {
        try
        {
            byte[] packetHead = BitConverter.GetBytes((short)protocal);
            byte[] packet = new byte[packetHead.Length + data.Length];

            Array.Copy(packetHead, 0, packet, 0, packetHead.Length);

            Array.Copy(data, 0, packet, packetHead.Length, data.Length);
            return packet;
        }
        catch (Exception e )
        {
            return new byte[0];
            Debuger.LogError(e.ToString()) ;
        }
        
    }
}

