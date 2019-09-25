﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class LogicWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(Logic), typeof(UnityEngine.MonoBehaviour));
		L.RegFunction("Init", Init);
		L.RegFunction("MatchMethod", MatchMethod);
		L.RegFunction("MakeMoveMethod", MakeMoveMethod);
		L.RegFunction("GameStart", GameStart);
		L.RegFunction("NewTurn", NewTurn);
		L.RegFunction("UpdateMove", UpdateMove);
		L.RegFunction("MsgHandler", MsgHandler);
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("Instance", get_Instance, set_Instance);
		L.RegVar("msgFunction", get_msgFunction, set_msgFunction);
		L.RegVar("myId", get_myId, set_myId);
		L.RegVar("pos", get_pos, set_pos);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Init(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			Logic obj = (Logic)ToLua.CheckObject<Logic>(L, 1);
			obj.Init();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int MatchMethod(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			Logic obj = (Logic)ToLua.CheckObject<Logic>(L, 1);
			obj.MatchMethod();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int MakeMoveMethod(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			Logic obj = (Logic)ToLua.CheckObject<Logic>(L, 1);
			obj.MakeMoveMethod();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GameStart(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			Logic obj = (Logic)ToLua.CheckObject<Logic>(L, 1);
			obj.GameStart();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int NewTurn(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Logic obj = (Logic)ToLua.CheckObject<Logic>(L, 1);
			string[] arg0 = ToLua.CheckStringArray(L, 2);
			obj.NewTurn(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int UpdateMove(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Logic obj = (Logic)ToLua.CheckObject<Logic>(L, 1);
			string[] arg0 = ToLua.CheckStringArray(L, 2);
			obj.UpdateMove(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int MsgHandler(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Logic obj = (Logic)ToLua.CheckObject<Logic>(L, 1);
			MyLib.GCPlayerCmd arg0 = (MyLib.GCPlayerCmd)ToLua.CheckObject(L, 2, typeof(MyLib.GCPlayerCmd));
			obj.MsgHandler(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int op_Equality(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.ToObject(L, 1);
			UnityEngine.Object arg1 = (UnityEngine.Object)ToLua.ToObject(L, 2);
			bool o = arg0 == arg1;
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Instance(IntPtr L)
	{
		try
		{
			ToLua.Push(L, Logic.Instance);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_msgFunction(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			Logic obj = (Logic)o;
			string ret = obj.msgFunction;
			LuaDLL.lua_pushstring(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index msgFunction on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_myId(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			Logic obj = (Logic)o;
			int ret = obj.myId;
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index myId on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_pos(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			Logic obj = (Logic)o;
			int ret = obj.pos;
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index pos on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_Instance(IntPtr L)
	{
		try
		{
			Logic arg0 = (Logic)ToLua.CheckObject<Logic>(L, 2);
			Logic.Instance = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_msgFunction(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			Logic obj = (Logic)o;
			string arg0 = ToLua.CheckString(L, 2);
			obj.msgFunction = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index msgFunction on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_myId(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			Logic obj = (Logic)o;
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			obj.myId = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index myId on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_pos(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			Logic obj = (Logic)o;
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			obj.pos = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index pos on a nil value");
		}
	}
}

