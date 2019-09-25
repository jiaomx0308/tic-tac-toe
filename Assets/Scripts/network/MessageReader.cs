using Google.ProtocolBuffers;
using UnityEngine;
using System;

//using MessageID = System.UInt16;
//using MessageLength = System.UInt32;

public class MessageReader
{
    enum READ_STATE
    {
        //READ_STATE_FLAG = 0,
        READ_STATE_MSGLEN = 1,
        READ_STATE_FLOWID = 2,
        READ_STATE_MODULEID = 3,
        READ_STATE_MSGID = 4,
        //READ_STATE_RESPONSE_TIME = 5,  //响应时间
        READ_STATE_RESPONSE_FLAG = 6,   //错误状态 
        READ_STATE_BODY = 7,
    }

    private byte msgid = 0;
    private ushort msglen = 0;
    byte flag;
    byte flowId;
    byte moduleId;
    byte responseFlag;
    public MessageHandler msgHandle = null;
    public IMainLoop mainLoop;
    /*
     * Response Packet Format
     * 
     * 0xcc   byte
     * length int
     * flowId int
     * moduleId byte
     * messageId short
     * responseTime int
     * responseFlag byte
     * protobuffer 
     */
    private uint expectSize = 1;   //表示MessageReader接下来要读取的字段长度
    private READ_STATE state = READ_STATE.READ_STATE_MSGLEN;  //表示当前正在读取的字段
    private MemoryStream stream = new MemoryStream();

    public MessageReader()
    {
        expectSize = 2;
        state = READ_STATE.READ_STATE_MSGLEN;
    }

    //该类的核心就是process函数,一种基于if else的状态机机制,而且if else具有优先级的特性,可以继续下去
    //datas是整个缓冲区的大小,length是其中数据的长度,flowHandler是可能的处理函数
    public void process(byte[] datas, uint length, System.Collections.Generic.Dictionary<uint, MessageHandler> flowHandler)
    {
        //Debug.LogError("process receive Data " + length + " state " + state+" expect "+expectSize);
        uint totallen = 0;  //这里把totallen设置为局部变量就表明
        while (length > 0 && expectSize > 0)
        {
            if (state == READ_STATE.READ_STATE_MSGLEN)//读取msg长度的模式
            {
                if (length >= expectSize)   //如果缓冲区数据长度大于可读取长度
                {
                    Array.Copy(datas, totallen, stream.data(), stream.wpos, expectSize);  //够长就直接拷贝数据Copy(Array sourceArray, long sourceIndex, Array destinationArray, long destinationIndex, long length);
                    totallen += expectSize;   //totallen是记录从网络上读取缓冲区的长度
                    stream.wpos += (int)expectSize;  //stream.wpos是写入目标缓冲区的偏移量
                    length -= expectSize;   //length是网络数据缓冲区的剩余长度

                    msglen = stream.readUint16();   //读取msglen
                    stream.clear();   //清空本地缓冲区

                    state = READ_STATE.READ_STATE_FLOWID;   //改变接下来要读取的状态
                    expectSize = 1;   //接下来要读取的数据长度大小
                }
                else    //如果不够读取的话,先读取length长度的数据,更新wpos和expectSize的大小,等待下一次读取
                {
                    Array.Copy(datas, totallen, stream.data(), stream.wpos, length);
                    stream.wpos += (int)length;
                    expectSize -= length;
                    break;
                }
            }
            else if (state == READ_STATE.READ_STATE_FLOWID)
            {
                if (length >= expectSize)
                {
                    Array.Copy(datas, totallen, stream.data(), stream.wpos, expectSize);
                    totallen += expectSize;
                    stream.wpos += (int)expectSize;
                    length -= expectSize;

                    flowId = stream.readUint8();
                    stream.clear();

                    state = READ_STATE.READ_STATE_MODULEID;
                    expectSize = 1;
                }
                else
                {
                    Array.Copy(datas, totallen, stream.data(), stream.wpos, length);
                    stream.wpos += (int)length;
                    expectSize -= length;
                    break;
                }

            }
            else if (state == READ_STATE.READ_STATE_MODULEID)
            {
                if (length >= expectSize)
                {
                    Array.Copy(datas, totallen, stream.data(), stream.wpos, expectSize);
                    totallen += expectSize;
                    stream.wpos += (int)expectSize;
                    length -= expectSize;

                    moduleId = stream.readUint8();
                    stream.clear();

                    state = READ_STATE.READ_STATE_MSGID;
                    expectSize = 1;
                }
                else
                {
                    Array.Copy(datas, totallen, stream.data(), stream.wpos, length);
                    stream.wpos += (int)length;
                    expectSize -= length;
                    break;
                }
            }
            else if (state == READ_STATE.READ_STATE_MSGID)
            {
                if (length >= expectSize)
                {
                    Array.Copy(datas, totallen, stream.data(), stream.wpos, expectSize);
                    totallen += expectSize;
                    stream.wpos += (int)expectSize;
                    length -= expectSize;

                    msgid = stream.readUint8();
                    stream.clear();

                    state = READ_STATE.READ_STATE_RESPONSE_FLAG;
                    expectSize = 1;
                }
                else
                {
                    Array.Copy(datas, totallen, stream.data(), stream.wpos, length);
                    stream.wpos += (int)length;
                    expectSize -= length;
                    break;
                }
            }
            else if (state == READ_STATE.READ_STATE_RESPONSE_FLAG)
            {
                if (length >= expectSize)
                {
                    Array.Copy(datas, totallen, stream.data(), stream.wpos, expectSize);
                    totallen += expectSize;
                    stream.wpos += (int)expectSize;
                    length -= expectSize;

                    responseFlag = stream.readUint8();
                    stream.clear();

                    state = READ_STATE.READ_STATE_BODY;
                    //expectSize = msglen - 4 - 1 - 2 - 4 - 2;
                    expectSize = (uint)(msglen - 1 - 1 - 1 - 1);//flowId moduleId msgId 
                }
                else
                {
                    Array.Copy(datas, totallen, stream.data(), stream.wpos, length);
                    stream.wpos += (int)length;
                    expectSize -= length;
                    break;
                }
            }

            /*
             * body Can be empty
             */
            if (state == READ_STATE.READ_STATE_BODY)   //真正的Body消息体
            {
                //Debug.LogError("body expect BodySize:"+length+" expSize "+expectSize);
                if (length >= expectSize)
                {
                    Array.Copy(datas, totallen, stream.data(), stream.wpos, expectSize);
                    totallen += expectSize;
                    stream.wpos += (int)expectSize;
                    length -= expectSize;
                    /*
                     * No Handler Or PushMessage  forward To IPacketHandler 
                     * Call Who's RPC Method Or Register Many RPC Method to Handle It ?
                     * [PushHandler]
                     * void GCPushSpriteInfo(Packet packet) {
                     * }
                     * 
                     * PacketHandler namespace
                     * IPacketHandler---->GCPushSpriteInfo
                     */
                    IMessageLite pbmsg = MyLib.Util.GetMsg(moduleId, msgid, stream.getBytString());  //通过moduleId,msgid找到对于的proto结构,然后把stream中的内容调用proto的库转换为proto结构
                                                                                               //Debug.LogError("expect msgType: "+pbmsg.GetType());
                    Packet p = new Packet(msglen, flowId, moduleId, msgid, responseFlag, pbmsg);
                    var fullName = pbmsg.GetType().FullName;
                    mainLoop.queueInLoop(() =>     //这里的mainloop就是用MonoBehaviour开出来的模拟线程
                    {
                        Debug.Log("ReadPacket: " + p.protoBody.ToString());
                    });

                    mainLoop.queueInLoop(() =>   //放到Mainloop中进行消息处理
                    {
                        MessageHandler handler = null;
                        if (flowHandler == null)
                        {
                            handler = msgHandle;     //msgHandle由RemoteClient提供
                            }
                        else if (flowHandler.ContainsKey(flowId))  //如果需要对该flowId特殊处理,在这里完成,对于由客户端发起带有Flowid的消息,服务器会回一个同样Flowid的消息
                            {
                            handler = flowHandler[flowId];
                            flowHandler.Remove(flowId);
                            if (handler == null)
                            {
                                Debug.LogError("FlowHandlerIsNull: " + flowId);
                            }
                        }
                        else
                        {
                            handler = msgHandle;   //,默认也是由RemoteClient提供msgHandle处理
                            }
                        if (handler != null)
                        {
                            handler(p);
                        }
                        else
                        {
                            Debug.LogError("MessageReader::process No handler for flow Message " + msgid + " " + flowId + " " + pbmsg.GetType() + " " + pbmsg);
                        }


                    });


                    //fullName是检测handlePB()中返回的继承该IMessageLite接口的类是否带有Push字段,带有该字段的协议是由服务器主动发起,

                    //Debug.LogError("HandlerIs: "+flowId+" h "+handler);
                    if (fullName.Contains("Push"))
                    {
                        //Log.Net("MessageReader Handler PushMessage");
                        if (mainLoop != null)
                        {
                            mainLoop.queueInLoop(delegate
                            {
                                var handlerName = fullName.Replace("MyLib", "PacketHandler");
                                var tp = Type.GetType(handlerName);   //通过type的名字找到该Type
                                    if (tp == null)
                                {
                                    Debug.LogError("PushMessage noHandler " + handlerName);
                                }
                                else
                                {
                                        //Debug.Log("Handler Push Message here "+handlerName);
                                        var ph = (IPacketHandler)Activator.CreateInstance(tp);  //Activator .CreateInstance()用于创建类的实例
                                        ph.HandlePacket(p);   //IPacketHandler提供了HandlePacket的方法,该IPacketHandler接口主要是对协议中的Push字段(服务器单方消息)进行处理的接口
                                    }

                            });
                        }
                    }

                    stream.clear();
                    state = READ_STATE.READ_STATE_MSGLEN;
                    expectSize = 2;
                }
                else
                {
                    Array.Copy(datas, totallen, stream.data(), stream.wpos, length);
                    stream.wpos += (int)length;
                    expectSize -= length;
                    break;
                }
            }

        }

        if (responseFlag != 0)
        {
            Debug.LogError("MessageReader:: read Error Packet " + responseFlag);
        }

        //Log.Net("current state after " + state + " msglen " + msglen + " " + length);
        //Log.Net("MessageReader::  prop  flag" + flag + "  msglen " + msglen + " flowId " + flowId + " moduleId " + moduleId + " msgid " + msgid + " responseTime " + responseTime + " responseFlag " + responseFlag + " expectSize " + expectSize);
    }

}
