require "define"
require "CtrlManager"

Game = {};
local this = Game;


function Game.OnInitOK()
  print("Start Init lua");
	for i = 1, #PanelNames do
		require (tostring(PanelNames[i]))
	end
  
  CtrlManager.Init();
  local ctrl = CtrlManager.GetCtrl(CtrlNames.UI);
  if ctrl ~= nil then
      ctrl:Awake();
  end
       
  print('LuaFramework InitOK--->>>');
end
  