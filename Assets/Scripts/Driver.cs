using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour
{
    void Awake()
    {
        GameObjectManager.Instance.AddManager<SaveGame>(ManagerName.SaveGame);
        GameObjectManager.Instance.AddManager<ABLoader>(ManagerName.ABLoader);
        GameObjectManager.Instance.AddManager<LuaManager>(ManagerName.LuaManager);
        GameObjectManager.Instance.AddManager<PanelManager>(ManagerName.PanelManager);
        GameObjectManager.Instance.AddManager<NetworkScene>(ManagerName.NetworkScene);
        GameObjectManager.Instance.AddManager<Logic>(ManagerName.Logic);
        
    }

    private void OnGUI()
    {
        GUILayout.TextArea("123");
    }
}
