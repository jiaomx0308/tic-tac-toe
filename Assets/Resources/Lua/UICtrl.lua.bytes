UICtrl = {};
local this = UICtrl;

local panelMgr = LuaHelper.GetPanelManager();
local networkMgr = LuaHelper.GetNetworkScene();
local logicMgr = LuaHelper.GetLogic();

--Panel属性
local gameObject;
local transform;
local uiLogic;

--逻辑相关
local myId;
local isMe;
local pos
local pieceFunc
local piecePosMap = {};

function UICtrl.New()
  print(UICtrl.New)
    return this;
  end
  
  
  function UICtrl.Awake()
    panelMgr:CreatePanel("UI", this.OnCreate);
  end
  
  function UICtrl.OnCreate(obj)
    gameObject = obj;
    transform = obj.transform;
    
    uiLogic = transform:GetComponent('LuaUIBehaviour');
    this.InitPanel()

    logicMgr.msgFunction = "UICtrl.MsgHandler";
    uiLogic:AddClick(this.Button, this.MatchMethod);
    
    networkMgr:StartConnect();
  end
  
  function UICtrl.InitPanel()
    this.Button = transform:Find("Button").gameObject;
    this.O = transform:Find("O").gameObject;
    this.X = transform:Find("X").gameObject;
    this.Pos = transform:Find("Pos1").gameObject;
    this.uiText = this.Button.transform:Find("Text"):GetComponent('Text');
    
    this.uiText.text = "Match";
    
    local initpos = this.Pos.transform.localPosition;
    local offX = 258;
    local offY = 120;
    for i = 1 , 9 do
      local col = (i - 1)  % 3;
      local row = math.ceil(i / 3) - 1;
      local np = GameObject.Instantiate(this.Pos);
      np.name = "Pos"..tostring(i);
      np.transform.parent = this.Pos.transform.parent;
      np.transform.localScale = Vector3.one;
      np.transform.localPosition = Vector3.zero;
      
      local newPos = initpos + Vector3(offX*col, -offY*row ,0);
      np.transform.localPosition = newPos;
      
      uiLogic:AddClick(np, function ()
                              this.OnPieceClick(i)
                           end);
      piecePosMap[tostring(i)] = newPos;
    end
    this.Pos.gameObject:SetActive(false);
  end
  
  function UICtrl.MsgHandler(cmd)
    local cmds = Split(cmd.Result, " ");
    local switch = {
      ["Init"] = function ()
        UICtrl.Init(cmds);
      end,
      ["GameStart"] = function()
        UICtrl.GameStart(cmds);
      end,
      ["NewTurn"] = function()
        UICtrl.NewTurn(cmds);
      end,
      ["MakeMove"] = function()
        UICtrl.UpdateMove(cmds);
      end,
    }
    func = switch[cmds[1]];
    if func then
      func()
    end
  end
  
  
  function UICtrl.MatchMethod()
        local cmd = CGPlayerCmd.CreateBuilder();
        cmd.Cmd = "Match";
        networkMgr:SendPacket(cmd);

        this.uiText.text = "Matching";
        uiLogic:RemoveClick(this.Button);
  end
  
  function UICtrl.OnPieceClick(id)
    if pieceFunc then
      pieceFunc(id);
    end
  end
  
  
  
  function UICtrl.Init(cmds)
    print(cmds[2].."-------------------------")
    myId = cmds[2];
  end
  
  function UICtrl.GameStart(cmds)
    this.uiText.text = "InGaming";
  end
  
  function UICtrl.NewTurn(cmds)
    local curTurn = cmds[2];
    local playerId = cmds[3];
    
    local who = "me";
    isMe = true;
    if playerId ~= myId then
      who = "other";
      isMe = false;
    end

    this.uiText.text = "Turn "..tostring(curTurn).." "..who;
    
    pieceFunc = function(id)
                  if isMe then
                    pos = id;
                    UICtrl.MakeMoveMethod()
                  end
                end
  end
  
    
  function UICtrl.MakeMoveMethod()
    local cmd = CGPlayerCmd.CreateBuilder();
    cmd.Cmd = "MakeMove "..tostring(pos);
    networkMgr:SendPacket(cmd);
  end
  
  
 function UICtrl.UpdateMove (cmds)
   local playerId = cmds[2];
   local posPiece = cmds[3];
   
   local isMePlayPiece = false;
   if playerId == myId then
     isMePlayPiece = true;
   end
   
   local piece;
   if isMePlayPiece then
     piece = this.O;
   else
     piece = this.X;
   end
   
   copyPiece = GameObject.Instantiate(piece);
   copyPiece.transform.parent = piece.transform.parent;
   copyPiece.transform.localScale = Vector3.one;
   copyPiece.transform.localPosition = piecePosMap[tostring(posPiece)];
   
 end
  
  
  --字符串切割
function Split(szFullString, szSeparator)  
  local nFindStartIndex = 1  
  local nSplitIndex = 1  
  local nSplitArray = {}  
  while true do  
    local nFindLastIndex = string.find(szFullString, szSeparator, nFindStartIndex)  
    if not nFindLastIndex then  
      nSplitArray[nSplitIndex] = string.sub(szFullString, nFindStartIndex, string.len(szFullString))  
      break  
    end  
    nSplitArray[nSplitIndex] = string.sub(szFullString, nFindStartIndex, nFindLastIndex - 1)  
    nFindStartIndex = nFindLastIndex + string.len(szSeparator)  
    nSplitIndex = nSplitIndex + 1  
  end  
  return nSplitArray  
end  
  