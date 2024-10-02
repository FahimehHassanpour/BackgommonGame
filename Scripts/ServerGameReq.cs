using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ServerGameReq : Singleton<ServerGameReq>
{
    public TextMeshProUGUI User,userLevel, Opponent,opponentLevel,Bet;

    

    public class GameRequest
    {
        public string userName;
        public int userId;
        public string tag;
        public int level;
        public int room;
        public int roomId;
        public string UserToken;
    }

    public string UserName;
    public int UserId,level,room,roomId;
    public string Message;
    public void RequestForGame()
    {
        GameRequest request = new GameRequest();
        request.UserToken = PlayerPrefs.GetString("token");
        request.userId = UserId;
        request.userName = UserProfile.instance.getUserName();
        request.tag = "reqGame";
        request.level = UserProfile.instance.GetLevel();
        request.room = room;
        request.roomId = roomId;
        if (ServerConnector.instance.ws != null)
        {
            var message=JsonUtility.ToJson(request);
            ServerConnector.instance.SendWebSocketMessage(message);
            //print(message);
        }
    }
    private Action<bool> sendMessage;
    public void RejectGame()
    {
        MenuAudioManager.instance.playClick();
        homePage.instance.HomePageOn();
        
        
        ServerConnector.MessageTag tag = new ServerConnector.MessageTag();
        tag.tag = "ignoreNewGame";
        ServerConnector.instance.SendWebSocketMessage(JsonUtility.ToJson(tag));
    }

    
    
 
}
