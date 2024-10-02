using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomSuggest : MonoBehaviour,IPointerClickHandler
{
  public int Room,point;
    public int RoomId;
  public TextMeshProUGUI roomPrice;

  

  public void SetupRoom(int roomId,int points)
  {
        RoomId = roomId;
        point = points;
        roomPrice.text = UserProfile.instance.TurnNumberToDecimalpointSeperator( point);
        if(points>UserProfile.instance.GetCoin())
        {
            gameObject.SetActive(false);
        }
        
  }

  public void SelectRoom()
  {
    homePage.instance.SetRoomSuggestPrice(point);
    FriendMenuManager.instance.selectedFriend.roomId = RoomId;
    FriendMenuManager.instance.selectedFriend.room = point;
  }

    public void OnPointerClick(PointerEventData eventData)
    {
       SelectRoom();
    }
}
