using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LuaInterface;

public class PanelManager : MonoBehaviour
{
    private Transform parent;

    Transform Parent
    {
        get
        {
            if (parent == null)
            {
                GameObject go = this.transform.root.Find("Canvas").gameObject;
                if (go != null) parent = go.transform;
            }
            return parent;
        }
    }
    public void CreatePanel(string name, LuaFunction func = null)
    {
        string assetName = name + "Panel.prefab";
        //string abName = name.ToLower() + AppConst.ExtName;
        if (Parent.Find(assetName) != null) return;

        GameObject prefab = ABLoader.Instance.LoadABAsset(ABLoader.abPreName + assetName) as GameObject;
        GameObject go = Instantiate(prefab);
        go.name = assetName;
        go.layer = LayerMask.NameToLayer("Default");
        go.transform.SetParent(Parent);
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;
        go.AddComponent<LuaUIBehaviour>();

        if (func != null) func.Call(go);
        Debug.LogWarning("CreatePanel::>> " + name + " " + prefab);
    }

    /// <summary>
    /// �ر����
    /// </summary>
    /// <param name="name"></param>
    public void ClosePanel(string name)
    {
        var panelName = name + "Panel";
        var panelObj = Parent.Find(panelName);
        if (panelObj == null) return;
        Destroy(panelObj.gameObject);
    }
}
