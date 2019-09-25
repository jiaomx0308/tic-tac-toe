require "UICtrl"

CtrlManager = {};
local this = CtrlManager;
local ctrlList = {};

function CtrlManager.Init()
  ctrlList[CtrlNames.UI] = UICtrl.New();
  return this;
end


function CtrlManager.GetCtrl(ctrlName)
  return ctrlList[ctrlName];
end

