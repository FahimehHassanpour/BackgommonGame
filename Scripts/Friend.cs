using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Friend : MonoBehaviour,IPointerClickHandler
{
    private string username,userId;
    public TextMeshProUGUI frindsName;
   
    public void setFriendsName(string val,string id)
    {
        username = val;
        frindsName.text = val;
        userId = id;
        print(userId);
    }

   
    
    public void AskForGame()
    {
        print("friendReq");
        FriendMenuManager.FriendGameList obj = new FriendMenuManager.FriendGameList();
        FriendMenuManager.instance.selectedFriend = obj;
        obj.username = UserProfile.instance.getUserName();
        obj.friendid = userId;
        obj.tag = "reqFrndgame";
        obj.friendName = username;
        UserProfile.instance.SetOppUserName(username);  
        obj.UserToken= PlayerPrefs.GetString("token");
        obj.level = UserProfile.instance.GetLevel();
        
        ServerConnector.instance.GetRoomsFromServer(true);
        homePage.instance.OpenFriendPage(obj);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AskForGame();
    }
}
