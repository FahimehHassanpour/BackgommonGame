using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class homePage : Singleton<homePage>
{
  public HomeMessageBoard messageboard;
  public TextMeshProUGUI coin, UserName,level;
  public Image UserImage;
  public GameObject PlayConfirm,FriendPlayButton,HomePagePanel;
  public Transform FrindListFrame,FriendListPlaceHolder,FriendPage,FriendPageHolder;
  private Vector3 friendListOrigin,friendpageOrigin;
  public Transform RoomParent;
  private bool frienlistOpen;
  public TextMeshProUGUI friendsName,room;
  public GameObject _Shop, ShopContentParent;
  public GameObject FriendRoomPanel,RoomPanel,LeftArrow,RightArrow,LobbyEffect,friendlyLobbyEffect;

    
    public void HomePageOff()
  {
        HomePagePanel.SetActive(false);
  }

  public void HomePageOn()
    {
        HomePagePanel.SetActive(true);
    }
   
  public void clickOnFriendList()
  {
    FriendMenuManager.instance.FriendReqAlaram.SetActive(false);
    if (frienlistOpen)
    {
      CloseFriendList();
    }
    else
    {
      OpenFriendList();
    }
    frienlistOpen = !frienlistOpen;
    ServerConnector.instance.AskForListOfFriends();
    ServerConnector.instance.AskForFriendShipRequests();
    print("4");
    ServerConnector.instance.AskforUserStats();
  }

  public void SetRoomSuggestPrice(int room)
  {
        print(room.ToString());

    this.room.text = UserProfile.instance.TurnNumberToDecimalpointSeperator( room.ToString());
    FriendMenuManager.instance.selectedFriend.room = room;
    FriendPlayButton.SetActive(true);
  }

  public void OpenShop()
  {
    Login.instance.GetUserAbility();
    foreach (var VARIABLE in ShopContentParent.GetComponentsInChildren<SHopItem>())
    {
      VARIABLE.GetComponent<CanvasGroup>().alpha = 0;
    }
    _Shop.GetComponent<CanvasGroup>().alpha = 0;
    StartCoroutine(StartShop());
    _Shop.SetActive(true);
  }

  public void CloseShop()
  {
    _Shop.SetActive(false);
  }

  IEnumerator StartShop()
  {
    LeanTween.alphaCanvas(_Shop.GetComponent<CanvasGroup>(), 1, 0.6f);
    yield return new WaitForSeconds(0.6f);
    foreach (var VARIABLE in ShopContentParent.GetComponentsInChildren<SHopItem>())
    {
      LeanTween.alphaCanvas(VARIABLE.GetComponent<CanvasGroup>(), 1, 0.3f);
      yield return new WaitForSeconds(0.12f);
            MenuAudioManager.instance.PlaySwoosh();
    }
  }

  public void SuggestGame()
  {
    CloseFriendPage();
    CloseFriendList();
    ServerConnector.instance.SendWebSocketMessage(JsonConvert.SerializeObject(FriendMenuManager.instance.selectedFriend));
  }

  public void OpenFriendPage(FriendMenuManager.FriendGameList obj)
  {
    LeanTween.move(FriendPage.gameObject, FriendPageHolder, 0.2f);
    friendsName.text = obj.friendName;
  }

  public void CloseFriendPage()
  {
    LeanTween.move(FriendPage.gameObject, friendpageOrigin, 0.2f);
  }
  
  
  public void OpenFriendList()
  {
    LeanTween.move(FrindListFrame.gameObject, FriendListPlaceHolder, 0.2f);
  }

  public void CloseFriendList()
  {
    LeanTween.move(FrindListFrame.gameObject, friendListOrigin, 0.2f);
  }
  
  
  public void ExitGame()
  {
    Application.Quit();
  }
  
  
  private void Start()
  {
    friendListOrigin = FrindListFrame.transform.position;
    friendpageOrigin = FriendPage.transform.position;
    
  }

[ContextMenu("roomAnim")]
  public void RoomAnimation()
  {
        print("roomAnimation");
    StartCoroutine(Co_RoomInit());
  }

  public void PanelMove(int target)
  {
        if(PanelMover!=null)
        {
            StopCoroutine(PanelMover);
        }
        StartCoroutine(CoPanelMove(target));
  }
    Coroutine PanelMover;
    public ScrollRect Panel;
    IEnumerator CoPanelMove(int target)
    {
        MenuAudioManager.instance.PlaySwoosh();
        float start = Panel.horizontalNormalizedPosition;
        float starttime = Time.time;
        bool move = true;
        while(move)
        {
            float t = (Time.time - starttime)/0.15f;
            float lerp = Mathf.Lerp(start, target,t);
            Panel.horizontalNormalizedPosition =lerp;
            yield return null;
            if(t>1)
            {
                move = false;
            }
        }
        MenuAudioManager.instance.playClick();
    }

  IEnumerator Co_RoomInit()
  {
    Vector3 rightOrigin = RightArrow.transform.position;
    Vector3 LeftArrowOrigin = LeftArrow.transform.position;
    RoomPanel.GetComponent<CanvasGroup>().alpha = 0;
    yield return new WaitForSeconds(0.3f);
    RoomPanel.SetActive(true);
    RightArrow.SetActive(true);
    LeftArrow.SetActive(true);
    RoomPanel.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
    LeanTween.scale(RoomPanel, Vector3.one, 0.8f);
    
    
    
    RightArrow.GetComponent<CanvasGroup>().alpha = 0;
    LeftArrow.GetComponent<CanvasGroup>().alpha = 0;
    
    yield return null;
    RightArrow.transform.position += new Vector3(300, 0, 0);
    LeftArrow.transform.position += new Vector3(-300, 0, 0);
    yield return null;
    LeanTween.alphaCanvas(RoomPanel.GetComponent<CanvasGroup>(), 1, 1f).setOnComplete(() =>
    {
      LeanTween.alphaCanvas(RightArrow.GetComponent<CanvasGroup>(), 1, 0.4f);
      LeanTween.alphaCanvas(LeftArrow.GetComponent<CanvasGroup>(), 1, 0.4f);
      LeanTween.move(RightArrow, rightOrigin, 0.3f);
      LeanTween.move(LeftArrow, LeftArrowOrigin, 0.3f);
    });
  }
  
  
  

  public Slider levelLider;
  public void SetUserUi()
  {
    
    coin.text = UserProfile.instance.TurnNumberToDecimalpointSeperator( UserProfile.instance.GetCoin());
    level.text ="Level: "+ UserProfile.instance.TurnNumberToDecimalpointSeperator((UserProfile.instance.GetLevel()/150));
    UserName.text = UserProfile.instance.getUserName();
    levelLider.value = Mathf.InverseLerp(0, 150, UserProfile.instance.GetLevel() % 150);  
    foreach (var VARIABLE in RoomParent.GetComponentsInChildren<GameRoom>())                   
    {
      if (VARIABLE.GameCost > UserProfile.instance.GetCoin())
      {
        VARIABLE.disableGameRoom();
      }
      else
      {
        VARIABLE.enableRoom();
      }
    }
    
  }

  #region Prifile

  public void SetCoinText()
  {        
     coin.text = UserProfile.instance.TurnNumberToDecimalpointSeperator( UserProfile.instance.GetCoin());    
  }

  public void SetuserNameText()
  {
    UserName.text = UserProfile.instance.getUserName();
  }

  public void setuserImage(string url)
  {
    
  }

  #endregion

  #region PlayConfirm

  public void ShowPlayConfirm()
  {
    PlayConfirm.SetActive(true);
    ServerGameReq.instance.User.text = UserProfile.instance.getUserName();
    ServerGameReq.instance.userLevel.text = "Level:"+(UserProfile.instance.GetLevel()/150).ToString();
  }

  public void HidePlayConfirm()
  {
    PlayConfirm.SetActive(false);
  }

  
 
  public void StartGame()
  {
    SceneManager.LoadScene(1);
  }

    #endregion

    float timer;
    private void Update()
    {
        if (!Connected) return;
        if (timer == 0)
        {
            timer = Time.time;
        }
        if (Time.time - timer > 3)
        {
            CheckConnection();
        }

    }

    void CheckConnection()
    {
       // print("check connection");
        if (ServerConnector.instance == null)
        {
            return;
        }

        if (ServerConnector.instance.ws == null)
        {
            StartCoroutine(Co_ConnectionError("server null"));
            return;
        }



        if (ServerConnector.instance.ws.State != NativeWebSocket.WebSocketState.Open)
        {
            StartCoroutine(Co_ConnectionError("server unreachable"));
            return;
        }

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            StartCoroutine(Co_ConnectionError("No internet"));
            return;
        }
    }
    public TextMeshProUGUI SystemError;
    public  bool Connected;

    IEnumerator Co_ConnectionError(string error)
    {
        SystemError.text = error;
        SystemError.gameObject.SetActive(true);
        ServerConnector.instance.ws = null;
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);
    }

   


}
