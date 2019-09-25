using Google.ProtocolBuffers;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public struct SendPacketInfo
{
    public uint fid;
    public byte[] data;
}

public class Bundle
{
    private static Queue<Bundle> pool = new Queue<Bundle>();
    public static List<string> sendMsg = new List<string>();
    public static List<string> recvMsg = new List<string>();

    private static byte flowId = 1;

    private MemoryStream stream = new MemoryStream();
    //public List<MemoryStream> streamList = new List<MemoryStream>();
    public int numMessage = 0;

    public byte moduleId;
    public byte msgId;

    private static uint lastFid;
    public Bundle()
    {
    }

    private static Bundle GetBundle()
    {
        if (pool.Count > 0)
        {
            var q = pool.Dequeue();
            return q;
        }
        else
        {
            return new Bundle();
        }
    }

    public static void ReturnBundle(Bundle bundle)
    {
        bundle.Reset();
        pool.Enqueue(bundle);
    }

    private void Reset()
    {
        moduleId = 0;
        msgId = 0;
        numMessage = 0;
        stream.clear();
        //streamList.Clear();
    }

    public void newMessage(System.Type type)
    {  //这里根据msh的名字取得了msg的id和协议id
       //fini (false);

        var pa = SaveGame.saveGame.GetMsgID(type.Name);
        moduleId = pa.moduleId;
        msgId = pa.messageId;

        numMessage += 1;
    }



    public void fini(bool issend)
    {
        /*
        if(numMessage > 0)
        {
            if(stream != null)
                streamList.Add(stream);
        }


        if(issend)
        {
            numMessage = 0;
        }
        */
    }





    public static Packet GetPacketFid(IBuilderLite build)
    {  //根据传入的协议,返回Packet,Packet中的data为序列化为2进制流的proto流,flowid为消息id标识
        var p = new Packet();

        var bundle = GetBundle();
        var data = build.WeakBuild();
        Debug.Log("GetPacket: " + data);
        bundle.newMessage(data.GetType());
        var fid = bundle.writePB(data);
        var buff = bundle.stream.getbuffer();
        p.flowId = (byte)fid;
        p.data = buff;
        return p;
    }


    public static byte[] GetPacket(IBuilderLite build, out Bundle b)
    {   //根据创建的proto协议解析成二进制流返回,并可附带传入包含二进制流的Bundle对象
        var bundle = GetBundle();
        b = bundle;
        var data = build.WeakBuild(); //通过WeakBuild接口,把IBuilderLite转化为2进制流的数据
        Debug.Log("GetPacket: " + data);
        bundle.newMessage(data.GetType());   //设置该Bundle的moduleId和msgid
        var fid = bundle.writePB(data);    //把数据写入bundle,包括协议头,数据长度,协议id,msgid,数据
        lastFid = fid;   //Fidid可以用来表示该客户端写入的第几条消息
        return bundle.stream.getbuffer();   //返回该二进制流
    }

    public static SendPacketInfo GetPacketFull(IBuilderLite build, out Bundle b)
    {   //根据建的proto协议解析成二进制流,返回SendPacketInfo的结构体,该结构体包含而进制流和fidid
        var data = GetPacket(build, out b);
        var t = new SendPacketInfo()
        {
            fid = lastFid,
            data = data,
        };
        return t;
    }



    public void checkStream(int v)
    {
    }

    //---------------------------------------------------------------------------------
    public void writeInt8(SByte v)
    {
        checkStream(1);
        stream.writeInt8(v);
    }

    public void writeInt16(Int16 v)
    {
        checkStream(2);
        stream.writeInt16(v);
    }

    public void writeInt32(Int32 v)
    {
        checkStream(4);
        stream.writeInt32(v);
    }

    public void writeInt64(Int64 v)
    {
        checkStream(8);
        stream.writeInt64(v);
    }

    public void writeUint8(Byte v)
    {
        checkStream(1);
        stream.writeUint8(v);
    }

    public void writeUint16(UInt16 v)
    {
        checkStream(2);
        stream.writeUint16(v);
    }

    public void writeUint32(UInt32 v)
    {
        checkStream(4);
        stream.writeUint32(v);
    }

    public void writeUint64(UInt64 v)
    {
        checkStream(8);
        stream.writeUint64(v);
    }

    public void writeFloat(float v)
    {
        checkStream(4);
        stream.writeFloat(v);
    }

    public void writeDouble(double v)
    {
        checkStream(8);
        stream.writeDouble(v);
    }

    public void writeString(string v)
    {
        checkStream(v.Length + 1);
        stream.writeString(v);
    }

    public void writeBlob(byte[] v)
    {
        checkStream(v.Length + 4);
        stream.writeBlob(v);
    }

    /*
     * 0xcc   int8
     * length int32
     * flowId int32
     * moduleId int8
     * messageId int16
     * protobuffer
     */
    public uint writePB(byte[] v)
    {    //把完整的协议写入memeryStream中
        byte fid = flowId++;
        if (fid == 0)
        {
            fid++;
            flowId++;
        }

        int bodyLength = 1 + 1 + 1 + v.Length;
        int totalLength = 2 + bodyLength;
        //checkStream (totalLength);
        //Debug.Log ("Bundle::writePB pack data is "+bodyLength+" pb length "+v.Length+" totalLength "+totalLength);
        //Debug.Log ("Bundle::writePB module Id msgId " + moduleId+" "+msgId);
        //stream.writeUint8 (Convert.ToByte(0xcc));
        stream.writeUint16(Convert.ToUInt16(bodyLength));
        stream.writeUint8(Convert.ToByte(fid));
        stream.writeUint8(Convert.ToByte(moduleId));
        stream.writeUint8(Convert.ToByte(msgId));
        stream.writePB(v);

        return fid;
    }

    public uint writePB(IMessageLite pbMsg)
    {   //把IMessageLite通过MemoryStream转化为Bytes
        byte[] bytes;
        using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
        {
            pbMsg.WriteTo(stream);
            bytes = stream.ToArray();
        }
        return writePB(bytes);
    }

}
