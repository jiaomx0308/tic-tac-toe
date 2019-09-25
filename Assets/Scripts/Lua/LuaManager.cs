using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;

public class LuaManager : MonoBehaviour
{

    public string appname = "tic-tac-toe";
    private LuaState lua;
    //private LuaLoader loader;
    private LuaLooper loop = null;

    // Use this for initialization
    void Awake()
    {
        //loader = new LuaLoader();
        lua = new LuaState();
        //this.OpenLibs();
        lua.LuaSetTop(0);

        LuaBinder.Bind(lua);
        DelegateFactory.Init();
        LuaCoroutine.Register(lua, this);

    }

    public void InitStart()
    {
        InitLuaPath();
        InitLuaBundle();
        this.lua.Start();    //启动LUAVM
        this.StartMain();
        this.StartLooper();
    }

    void StartLooper()
    {
        loop = gameObject.AddComponent<LuaLooper>();
        loop.luaState = lua;
    }

    //cjson 比较特殊，只new了一个table，没有注册库，这里注册一下
    protected void OpenCJson()
    {
        lua.LuaGetField(LuaIndexes.LUA_REGISTRYINDEX, "_LOADED");
        lua.OpenLibs(LuaDLL.luaopen_cjson);
        lua.LuaSetField(-2, "cjson");

        lua.OpenLibs(LuaDLL.luaopen_cjson_safe);
        lua.LuaSetField(-2, "cjson.safe");
    }

    void StartMain()
    {
        //lua.DoFile("Main.lua");

        //LuaFunction main = lua.GetFunction("Main");
        //main.Call();
        //main.Dispose();
        //main = null;
    }

    /// <summary>
    /// 初始化加载第三方库
    /// </summary>
    void OpenLibs()
    {
        //lua.OpenLibs(LuaDLL.luaopen_pb);
        //lua.OpenLibs(LuaDLL.luaopen_sproto_core);
        //lua.OpenLibs(LuaDLL.luaopen_protobuf_c);
        //lua.OpenLibs(LuaDLL.luaopen_lpeg);
        //lua.OpenLibs(LuaDLL.luaopen_bit);
        //lua.OpenLibs(LuaDLL.luaopen_socket_core);

        //this.OpenCJson();
    }

    /// <summary>
    /// 初始化Lua代码加载路径
    /// </summary>
    void InitLuaPath()
    {
        lua.AddSearchPath(Application.persistentDataPath + "/Lua");
        lua.AddSearchPath(Application.persistentDataPath + "/lua");
    }

    /// <summary>
    /// 初始化LuaBundle
    /// </summary>
    void InitLuaBundle()
    {
        //object[] luaFiles = Resources.LoadAll("Lua", typeof(TextAsset));
        //Debug.LogError(luaFiles.Length);
        //foreach (var i in luaFiles)
        //    Debug.LogError(i.GetType().ToString() + i.ToString() );

        //Debug.LogError(luaFiles.Length);

        //if (loader.beZip)
        //{
        //    loader.AddBundle("lua/lua.unity3d");
        //    loader.AddBundle("lua/lua_math.unity3d");
        //    loader.AddBundle("lua/lua_system.unity3d");
        //    loader.AddBundle("lua/lua_system_reflection.unity3d");
        //    loader.AddBundle("lua/lua_unityengine.unity3d");
        //    loader.AddBundle("lua/lua_common.unity3d");
        //    loader.AddBundle("lua/lua_logic.unity3d");
        //    loader.AddBundle("lua/lua_view.unity3d");
        //    loader.AddBundle("lua/lua_controller.unity3d");
        //    loader.AddBundle("lua/lua_misc.unity3d");

        //    loader.AddBundle("lua/lua_protobuf.unity3d");
        //    loader.AddBundle("lua/lua_3rd_cjson.unity3d");
        //    loader.AddBundle("lua/lua_3rd_luabitop.unity3d");
        //    loader.AddBundle("lua/lua_3rd_pbc.unity3d");
        //    loader.AddBundle("lua/lua_3rd_pblua.unity3d");
        //    loader.AddBundle("lua/lua_3rd_sproto.unity3d");
        //}
    }

    public void DoFile(string filename)
    {
        lua.DoFile(filename);
    }

    public void DoString(string luaString)
    {
        lua.DoString(luaString);
    }

    // Update is called once per frame
    public object[] CallFunction(string funcName, params object[] args)
    {
        LuaFunction func = lua.GetFunction(funcName);
        if (func != null)
        {
            //return func.Call(args);
            return func.LazyCall(args);
        }
        return null;
    }

    public void LuaGC()
    {
        lua.LuaGC(LuaGCOptions.LUA_GCCOLLECT);
    }

    public void Close()
    {
        loop.Destroy();
        loop = null;

        lua.Dispose();
        lua = null;
        //loader = null;
    }
}
