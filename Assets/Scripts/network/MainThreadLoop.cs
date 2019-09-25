using System.Threading;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public interface IMainLoop
{
    void queueInLoop(Action cb);
    void queueInUpdate(Action cb);
    void removeUpdate(Action cb);
}

public class MainThreadLoop : MonoBehaviour, IMainLoop
{
    private Queue<Action> pendingCallbacks = new Queue<Action>();
    private List<Action> updateCb = new List<Action>();
    private List<Action> removeCb = new List<Action>();

    private double lastUpdateTime = 0;

    public void queueInLoop(System.Action cb)
    {
        lock (pendingCallbacks)
        {
            pendingCallbacks.Enqueue(cb);
        }
    }
    //非线程安全
    public void queueInUpdate(System.Action cb)
    {
        updateCb.Add(cb);
    }
    public void removeUpdate(System.Action cb)
    {
        //updateCb.Remove(cb);
        removeCb.Add(cb);
    }

    private void Update()
    {
        lock (pendingCallbacks)
        {
            while (pendingCallbacks.Count > 0)
            {
                var p = pendingCallbacks.Dequeue();
                try
                {
                        p();
                }
                catch (Exception e)
                {
                    Debug.LogError("CallBacks: " + e.ToString());
                }
            }
        }

        foreach (var c in updateCb)
        {
            try
            {
                c();
            }
            catch (Exception exp)
            {
                Debug.LogError("UpdateCB: " + exp.ToString());
            }
        }
        //删除安全
        foreach (var c in removeCb)
        {
            updateCb.Remove(c);
        }
        removeCb.Clear();
    }

}
