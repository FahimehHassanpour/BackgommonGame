using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameRoom : MonoBehaviour
{
    public int GameCost;
    public TextMeshProUGUI RooPriceTag;
    public Button btn;
    public GameObject stop;
    private void Start()
    {
        RooPriceTag.text = UserProfile.instance.TurnNumberToDecimalpointSeperator(GameCost);
    }


    public int roomId;
    public void SetUpRoom(int point,int roomid)
    {

        GameCost = point;
        roomId = roomid;
        RooPriceTag.text = GameCost.ToString();

    }



    public void disableGameRoom()
    {
        btn.interactable = false;
        stop.SetActive(true);
    }

    public void enableRoom()
    {
        btn.interactable = true;
        stop.SetActive(false);
    }
    
    public void OpenRoom()
    {
        //homePage.instance.ShowPlayConfirm();
        //PlayerPrefs.SetInt("fee",GameCost);
        MenuAudioManager.instance.playClick();
        ServerGameReq.instance.room = GameCost;
        ServerGameReq.instance.roomId = roomId;
        ServerGameReq.instance.RequestForGame();


    }

    
    
}
