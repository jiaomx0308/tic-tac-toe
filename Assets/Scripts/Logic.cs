using UnityEngine;
using System.Collections;
using MyLib;
using UnityEngine.UI;
using System;

public enum GameState
{
    Idle,
    InGame,
    GameOver,
}


public class Logic : MonoBehaviour ,ILogic
{
    public static Logic Instance;

    public string msgFunction = "";
    private Text uiText;

    public int myId;
    public int pos;
    private bool isMe = false;
    private bool isMeFirst = false;

    private GameState gameState = GameState.Idle;
    private LuaManager luaManager;
    private NetworkScene network;

    void Awake() {
        Instance = this;
        luaManager = this.GetComponent<LuaManager>();
        network = this.GetComponent<NetworkScene>();

        network.Logic = this;
    }

    private void Start()
    {
       
    }

    public void Init() {
        luaManager.InitStart();

        luaManager.DoFile("Game");         //加载游戏
        luaManager.CallFunction("Game.OnInitOK"); //初始化完成 



        //isMe = false;
        //isMeFirst = false;

        //MainUI.Instance.onbutton = MatchMethod;
        //var text = MainUI.Instance.button.transform.Find("Text").GetComponent<Text>();
        //text.text = "Match";
        //uiText = text;
    }

    public void MatchMethod() {
        var cmd = CGPlayerCmd.CreateBuilder();
        cmd.Cmd = "Match";
        NetworkScene.Instance.SendPacket(cmd);

        uiText.text = "Matching";
        MainUI.Instance.onbutton = null;
    }

    public void MakeMoveMethod() {
        var cmd = CGPlayerCmd.CreateBuilder();
        cmd.Cmd = string.Format("MakeMove {0}", pos);
        NetworkScene.Instance.SendPacket(cmd);
    }


    public void GameStart() {
        uiText.text = "InGaming";
    }



    public void NewTurn(string[] cmds) {
        var curTurn  = Convert.ToInt32(cmds[1]);
        var playerId = Convert.ToInt32(cmds[2]);
        var who = "me";
        if(playerId != myId) {
            who = "other";
        }

        uiText.text = string.Format("Turn {0} {1}", curTurn, who);
        isMe = who == "me";
        if(isMe && curTurn == 0) {
            isMeFirst = true;
        }else {
            isMeFirst = false;
        }

        MainUI.Instance.onpos = OnPos;

    }

    void OnPos(int p) {
        if(isMe) {
            pos = p;
            MakeMoveMethod();
        }
    }

    public void UpdateMove(string[] cmds) {
        var playerId = Convert.ToInt32(cmds[1]);
        var pos = Convert.ToInt32(cmds[2]);
        var meToDo = playerId == myId;
        GameObject qizi;
        if(meToDo) {
            qizi = MainUI.Instance.O;
        }else {
            qizi = MainUI.Instance.X;
        }

        var copyQizi = (GameObject)GameObject.Instantiate(qizi);
        copyQizi.transform.parent = qizi.transform.parent;
        copyQizi.transform.localScale = Vector3.one;
        copyQizi.transform.localPosition = Vector3.zero;

        copyQizi.transform.localPosition = MainUI.Instance.gos[pos].transform.localPosition;
    }

    public void MsgHandler(GCPlayerCmd cmd)
    {
        if (msgFunction == "")
            throw new Exception("msgFunction is null");
        else
            luaManager.CallFunction(msgFunction, cmd);
        //switch (cmds[0])
        //{
        //    case "Init":
        //        Debug.Log("Init:" + myId);
        //        myId = Convert.ToInt32(cmds[1]);
        //        break;
        //    case "GameStart":
        //        gameState = GameState.InGame;
        //        Logic.Instance.GameStart();
        //        break;
        //    case "NewTurn":
        //        Logic.Instance.NewTurn(cmds);
        //        break;
        //    case "MakeMove":
        //        Logic.Instance.UpdateMove(cmds);
        //        break;
        //    default:
        //        break;
        //}
    }
}

