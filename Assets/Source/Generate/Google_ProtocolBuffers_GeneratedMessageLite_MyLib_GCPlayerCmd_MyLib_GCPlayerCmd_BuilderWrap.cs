﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class Google_ProtocolBuffers_GeneratedMessageLite_MyLib_GCPlayerCmd_MyLib_GCPlayerCmd_BuilderWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(Google.ProtocolBuffers.GeneratedMessageLite<MyLib.GCPlayerCmd,MyLib.GCPlayerCmd.Builder>), typeof(Google.ProtocolBuffers.AbstractMessageLite<MyLib.GCPlayerCmd,MyLib.GCPlayerCmd.Builder>), "GeneratedMessageLite_MyLib_GCPlayerCmd_MyLib_GCPlayerCmd_Builder");
		L.RegFunction("ToString", ToString);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ToString(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			Google.ProtocolBuffers.GeneratedMessageLite<MyLib.GCPlayerCmd,MyLib.GCPlayerCmd.Builder> obj = (Google.ProtocolBuffers.GeneratedMessageLite<MyLib.GCPlayerCmd,MyLib.GCPlayerCmd.Builder>)ToLua.CheckObject<Google.ProtocolBuffers.GeneratedMessageLite<MyLib.GCPlayerCmd,MyLib.GCPlayerCmd.Builder>>(L, 1);
			string o = obj.ToString();
			LuaDLL.lua_pushstring(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}
