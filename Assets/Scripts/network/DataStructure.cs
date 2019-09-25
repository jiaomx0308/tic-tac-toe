using UnityEngine;
using Google.ProtocolBuffers;
using System.Collections;
using System.Collections.Generic;


public delegate void MessageHandler(Packet msg);

public class UtilPair
{
    public byte moduleId;
    public byte messageId;

    public UtilPair(byte a, byte b)
    {
        moduleId = a;
        messageId = b;
    }
}


public class Packet
{
    public System.UInt32 msglen = 0;
    public byte flowId;
    public byte moduleId;
    public byte msgid = 0;
    public byte responseFlag;

    public IMessageLite protoBody;
    public byte[] data;


    public Packet(uint len, uint fid, byte module, byte msg, byte resflag, IMessageLite pb)
    {
        //Debug.Log ("receive packet" );
        msglen = len;
        flowId = (byte)fid;
        moduleId = module;
        msgid = msg;
        responseFlag = resflag;

        protoBody = pb;
        //Debug.Log ("Packet:: readPacket "+fid);
        //Debug.Log ("Packet:: readPacket " + protoBody.GetType ().FullName);
    }

    public Packet()
    {
    }
}

public class PacketHolder
{
    public Packet packet;
}

public abstract class IPacketHandler
{
    public IPacketHandler()
    {

    }
    public abstract void HandlePacket(Packet packet);

}
