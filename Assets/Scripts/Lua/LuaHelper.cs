using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ManagerName
{
    public const string LuaManager = "LuaManager";
    public const string PanelManager = "PanelManager";
    public const string NetworkScene = "NetworkScene";
    public const string SaveGame = "SaveGame";
    public const string Logic = "Logic";
    public const string ABLoader = "ABLoader";
}


public static class LuaHelper
{
    public static PanelManager GetPanelManager()
    {
        return GameObjectManager.Instance.GetManager<PanelManager>(ManagerName.PanelManager);
    }

    public static LuaManager GetLuaManager()
    {
        return GameObjectManager.Instance.GetManager<LuaManager>(ManagerName.LuaManager);
    }

    public static NetworkScene GetNetworkScene()
    {
        return GameObjectManager.Instance.GetManager<NetworkScene>(ManagerName.NetworkScene);
    }

    public static SaveGame GetSaveGame()
    {
        return GameObjectManager.Instance.GetManager<SaveGame>(ManagerName.SaveGame);
    }

    public static Logic GetLogic()
    {
        return GameObjectManager.Instance.GetManager<Logic>(ManagerName.Logic);
    }
    public static ABLoader GetABLoader()
    {
        return GameObjectManager.Instance.GetManager<ABLoader>(ManagerName.ABLoader);
    }
}
