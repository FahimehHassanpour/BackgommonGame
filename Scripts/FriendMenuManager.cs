using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class FriendMenuManager : Singleton<FriendMenuManager>
{
   public  Transform friendList,RequestList;
   public Friend FriendPrefab;
   public FrienRequest FriendRqPreFab;
   public GameObject FriendLobby,incomingGameReq,incomingfriendGameDetails;
   public FriendGameList selectedFriend;
   public TextMeshProUGUI incomingFriend, roomPrice;
   public FriendSuggestion IncomingFriend;
   public TMP_InputField FriendSearchInput;
   public GameObject SearchResBtns;
   public GameObject FriendReqAlaram;
   private List<string> friendsId;
   private List<string> friendsReqs;

   private void Start()
   {
      friendsId = new List<string>();
      friendsReqs = new List<string>();
   }

   public void AddFriendToList(string userName,string UserId)
   {
      if (friendsId.Contains(UserId))
      {
         return;
      }
      friendsId.Add(UserId);
      
      Friend temp = Instantiate(FriendPrefab, friendList);
      temp.setFriendsName(userName,UserId);
   }

   public void AddFriendRequests(string requesterName,string requesterId)
   {
      if (friendsReqs.Contains(requesterId))
      {
         return;
      }
      friendsReqs.Add(requesterId);
      FrienRequest temp = Instantiate(FriendRqPreFab, RequestList);
      temp.SetRequest(requesterName,requesterId);
      FriendMenuManager.instance.FriendReqAlaram.SetActive(true);
   }

   public void acceptFriendship(string friendId,String UserName)
   {
      FriendGameList obj = new FriendGameList();
      obj.tag = "fr-Req-accept";
      obj.friendid = friendId;
      obj.username = UserName;
      ServerConnector.instance.SendWebSocketMessage(JsonConvert.SerializeObject(obj));
   }

   public void RejectFriendship(string friendId,String UserName)
   {
      FriendGameList obj = new FriendGameList();
      obj.tag = "fr-Req-reject";
      obj.friendid = friendId;
      obj.username = UserName;
      ServerConnector.instance.SendWebSocketMessage(JsonConvert.SerializeObject(obj));
   }
   

   public GameObject SearchResultFrame;
   public TextMeshProUGUI searchReport;
   public void SearchResultOpen(string result,FriendGameList obj)
   {
      SearchResBtns.SetActive(false);
      SearchResultFrame.SetActive(true);
      if (result == "")
      {
         searchReport.text = "No Result";
      }else if (result == "not found")
      {
         searchReport.text = obj.friendName+" found, But request already sent!";
      }
      else
      {
         searchReport.text = result+" found, Do you want to send friendship request?";
         selectedFriend = obj;
         SearchResBtns.SetActive(true);
      }
   }

   public void SearchresultClose()
   {
      SearchResultFrame.SetActive(false);
      selectedFriend = null;
   }

   public void SendFriendRequest()
   {
      SearchResultFrame.SetActive(false);
      FriendGameList obj = new FriendGameList();
      obj.tag = "frshpReqBySrch";
      obj.username = UserProfile.instance.getUserName();
      obj.friendid = selectedFriend.friendid;
      obj.friendName = selectedFriend.friendName;
      ServerConnector.instance.SendWebSocketMessage(JsonConvert.SerializeObject(obj));

   }
   
   
   

   public void StartSearch()
   {
      string userToSearch = FriendSearchInput.text;
      FriendGameList obj = new FriendGameList();
      obj.tag = "searchFriend";
      obj.friendName = userToSearch;
      obj.username = UserProfile.instance.getUserName();
      print(userToSearch);
      ServerConnector.instance.SendWebSocketMessage(JsonConvert.SerializeObject(obj));



   }

   public TextMeshProUGUI username, OppName;

   public void OpenFriendLobby()
   {
      username.text = UserProfile.instance.getUserName();
      OppName.text = instance.selectedFriend.friendName;
      FriendLobby.SetActive(true);
   }

   public void CancelGameRequest()
   {
      FriendLobby.SetActive(false);
      ServerConnector.MessageTag ob = new ServerConnector.MessageTag();
      ob.tag = "cancelfr";
      ServerConnector.instance.SendWebSocketMessage(JsonConvert.SerializeObject(ob));
   }

    public GameObject AcceptBtn;
   public void ShowIncomingGameDetails()
   {
      incomingGameReq.SetActive(false);
      incomingfriendGameDetails.SetActive(true);
      incomingFriend.text = IncomingFriend.friend;
      roomPrice.text = IncomingFriend.room.ToString();
        print(UserProfile.instance.GetCoin());
      if(IncomingFriend.room> UserProfile.instance.GetCoin())
        {
            AcceptBtn.SetActive(false);
        }
        else
        {
            AcceptBtn.SetActive(true);
        }
   }

   public void hideIncomingGameDetails()
   {
      incomingfriendGameDetails.SetActive(false);
      incomingFriend.text = "";
      roomPrice.text = "";
   }
   
   public void IncomingGameRequest(FriendSuggestion incomingfriend)
   {
      incomingGameReq.SetActive(true);
      IncomingFriend = incomingfriend;

   }

   public class AcceptFriendGameClass
   {
      public string tag;
      public string friendname;
      public string username;
      public int level;
      public int room;
   }

   public void RejectFriendGame()
   {
      incomingfriendGameDetails.SetActive(false);
      AcceptFriendGameClass obj = new AcceptFriendGameClass();
      obj.tag = "rejectinv";
      obj.friendname = IncomingFriend.friend;
      obj.username = UserProfile.instance.getUserName();
      obj.level = UserProfile.instance.GetLevel();
      obj.room = IncomingFriend.room;
      print("rejecting game request sent by"+obj.friendname);
      
      ServerConnector.instance.SendWebSocketMessage(JsonConvert.SerializeObject(obj));
   }
   
   
   public void AcceptFriendGame()
   {
      FriendMenuManager.FriendGameList obj = new FriendMenuManager.FriendGameList();    
      incomingfriendGameDetails.SetActive(false);
      
      obj.tag = "acceptFriendGame";
      obj.friendName = IncomingFriend.friend;
      obj.username = UserProfile.instance.getUserName();
      obj.level = UserProfile.instance.GetLevel();
      obj.room = IncomingFriend.room;
      obj.UserToken = PlayerPrefs.GetString("token");
      obj.roomId = IncomingFriend.roomid;
      
        print("accepting game request sent by"+obj.friendName);
      
      ServerConnector.instance.SendWebSocketMessage(JsonConvert.SerializeObject(obj));
   }
   
   public void CloseFriendLobby()
   {
      FriendLobby.SetActive(false);
   }
   
   public class FriendSuggestion
   {
      public string friend;
      public int room;
      public int roomid;
      public int level;
     
   }
   
   
   
   public class FriendGameList
   {
      public string tag;
      public string friendid;
      public string username;
      public string friendName;
      public int room;        
      public int userId;        
      public int level;
      public int roomId;
      public string UserToken;

    }
}
