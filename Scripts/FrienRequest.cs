using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FrienRequest : MonoBehaviour
{
   public string RequesterName, RequesterId;
   public TextMeshProUGUI requesterName;

   public void SetRequest(string requesterName, string RequesterId)
   {
      RequesterName = requesterName;
      this.RequesterId = RequesterId;
      this.requesterName.text = requesterName;
   }

   public void AcceptRequest()
   {
      FriendMenuManager.instance.acceptFriendship(RequesterId,UserProfile.instance.getUserName());
      RemoveRequestFromList();
      
   }

   public void rejectRequest()
   {
      FriendMenuManager.instance.RejectFriendship(RequesterId,UserProfile.instance.getUserName());
      RemoveRequestFromList();
   }

   public void RemoveRequestFromList()
   {
      Destroy(gameObject);
   }
   
}
