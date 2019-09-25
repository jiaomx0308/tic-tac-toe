using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;
using UnityEngine.UI;

public class LuaUIBehaviour : MonoBehaviour
{
    private Dictionary<string, LuaFunction> buttons = new Dictionary<string, LuaFunction>();

    public void AddClick(GameObject go, LuaFunction luafunc)
    {
        if (go == null || luafunc == null) return;
        buttons.Add(go.name, luafunc);
        go.GetComponent<Button>().onClick.AddListener(
            delegate () {
                luafunc.Call(go);
            }
        );
    }

    public void RemoveClick(GameObject go)
    {
        if (go == null) return;
        LuaFunction luafunc = null;
        if (buttons.TryGetValue(go.name, out luafunc))
        {
            //luafunc.Dispose();
            //luafunc = null;
            buttons.Remove(go.name);
        }
    }


    public void ClearClick()
    {
        foreach (var de in buttons)
        {
            if (de.Value != null)
            {
                de.Value.Dispose();
            }
        }
        buttons.Clear();
    }
}
