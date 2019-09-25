using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameObjectManager
{
    private static GameObjectManager _instance;
    static GameObject m_GameManager;
    static Dictionary<string, object> m_Managers = new Dictionary<string, object>();

    private GameObjectManager() { }
    public static GameObjectManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObjectManager();
            }
            return _instance;
        }
    }

    GameObject GameManager
    {
        get
        {
            if (m_GameManager == null)
            {
                m_GameManager = GameObject.Find("GameManager");
            }
            return m_GameManager;
        }
    }

    public T AddManager<T>(string typeName) where T : Component
    {
        object result = null;
        m_Managers.TryGetValue(typeName, out result);
        if (result != null)
        {
            return (T)result;
        }
        Component c = GameManager.AddComponent<T>();
        m_Managers.Add(typeName, c);
        return default(T);
    }

    public T GetManager<T>(string typeName) where T : class
    {
        if (!m_Managers.ContainsKey(typeName))
        {
            return default(T);
        }
        object manager = null;
        m_Managers.TryGetValue(typeName, out manager);
        return (T)manager;
    }

    public void RemoveManager(string typeName)
    {
        if (!m_Managers.ContainsKey(typeName))
        {
            return;
        }
        object manager = null;
        m_Managers.TryGetValue(typeName, out manager);
        Type type = manager.GetType();
        if (type.IsSubclassOf(typeof(MonoBehaviour)))
        {
            GameObject.Destroy((Component)manager);
        }
        m_Managers.Remove(typeName);
    }
}
