using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System;
using System.Threading;
using System.Collections.Generic;

public class MsgBuffer
{
    public int position = 0;
    public byte[] buffer;
    public Bundle bundle;

    public int Size
    {
        get
        {
            return buffer.Length - position;
        }

    }

}

public enum RemoteClientEvent
{
    None,
    Connected,
    Close,
}

public class RemoteClient
{
    byte[] mTemp = new byte[8192];   //读取TCP数据的buff
    MessageReader msgReader = new MessageReader();   //接收消息后的反序列化的处理函数

    //Socket mSocket;
    private TcpClient client;
    IPEndPoint endPoint;
    public bool IsClose = false;
    private Queue<MsgBuffer> msgBuffer = new Queue<MsgBuffer>();   //send线程负责发送msg的queue队列
    public MessageHandler msgHandler;   //msg的处理回调
    public System.Action<RemoteClientEvent> evtHandler;   //evt的处理回调,这里的evt指的是网络连接状态

    private Dictionary<uint, MessageHandler> flowHandler = new Dictionary<uint, MessageHandler>();
    private ManualResetEvent signal = new ManualResetEvent(false);

    private IMainLoop ml;    //一个用MonoBehaviour模拟的线程
    private Thread sendThread;
    private Thread recvThread;

    public IMainLoop GetMainLoop()
    {
        return ml;
    }
    public RemoteClient(IMainLoop loop)
    {
        msgReader.msgHandle = HandleMsg;
        msgReader.mainLoop = loop;
        ml = loop;
    }


    private void SendThread()   //发送消息的线程
    {
        while (!IsClose)
        {
            //var tn = Thread.CurrentThread.ManagedThreadId;
            //Debug.LogError("ThreadN: "+tn);
            //var st = Util.GetTimeNow();

            signal.WaitOne();    //阻塞等待,当调用signal.set()方法时被唤醒,当signal.set()被调用后需要调用signal.Reset()才能时WaitOne()重新阻塞
            if (IsClose)
            {
                break;
            }
            MsgBuffer mb = null;
            lock (msgBuffer)
            {
                if (msgBuffer.Count > 0)
                {
                    mb = msgBuffer.Dequeue();
                }
            }
            if (mb != null)
            {
                try
                {
                    client.GetStream().Write(mb.buffer, mb.position, mb.Size);  //传输网络数据
                                                                                /*
                                                                                ml.queueInLoop(() =>
                                                                                {
                                                                                    Bundle.ReturnBundle(mb.bundle);
                                                                                });
                                                                                */
                }
                catch (Exception exception)
                {
                    Debug.LogError(exception.ToString());
                    Close();
                }
            }

            lock (msgBuffer)
            {
                if (msgBuffer.Count <= 0)
                {
                    signal.Reset();
                }
            }
            //var et = Util.GetTimeNow();
            //Debug.LogError("DiffTime: "+(et-st));
        }
    }

    /// <summary>
    /// 当消息处理器已经退出场景则关闭网络连接 
    /// </summary>
    /// <param name="packet">Packet.</param>
    void HandleMsg(Packet packet)   //proto消息解析后的处理函数,主要调用MessageHandler对proto消息进行处理
    {
        Debug.Log("HandlerMsg " + packet.protoBody);

        if (msgHandler != null)
        {
            msgHandler(packet);
        }
        else
        {
            Close();
        }
    }

    public void Connect(string ip1, int port1)   //网络连接,传入ip和port
    {
        //NetDebug.netDebug.AddMsg("Connect: "+ip1+" port "+port1);
        endPoint = new IPEndPoint(IPAddress.Parse(ip1), port1);
        try
        {
            client = new TcpClient();
            client.NoDelay = true;
            client.SendBufferSize = 1024;
            client.SendTimeout = 5;

            var result = client.BeginConnect(endPoint.Address, endPoint.Port, OnConnectResult, null);  //BeginConnect是一个异步调用,自己开线程去Connet,当连接完成时调用OnConnectResult
            var th = new Thread(CancelConnect);
            th.Start(result);
        }
        catch (Exception exception)
        {
            Debug.LogError(exception.Message);
            Close();
        }
    }

    void CancelConnect(object obj)  //连接超时检测
    {
        var res = (IAsyncResult)obj;
        if (res != null && !res.AsyncWaitHandle.WaitOne(3000))
        {
            Debug.LogError("ConnectError");
            Close();
        }
        else
        {
            //
        }
    }

    private void StartReceive()   //读取线程
    {
        //Debug.LogError("curThread: "+Thread.CurrentThread.ManagedThreadId);
        //Debug.LogError("StartReceive");
        try
        {
            //client.GetStream().BeginRead(mTemp, 0, mTemp.Length, OnReceive, null);
            var num = client.GetStream().Read(mTemp, 0, mTemp.Length);   //num被读取的字节数
            OnReceive2(num);
        }
        catch (Exception exception)
        {
            Debug.LogError(exception.ToString());
            Close();
        }
    }

    private void OnReceive2(int bytes)
    {
        //Debug.LogError("curThread: "+Thread.CurrentThread.ManagedThreadId);
        //var st = Util.GetTimeNow(); 
        if (bytes <= 0 || client == null || !client.Connected)  //断开连接的处理
        {
            Close();
            return;
        }

        uint num = (uint)bytes;
        msgReader.process(mTemp, num, flowHandler);  //数据读取完毕后交给MessageReader进行处理,反序列化
    }


    void OnConnectResult(IAsyncResult result)  //连接成功后开启两个线程,用于收发消息,
    {
        if (client == null)    //连接已经断开
        {
            return;
        }
        bool success = false;
        try
        {
            //mSocket.EndConnect(result);
            client.EndConnect(result);  //EndConnect是一个阻止方法, 用于完成BeginConnect方法中启动的异步远程主机连接请求,和BeginConnect配套使用
            success = true;

        }
        catch (Exception exception)
        {
            Debug.LogError(exception.Message);
            success = false;
        }
        if (success)
        {
            Debug.LogError("Connect Success");

            sendThread = new Thread(SendThread);
            sendThread.Start();
            recvThread = new Thread(RecvThread);
            recvThread.Start();
            this.ml.queueInLoop(() =>
            {
                //这里把该匿名函数加入到MainThreadLoop的queueInLoop中进行处理,主要是需要在MonoBehaviour的主线程中对消息处理
                if (evtHandler != null)
                {
                    evtHandler(RemoteClientEvent.Connected);
                }
                else
                {
                    Close();
                }
            });
        }
        else
        {
            Close();
        }
    }
    public void Disconnect()
    {
        Close();
    }

    //线程安全方法
    void Close()   //断开连接
    {
        if (IsClose)
        {
            return;
        }
        Debug.LogError("CloseRemoteClient");
        if (client != null)
        {
            try
            {
                if (client != null && client.Connected)
                {
                    client.Close();   //断开TCP连接
                }
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.ToString());
            }
        }
        client = null;
        IsClose = true;
        signal.Set();

        if (evtHandler != null)
        {
            ml.queueInLoop(() =>
            {
                    //这里把该匿名函数加入到MainThreadLoop的queueInLoop中进行处理,主要是需要在MonoBehaviour的主线程中对消息处理
                    evtHandler(RemoteClientEvent.Close);
            });
        }
        evtHandler = null;
        msgHandler = null;
    }

    private void RecvThread()
    {
        while (!IsClose)
        {
            StartReceive();
        }
    }

    public IEnumerator SendWaitResponse(byte[] data, uint fid, MessageHandler handler, Bundle bundle)
    {   //提供2进制消息数据流并发送,等待接收服务器的返回消息并处理
        var ret = false;
        flowHandler.Add(fid, (packet) =>
        {
            handler(packet);
            ret = true;
        });
        Send(data, bundle);
        //float passTime = 0;
        while (!ret && !IsClose)
        {
            yield return null;
            //passTime += Time.deltaTime;
        }
        if (!ret)
        {
            Debug.LogError("TimeOutRequest: " + fid + " h " + handler);
            var packet = new Packet();
            packet.responseFlag = 1;
            handler(packet);
        }
    }

    public void Send(byte[] data, Bundle bundle)  //Send命令主要把2进制数据流封装了一下,然后放入了msgBuffer的Queue中等待处理
    {
        lock (msgBuffer)
        {
            var mb = new MsgBuffer() { position = 0, buffer = data, bundle = null };
            Bundle.ReturnBundle(bundle);
            msgBuffer.Enqueue(mb);
        }
        signal.Set();
    }

}
