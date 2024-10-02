using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : Singleton<GameUI>
{
    public TextMeshProUGUI RoomPrice, userName, OppName, Userlevel, OppLevel;
    public GameObject playerIcon, OppIcon;
    public Animator Sticker,OppSticker;
    public GameObject StickerFrame;
    public GameObject ResignWindow;
    public GameObject CoinImage, PowrUpImage,CROWN,oppCoinImage,oppPowerup,oppCrown,finalBoard,finalPanel;
    public TextMeshProUGUI oppExtracoin,oppExtraPower, ExtraCoin, ExtraPower,playerName,oppnamefinal,Board;
    public GameObject ErrorUI,OppOflineError;

    

    public void ShowOppIsOffline()
    {
        
        OppOflineError.SetActive(true);
       //  GameTouchManager.instance.IgnorTouch=true;
    }
    public GameObject DoneBtn, UndoBtn;
    public void hideUi()
    {
        DoneBtn.SetActive(false);
        UndoBtn.SetActive(false);   
    }
    public void HideOppIsOffline()
    {
        OppOflineError.SetActive(false);
        if (GameStateManager.instance.gameState != GameStateManager.GameState.GameFinished)
        {
            if (GameTouchManager.instance.PlayerMoved)
            {
                SceneManager.LoadScene(0);
            }
            

        }
        GameTouchManager.instance.IgnorTouch = false;   
    }

    private void Start()
    { 
        if(ServerConnector.instance.OppOflineWhileReconnect)
        {
            ShowOppIsOffline();
        }
        SetGameUi(ServerGameReq.instance.room.ToString(),UserProfile.instance.getUserName(),
            (UserProfile.instance.GetLevel()).ToString(),UserProfile.instance.GetOppName(),UserProfile.instance.getOppLevel());
    }

    public void OpenResign()
    {
        ResignWindow.SetActive(true);
        ResignWindow.GetComponent<CanvasGroup>().alpha = 0;
        LeanTween.alphaCanvas(ResignWindow.GetComponent<CanvasGroup>(), 1, 0.5f);
        GameTouchManager.instance.IgnorTouch = true;
    }

    public void CloseResign()
    {
        StartCoroutine(StartTouch());
        LeanTween.alphaCanvas(ResignWindow.GetComponent<CanvasGroup>(), 0, 0.5f).setOnComplete(() =>
        {
            ResignWindow.SetActive(false);
        });
    }

    public void Resign()
    {
        ServerConnector.instance.Resign();
    }

    IEnumerator StartTouch()
    {
        yield return new WaitForSeconds(0.1f);
        GameTouchManager.instance.IgnorTouch = false;
    }

    public void SetGameUi(string roomprice, string username, string userlevel, string oppusername, string opplevel)
    {
        playerIcon.SetActive(true);
        OppIcon.SetActive(true);
        
        RoomPrice.transform.parent.gameObject.SetActive(true);
        userName.transform.parent.gameObject.SetActive(true);
        Userlevel.transform.parent.gameObject.SetActive(true);
        OppName.transform.parent.gameObject.SetActive(true);
        OppLevel.transform.parent.gameObject.SetActive(true);
        
        Int64.TryParse(UserProfile.instance.getOppLevel(), out long oppL);
        Int64.TryParse(userlevel, out long userLevel);
        if(ServerConnector.instance.Resume)
        {
            roomprice = ServerConnector.instance.room.ToString();
        }
        RoomPrice.text = UserProfile.instance.TurnNumberToDecimalpointSeperator( roomprice);
        userName.text = username;
        Userlevel.text =UserProfile.instance.TurnNumberToDecimalpointSeperator((userLevel/150).ToString());
        OppName.text = oppusername;
        OppLevel.text = UserProfile.instance.TurnNumberToDecimalpointSeperator((oppL/150).ToString());

        if(ServerConnector.instance.SelfeRemaining!=0)
        {
            playerIcon.GetComponent<UserIcon>().StartExtraCounter(ServerConnector.instance.SelfeRemaining); 
        }

        if(ServerConnector.instance.OppRemaining!=0)
        {
            OppIcon.GetComponent<UserIcon>().StartExtraCounter(ServerConnector.instance.OppRemaining);
        }

    }

    public void SendSmile(string face)
    {
        
       ServerConnector.instance.SendFace(face);
        
    }

    public void ServerRecieveSticker(string sticker,string username)
    {
        if(sticker== "st-smile")
        {
            SFX.instance.PlaySmile();
        }else if(sticker== "st-cry")
        {
            SFX.instance.PlayCry();
        }
        else if(sticker== "st-winkle")
        {
            SFX.instance.PlaySmile();
        }
        else if(sticker == "st-surprise")
        {
            SFX.instance.PlaySurprise();
        }
        else if(sticker== "st-laugh")
        {
            SFX.instance.PlayLaugh();
        }
        if(username==UserProfile.instance.getUserName())
        {
            Sticker.gameObject.SetActive(true);
            Sticker.SetTrigger(sticker);
            StartCoroutine(StopAnimation(Sticker));
        }
        else
        {
            OppSticker.gameObject.SetActive(true);
            OppSticker.SetTrigger(sticker);
            StartCoroutine(StopAnimation(OppSticker));
        }
        
        

        
    }
    
    IEnumerator StopAnimation(Animator animator)
    {
        yield return new WaitForSeconds(2);
        animator.SetTrigger("end");
        animator.gameObject.SetActive(false);
    }

    public void OpenStickerFrame()
    {
        StickerFrame.SetActive(true);
    }


    [ContextMenu("final result")]
    public void TestFinalImgae()
    {
        StartCoroutine(co_coindPowerAnimation(10, 150));
    }
    public GameObject Coin,playerIconEnd,OppIconEnd,Canvas;
    public void FinalResult(bool won,int point)
    {
        finalPanel.gameObject.SetActive(true);
        finalBoard.SetActive(true);
        playerName.text =UserProfile.instance.getUserName();
        var oppname= UserProfile.instance.GetOppName();
        print("zzx"+oppname);
        oppnamefinal.text = oppname;
        if(won)
        {
            StartCoroutine(FinalCoin(OppIconEnd.transform, playerIcon.transform));
            StartCoroutine(co_coindPowerAnimation(point, 150));
            Board.text = " YOU WON !";
        }
        else
        {
            StartCoroutine(FinalCoin(playerIconEnd.transform, OppIcon.transform));
            Board.text = " YOU LOST !";
            oppCrown.SetActive(true);
        }
    }


    public void ContinueAftarGameFinished()
    {
        SceneManager.LoadScene(0);
    }

    IEnumerator co_coindPowerAnimation(int coin,int powerUp)
    {
        float startTime=Time.time;
        CROWN.GetComponent<CanvasGroup>().alpha = 0;
        CROWN.SetActive(true);
        LeanTween.alphaCanvas(CROWN.GetComponent<CanvasGroup>(), 1, 1f);
        CoinImage.GetComponent<CanvasGroup>().alpha = 0;
        CoinImage.SetActive(true);
        bool coinplayed = false;
        while(Time.time-startTime < 2)
        {
            yield return null;
            if(!coinplayed)
            {
                LeanTween.alphaCanvas(CoinImage.GetComponent<CanvasGroup>(), 1, 0.4f).setOnComplete(() =>
                {
                    ExtraCoin.gameObject.SetActive(true);
                    ExtraCoin.text = coin.ToString() + "+";
                    SFX.instance.Playcoin();

                });
                coinplayed=true;
            }
           
        }
        CoinImage.SetActive(false);
        bool powerplayed=false;
        startTime=Time.time;
        PowrUpImage.SetActive(true);
        PowrUpImage.GetComponent<CanvasGroup>().alpha = 0;  
        while (Time.time - startTime < 2)
        {
            yield return null;
            if (!powerplayed)
            {
                LeanTween.alphaCanvas(PowrUpImage.GetComponent<CanvasGroup>(), 1, 0.4f).setOnComplete(() =>
                {
                    ExtraPower.gameObject.SetActive(true);
                    ExtraPower.text = powerUp.ToString() + "+";
                    SFX.instance.playPower();
                });
                powerplayed = true;
            }

        }
        PowrUpImage.SetActive(false);



    }

    [ContextMenu("CoinAnim")]
   
    public void Tester()
    {
        StartCoroutine(FinalCoin(OppIconEnd.transform, playerIcon.transform));
    }

   
    IEnumerator FinalCoin(Transform Origin,Transform Target)
    {
        yield return new WaitForSeconds(3);
        for(int i=0;i<10;i++)
        {
            var temp = Instantiate(Coin, Canvas.transform);
            temp.transform.SetParent(Canvas.transform, false);
            temp.transform.position=Origin.position;
            LeanTween.move(temp.gameObject, temp.transform.position + new Vector3(0, 75, 0), 0.5f).setEaseInCirc().setOnComplete(() =>
               {
                   LeanTween.move(temp,Target.position, 0.8f).setEaseInCirc().setOnComplete(() =>
                   {
                       temp.gameObject.SetActive(false);
                   });
               });
            yield return new WaitForSeconds(0.21f);

        }
    }

    float timer;
    private void Update()
    {
        if(timer==0)
        {
            timer = Time.time;  
        }
        if(Time.time-timer>3)
        {
            CheckConnection();
        }

    }

    void CheckConnection()
    {
       // print("check connection");
        if(ServerConnector.instance==null)
        {
            return;
        }

        if(ServerConnector.instance.ws==null)
        {
          StartCoroutine(Co_ConnectionError());
            return;
        }



        if(ServerConnector.instance.ws.State!=NativeWebSocket.WebSocketState.Open)
        {
            StartCoroutine(Co_ConnectionError());
            return;
        }

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            StartCoroutine(Co_ConnectionError());
            return;
        }
    }

    IEnumerator Co_ConnectionError()
    {
        ErrorUI.SetActive(true);
        ServerConnector.instance.ws = null; 
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);
    }

    public void CloseStickerFrame()
    {
        StickerFrame.SetActive(false);
        
    }
    
    
    
}

