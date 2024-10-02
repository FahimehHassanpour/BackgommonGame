using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerBridge : Singleton<ServerBridge>
{
    [SerializeField]
    private GameObject FriendRequestBTN,IncomingfriendRequest;
    public void SendFriendRequest()
    {
        ServerConnector.instance.SendFriendRequest();
       
    }

    public void FriendRequestSent()
    {
        FriendRequestBTN.SetActive(false);  
    }

    public void IncomingFriendRequest()
    {
        IncomingfriendRequest.SetActive(true);
    }

    public void EnableFriendRequest()
    {
        FriendRequestBTN.SetActive(true);
    }

    public void acceptFriendRequest()
    {
        print("accepting friend request");
        ServerConnector.instance.AcceptFriedRequest();
    }

    public void acceptFriendRequestSuccess()
    {
        IncomingfriendRequest.SetActive(false);
    }
    
    
}
