using UnityEngine;
using System.Collections;
using MyLib;
using System;


public interface ILogic
{
    void Init();
    void MsgHandler(GCPlayerCmd cmd);
}
public class NetworkScene : MonoBehaviour {
    public ILogic Logic { get; set; }
    public static NetworkScene Instance;
    private MainThreadLoop ml;
    private RemoteClient rc;
    private RemoteClientEvent lastEvt = RemoteClientEvent.None;
    

    private void Awake() {
        Instance = this;
        ml = gameObject.AddComponent<MainThreadLoop>();

        
    }

    public void StartConnect()
    {
        StartCoroutine(ConnectServer());
    }

    private IEnumerator ConnectServer() {
        rc = new RemoteClient(ml);
        rc.evtHandler = EvtHandler;
        rc.msgHandler = MsgHandler;

        rc.Connect("127.0.0.1", 12031);
        while (lastEvt == RemoteClientEvent.None)
        {
            yield return null;
        }

        //Logic.Init();
        //yield return null;
    }

    private void EvtHandler(RemoteClientEvent evt) {
        lastEvt = evt;
        Debug.Log("ClientEvent:"+evt);
    }


    public void MsgHandler(Packet packet)
    {
        var pb = packet.protoBody;
        var cmd = pb as GCPlayerCmd;
        if (Logic != null)
            Logic.MsgHandler(cmd);
        else
            Debug.LogError("NetworkScene Logic is Null");
    }


    public void SendPacket(CGPlayerCmd.Builder cg) {
        Debug.Log("BroadcastMsg: " + cg);
        if (rc != null)
        {
            Bundle bundle;
            var data = Bundle.GetPacket(cg, out  bundle);
            rc.Send(data, bundle);
        }
    }

    private void OnDestroy()
    {
        if (rc != null)
        {
            rc.Disconnect();
        }
    }
}
