using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;


public class NetWork
{
    TcpClient host;
    NetworkStream byteStream;

    int nextRecvPos; 
    byte[] revcBuf;
    byte[] buf;
    static NetWork NetWorkPtr = null;
    public Queue<ServerMsg> MQ;
    public bool connected;

    public static NetWork GetNetWork()
    {
        if(NetWorkPtr == null)
            NetWorkPtr = new NetWork();
        return NetWorkPtr;
    }

    NetWork()
    {
        nextRecvPos = 0;
        revcBuf = new byte[2048 * 4];
        buf = new byte[2048 * 8];
        connected = false;
        MQ = new Queue<ServerMsg>();
        host = new TcpClient();

        // 后台线程
        Thread client = new Thread(new ThreadStart(Connect));
        client.IsBackground = true;
        client.Start();
    }

    void Connect()
    {
        try
        {
            host.Connect(IPAddress.Parse(NetSetting.Addr), NetSetting.Port);
            Debug.Log("Connect Server Successfully");
            connected = true;
        }
        catch (Exception e)
        {
            Debug.Log("Connect Server Failed" + e);
            connected = false;
        }
    }

    // send to Server
    public IEnumerator SendServer(byte[] bytes)
    {
        bool flag = false;
        if(!connected)
            yield return flag;
        byteStream = host.GetStream();
        if (byteStream.CanWrite)
        {
            try
            {
                byte[] sendBuf = PackData(bytes);
                byteStream.Write(sendBuf, 0, sendBuf.Length);
                flag = true;
            }
            catch (System.IO.IOException e)
            {
                Debug.Log(e);
                flag = false;
                connected = false;
            }
        }
        yield return flag;
    }

    byte[] PackData(byte[] data)
    {
        byte[] lenByte = BitConverter.GetBytes(data.Length + NetSetting.NET_HEAD_LENGTH_SIZE);
        byte[] sendBuf = new byte[data.Length + lenByte.Length];
        Array.Copy(lenByte, 0, sendBuf, 0, lenByte.Length);
        Array.Copy(data, 0, sendBuf, lenByte.Length, data.Length);
        return sendBuf;
    }

    // recv from Server
    public void RecvServer()
    {
        if (!connected)
            return; 
        byteStream = host.GetStream();
        while (byteStream.DataAvailable)
        {
            int length = byteStream.Read(revcBuf, 0, revcBuf.Length);
            Array.Copy(revcBuf, 0, buf, nextRecvPos, length);
            UnPackData(buf, nextRecvPos + length);
        }
    }

    void UnPackData(byte[] dataBuffer, int lastPos)
    {
        int pos = 0;
        while (pos + NetSetting.NET_HEAD_LENGTH_SIZE < lastPos)      //  确保有4字节
        {
            short msgType = 0;
            string dataStr = "";
            int msgLen = BitConverter.ToInt32(dataBuffer, pos);      //  自定义数据包的头4字节  为整个自定义数据包的长度
            if (pos + msgLen > lastPos)                              //  自定义数据包数据不完整  部分在下一个网络数据包
                break;
            if (pos + NetSetting.NET_HEAD_LENGTH_SIZE >= 0 && msgLen > 0 && pos + NetSetting.NET_HEAD_LENGTH_SIZE + msgLen <= dataBuffer.Length)
            {
                pos += NetSetting.NET_HEAD_LENGTH_SIZE;                   //  第5字节才是数据
                pos += sizeof(short);                                     //  前两个字节为 服务器数据包 的标识符
                msgType = BitConverter.ToInt16(dataBuffer, pos);
                pos += sizeof(short);
                int len = BitConverter.ToInt32(dataBuffer, pos);
                pos += sizeof(int);
                dataStr = Encoding.Default.GetString(dataBuffer, pos,  len);
                pos += len;
            }
            ServerMsg serverMsg = MsgHandler.BuiltServerMsg(msgType, dataStr);   //  根据消息类型  生成相应的消息对象
            MQ.Enqueue(serverMsg);
        }
        if (pos < lastPos)
            Array.Copy(dataBuffer, pos, dataBuffer, 0, lastPos - pos);
        nextRecvPos = lastPos - pos;
    }

}