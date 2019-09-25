using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;

public class SaveGame : MonoBehaviour
{
    public JSONClass msgNameIdMap;
    public static SaveGame saveGame;

    void Awake()
    {
        saveGame = this; 
    }

    public void Init()
    {
        LoadMsg();
    }

    void LoadMsg()
    {
        TextAsset bindata = ABLoader.Instance.LoadABAsset(ABLoader.abPreName + "nameMap.json") as TextAsset;
        Debug.Log("nameMap " + bindata.text);
        msgNameIdMap = JSON.Parse(bindata.text).AsObject;
        Debug.Log("msgList " + msgNameIdMap.ToString());
    }

    public UtilPair GetMsgID(string msgName)   //在协议里,msgName的string是唯一的,这个id只是标识协议的id,而msgName的id是在这个协议id下的子id
    {
        foreach (KeyValuePair<string, JSONNode> m in msgNameIdMap)
        {
            if (m.Value[msgName] != null)
            {
                int a = m.Value["id"].AsInt;
                int b = m.Value[msgName].AsInt;
                return new UtilPair((byte)a, (byte)b);
            }
        }
        return null;
    }

    public string getMethodName(string module, int msgId)  //根据协议的string名字和在这个协议里的id编号,得到msg的名字(该名字唯一),注意遍历寻址msgid时要排除协议id
    {
        var msgs = msgNameIdMap[module].AsObject;
        foreach (KeyValuePair<string, JSONNode> m in msgs)
        {
            if (m.Key != "id")
            {
                if (m.Value.AsInt == msgId)
                {
                    return m.Key;
                }
            }
        }
        return null;

    }

    public string getMethodName(int moduleId, int msgId) //重载版本
    {
        var module = SaveGame.saveGame.getModuleName(moduleId);
        return getMethodName(module, msgId);
    }

    public string getModuleName(int moduleId)  //根据协议的id编号,得到协议的名字
    {
        //Debug.Log("find Module Name is " + moduleId);
        foreach (KeyValuePair<string, JSONNode> m in msgNameIdMap)
        {
            var job = m.Value.AsObject;
            if (job["id"].AsInt == moduleId)
            {
                return m.Key;
            }
        }
        //Debug.Log("name map file not found  ");
        return null;
    }
}