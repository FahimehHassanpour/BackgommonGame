using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using NativeWebSocket;

using Newtonsoft;
using Newtonsoft.Json;
using TMPro;
using static Login;
using UnityEngine.Networking;
using System.Text;

public class ServerConnector : Singleton<ServerConnector>
{
    public string ServerAddress, localAddres;
    public WebSocket ws;
    public bool host;
    [SerializeField] private int PORT;
    public bool LocalHost;
    public int[,] ServerBoard;
    public int numberOfhitted, numberOfStage;
    public int OppnumberOfhitted, OppnumberOfStage;
    public string userArray;
    public double ServerLastTimer;
    public string ServerPhase;
    public bool friendreqSent;
    public List<int> rooms;
    public int tester;
    public class GameId
    {
        //p
        public string id;
    }

    public bool TestName;
    public class GameState
    {
        public string tag = "";
        public int[] brd;
        public int hitted;
        public int stage;
        
    }


    [ContextMenu("sebd binary")]
    public byte[] binary(string message)
    {        
        // Convert a C# string to a byte array
        byte[] bytes = Encoding.ASCII.GetBytes(message);
        ws.Send(bytes);
        return bytes;
    }



    [ContextMenu("Get rooms")]
    public void GetRoomsFromServer(bool frindRooms=false)
    {
        StartCoroutine(Co_GetRoomsFromCryptoServer(frindRooms));
    }

    public GameObject RoomPrefab,FriendRoomPrefab;
    public Transform RoomPanel,FriendlyRoomPanel;
    IEnumerator Co_GetRoomsFromCryptoServer(bool friendlyRooms)
    {
        if(friendlyRooms)
        {
            var childs=homePage.instance.FriendRoomPanel.GetComponentsInChildren<RoomSuggest>();
            for(int i=0;i<childs.Length;i++)
            {
                Destroy(childs[i].gameObject);
            }
        }
        print("fetching rooms");
        rooms.Clear();
        UnityWebRequest uwr = UnityWebRequest.Get("https://game.iwco.io/api/matches/rooms");
        uwr.SetRequestHeader("Content-Type", "application/json; charset=utf-8");

        uwr.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("token"));
        uwr.certificateHandler = new BypassCertificate();
        yield return uwr.SendWebRequest();
        if (uwr.isNetworkError)
        {
            Debug.Log("//Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("ReceivedFetchedRoom: " + uwr.downloadHandler.text);
            var rooms=JsonConvert.DeserializeObject<List<Room>>(uwr.downloadHandler.text);
            foreach (Room room in rooms)
            {
                print("fetch" + room.point);
                if(friendlyRooms)
                {
                    print(room.room_id+"friendlyRoom"+room.point);
                    GameObject temp = Instantiate(FriendRoomPrefab, homePage.instance.FriendRoomPanel.transform);
                    temp.transform.parent = homePage.instance.FriendRoomPanel.transform;
                    temp.transform.localScale = Vector3.one;
                    temp.GetComponent<RoomSuggest>().SetupRoom(room.room_id,room.point);
                }
                else
                {
                    
                    GameObject temp = Instantiate(RoomPrefab, homePage.instance.RoomPanel.transform); 
                    temp.transform.parent = homePage.instance.RoomPanel.transform;
                    temp.transform.localScale = Vector3.one;
                    temp.GetComponent<GameRoom>().SetUpRoom(room.point, room.room_id);
                    temp.GetComponent<CanvasGroup>().alpha = 1;
                }
                
            }
        }

    }

  

    public class Room
    {
        public int room_id = 0;
        public int point = 0;
    }

    public int[] TurnBoardToSingleArray()
    {
        int[] SingleArray = new int[360];
        int count = 0;
        for (int i = 0; i < 24; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                SingleArray[count] = BackGamonBoard.instance.Board[i, j];
                count++;
            } 
        }

        return SingleArray;
    }

   


    public int[,] ConverArrayToMultiArray(int[] array,bool reverse=false)
    {
        if (array.Length == 0)
        {
            return null;
        }
        int[,] res = new int[24,15];
        int i = 0;
        int j = 0;
        foreach (var VARIABLE in array)
        {
            var temp = VARIABLE;
           
            if (!string.IsNullOrEmpty(userArray)  &&  userArray != UserProfile.instance.getUserName())
            {
                print("reversing");
                if (temp == 1)
                {
                    temp = 2;
                }else if (temp == 2)
                {
                    temp = 1;
                }
                print(temp);
            }
            
            res[i,j] = temp;
            j++;
            {
                if (j == 15)
                {
                    i++;
                    j = 0;
                }
                
            }
        }
        foreach (var item in res)
        {
            //p
            print(item);
        }
        return res;


    }

    public void SendFriendRequest()
    {
        MessageTag message = new MessageTag();
        message.tag = "reqFriendship";
        SendWebSocketMessage(JsonConvert.SerializeObject(message));
    }

    public void AcceptFriedRequest()
    {
        MessageTag objt = new MessageTag();
        objt.tag = "AccpetFriendRequest";
        SendWebSocketMessage(JsonConvert.SerializeObject(objt));
    }
   
    private Dictionary<MessageTag, String> messageDictioanry = new Dictionary<MessageTag, string>();

    public class MessageTag
    {
        public string tag;
        public string username;
    }

    public class AskForFriends
    {
        public string tag;
        public string username;
    }

    public void Resign()
    {
        MessageTag ob = new MessageTag();
        ob.tag = "resign";
        SendWebSocketMessage(JsonConvert.SerializeObject(ob));
    }

    public class NewGame
    {
        public bool host;
        public string oppname;
        public string opplevel;
        public int price;

    }

    public void SyncUserData(UserData userdata)
    {
        userdata.tag = "syncprofile";
        print("syncprofile");

        SendWebSocketMessage(JsonConvert.SerializeObject(userdata));
    }
    
    

    public  void ServerStart()
    {
        ConnectToWS();
        StartCoroutine(MeessageMOnitor());
        DontDestroyOnLoad(gameObject);
    }
    

    public void CheckForUserCount()
    {
        GameCheck gc=new GameCheck();
        gc.username = UserProfile.instance.getUserName();
        gc.tag = "checkUser";
        print($"checking{gc.username}");
        SendWebSocketMessage(JsonConvert.SerializeObject(gc));
    }

    
    public class GameCheck
    {
        public string tag;
        public string id;
        public string username;
        public int count;
    }
    [ContextMenu("connect")]
    public async void ConnectToWS()
    {
       // print("connecting to server");

        if (ws != null)
        {
            await ws.Close();
        }


        if (LocalHost)
        {
            ws = new WebSocket($"wss://{localAddres}:{PORT}");
        }
        else
        {
            ws = new WebSocket("ws://164.92.98.131:80/");
        }


        ws.OnOpen += () =>
        {
            print("connected");
            homePage.instance.Connected=true;
            MessageTag tag = new MessageTag();
            tag.tag = "conOpen";
            var message = "connection stablished";

            messageDictioanry.Add(tag, message);

            queue.Enqueue(tag);
            CheckForUserCount();

        };





        ws.OnError += (e) =>
        {
          //  print("connection error");
            ConnectToWS();
        };



        ws.OnMessage += (bt) =>
        {
            tester += 1;
            print("SOLVED-------------------");
            var message = System.Text.Encoding.UTF8.GetString(bt);
            MessageTag tag = JsonUtility.FromJson<MessageTag>(message);
            print(tag.tag);
            messageDictioanry.Add(tag, message);
            queue.Enqueue(tag);
        };


        ws.OnClose += (t) =>
        {
            print("close-------------------------");
        };

        await ws.Connect();
    }

    public void UpdateBoardOnReconnect_Req()
    {
        MessageTag tag = new MessageTag();
        tag.tag = "Asksync";
        SendWebSocketMessage(JsonConvert.SerializeObject(tag));
    }
    //ll
    
    public void UpdateBoardOnReconnect_Response()
    {
        print("jojo");
        print(GameStateManager.instance.gameState);
        if(GameStateManager.instance.gameState!=GameStateManager.GameState.GameFinished)
        {
            
                GameUI.instance.ShowOppIsOffline();
            
            
        }
       //ll
    }


    public void AskforUserStats()
    {
        print("asking for server stat");
        newUser user = new newUser();
        user.tag = "askstat";
        user.username = UserProfile.instance.getUserName();
        SendWebSocketMessage(JsonConvert.SerializeObject(user));
    }

    
    
    public void SendFace(string face)
    {
        MessageTag tag = new MessageTag();
        tag.tag = face;
        tag.username = UserProfile.instance.getUserName();
        SendWebSocketMessage(JsonConvert.SerializeObject(tag));
    }
    
    public void SendToServerTurnFinish()
    {
        GameState state = new GameState();
        state.tag = "turnFinish";
        state.brd = TurnBoardToSingleArray();
        state.hitted = CouterHitting.instance.PlayerBHittedCouters.Count;
        
        print("number of hitted couters"+state.hitted);
        
        state.stage = BackGamonBoard.instance.PlayerAFinalCouter.Count;
        
        print("number of staged couters"+state.stage);
        
        ws.SendText(JsonUtility.ToJson(state));        
        
    }
    
    public void SendToServerBringCouterBackTogame()
    {
        MessageTag msg = new MessageTag();
        msg.tag = "bringBackToGame";
        SendWebSocketMessage(JsonUtility.ToJson(msg));
    }

    public void SendToServerBringCouterBackToSelected(int selectedColumn)
    {
        MoveObj move = new MoveObj();
        move.tag = "bringBackToGameSel";
        move.targetI =23- selectedColumn;
        SendWebSocketMessage(JsonUtility.ToJson(move)); 
    }
    Queue<MessageTag> queue = new Queue<MessageTag>();
    public bool Resume,PlayersTurn;
    public int DiceA, DiceB;
    public int room;
    void UseMessage(MessageTag messageTag,string message)
    {
       print("incoming message");
        print(message);
        print(messageTag.tag);
        if (messageTag.tag == "newgame"){

            homePage.instance.HomePageOff();
            homePage.instance.LobbyEffect.SetActive(true);  
            NewGame newGame = JsonUtility.FromJson<NewGame>(message);
            Int64.TryParse(newGame.opplevel, out long nn);
            ServerGameReq.instance.opponentLevel.text ="Level:"+(nn/150).ToString();
            ServerGameReq.instance.Opponent.text = newGame.oppname;
            ServerGameReq.instance.room = newGame.price;
            UserProfile.instance.SetOppUserName(newGame.oppname);
            UserProfile.instance.SetOppLevel(newGame.opplevel);
            host = newGame.host;
        }
        else if (messageTag.tag == "oppreject")
        {
            ServerGameReq.instance.opponentLevel.text = "";
            ServerGameReq.instance.Opponent.text = "";
            ServerGameReq.instance.RequestForGame();
        }
        else if (messageTag.tag == "backtohall")
        {
            ServerGameReq.instance.opponentLevel.text = "";
            ServerGameReq.instance.Opponent.text = "";
            homePage.instance.HidePlayConfirm();
            //back to game
        }
        else if (messageTag.tag == "enterLobby")
        {
            //enter lobby
            //print(message);
            homePage.instance.ShowPlayConfirm();
        }
        else if (messageTag.tag == "conOpen")
        {
            print("hey you");
            Login.instance.SocketNotConnected = false;
            return;
            
            
        }
        else if (messageTag.tag == "noLastGame")
        {
            Resume = false;
            Login.instance.ActivateLogin();
            PlayerPrefs.SetString("gameid","");
            
        }
        else if (messageTag.tag == "LastGame")
        {
            Resume = true;
            GameState state = JsonUtility.FromJson<GameState>(message);
            instance.ServerBoard = ConverArrayToMultiArray(state.brd) ;
            instance.numberOfhitted = state.hitted;

        }
        else if (messageTag.tag == "gameStart")
        {
            GameId id = JsonUtility.FromJson<GameId>(message);
            PlayerPrefs.SetString("gameid",id.id);
            GameState state = new GameState();
            state = JsonUtility.FromJson<GameState>(message);
            print("sb"+state.brd.Length);
            ServerBoard = ConverArrayToMultiArray(state.brd);
            //print("sb"+ServerBoard.Length);
            homePage.instance.HidePlayConfirm();
            SceneManager.LoadScene(1);
        }
        else if (messageTag.tag == "dice")
        {
            _Dice dice = JsonUtility.FromJson<_Dice>(message);
            print($"dzz{dice.a}--{dice.b}");
            Dice.instance.DiceResultFromServer(dice.a,dice.b);
        }
        else if (messageTag.tag == "moveToSel")
        {
            MoveObj move = JsonUtility.FromJson<MoveObj>(message);
            print($"recieving {move.step} in opp {move.OriginI}-{move.OriginJ}-{move.targetI}-{move.targetJ}");
            MoveConfirm.instance.recieveMoveToSelectedFromServer(move.OriginI,move.OriginJ,move.targetI,move.targetJ,move.step);
            GameTouchManager.instance.PlayerMoved=true;
        }
        else if (messageTag.tag == "undo")
        {
            MoveConfirm.instance.Undo();
            GameTouchManager.instance.PlayerMoved = true;
        }
        else if (messageTag.tag == "move")
        {
            print("mvzv1");
            GameTouchManager.instance.PlayerMoved = true;
            MoveObj move = JsonUtility.FromJson<MoveObj>(message);
            MoveConfirm.instance.recieveMoveFromServer(move.OriginI);
            
        }
        else if (messageTag.tag == "bringBackToGame")
        {
            GameTouchManager.instance.PlayerMoved = true;
            BackGamonBoard.instance.BringCouterBackTogame();
        }
        else if (messageTag.tag == "turnFinish")
        {
            print("stopCorotine");
            GameUI.instance.OppIcon.GetComponent<UserIcon>().StopExtraCoroutine();
            GameUI.instance.playerIcon.GetComponent<UserIcon>().StopExtraCoroutine();
            GameStateManager.instance.MoveFinishConfirm(true);
            GameTouchManager.instance.PlayerMoved = false;
        }
        else if (messageTag.tag == "bringBackToGameSel")
        {
            Transform couter = null;
            MoveObj move = JsonUtility.FromJson<MoveObj>(message);
            GameTouchManager.instance.PlayerMoved = true;

            if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
            {
                couter=CouterHitting.instance.PlayerAHittedCouters.Last();
            }
        
            if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerBMoveTurn)
            {
                couter=CouterHitting.instance.PlayerBHittedCouters.Last(); 
            }
           
            print($"Message from server :player A Hitted count:{CouterHitting.instance.PlayerAHittedCouters.Count} " +
                  $"and player B Hitted count:{CouterHitting.instance.PlayerBHittedCouters.Count} and state is {GameStateManager.instance.gameState} column {move.targetI} " +
                  $"and couter is {couter.GetInstanceID()}");
            
            BackGamonBoard.instance.BringBackCouterTOSelectedColumn(move.targetI,couter.GetComponent<CounterState>());
        }
        else if (messageTag.tag == "gameFinished")
        {
            Timeout t = JsonUtility.FromJson<Timeout>(message);
            print(t.won);   
            GameStateManager.instance.TimeOut(t.won,t.point);
           
        }
        else if (messageTag.tag == "ServerBoard")
        {
            //dd
            Resume = true;
            BoardUpDate board = new BoardUpDate();
            board.userAdetails = new userAdetails();
            board.UserBdetails = new userBdetails();
            
            //board = JsonUtility.FromJson<BoardUpDate>(message);
            board=JsonConvert.DeserializeObject<BoardUpDate>(message);
            print("room after reconnect" + board.room);
            room=board.room;
            OppOflineWhileReconnect = board.Opp;
            print("stage counts");
            print(board.userAdetails.Stage);
            print(board.UserBdetails.Stage);
            //getting array
            
            //getting turn
            var u = UserProfile.instance.getUserName();
            ServerPhase = board.phase;
            print($"{board.userarray} created the board and this user is { UserProfile.instance.getUserName()} " +
                  $"and server phase is {ServerPhase}");
            userArray = board.userarray;
            ServerBoard = ConverArrayToMultiArray(board.board);
            
            

           
            if (!string.IsNullOrEmpty(userArray) && userArray != UserProfile.instance.getUserName())
            {
                int[,] reversed = new int[24, 15];
                for (int i = 0; i < 24; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        reversed[23 - i, j] = ServerBoard[i, j];
                    }
                }
                ServerBoard = reversed;
            }

            var us = UserProfile.instance.getUserName();
            //re
            if (board.userturn == UserProfile.instance.getUserName())
            {
                PlayersTurn = true;
            }
            else
            {
                PlayersTurn = false;
            }
            
            var oppUserName = board.userA == UserProfile.instance.getUserName() ? board.userB : board.userA;
            
            UserProfile.instance.SetOppUserName(oppUserName);

            
            
            DiceA = board.diceA;
            DiceB = board.diceB;

            print(DiceA);
            print(DiceB);

            ServerLastTimer = board.lasttimer;
            print("LastTimer"+ServerLastTimer);
            if (board.userAdetails.username == UserProfile.instance.getUserName())
            {
                numberOfhitted = board.userAdetails.Hitted;
                numberOfStage = board.userAdetails.Stage;
                
                OppnumberOfhitted = board.UserBdetails.Hitted;
               OppnumberOfStage = board.UserBdetails.Stage;

            }
            else
            {
                OppnumberOfhitted = board.userAdetails.Hitted;
                OppnumberOfStage = board.userAdetails.Stage;

                numberOfhitted = board.UserBdetails.Hitted;
                numberOfStage = board.UserBdetails.Stage;
            }
            
            print("number of hitted"+numberOfhitted);
            
            
            
            SceneManager.LoadScene(1);

        }
        else if (messageTag.tag == "Turn-start")
        {
            GameStateManager.instance.StartTurn();
        }
        else if (messageTag.tag == "winstagedrag")
        {
            GameTouchManager.instance.PlayerMoved = true;
            MoveObj obj = JsonConvert.DeserializeObject<MoveObj>(message);
            BackGamonBoard.instance.ServerOrderCouterToWinStage(obj.targetI,obj.targetJ);
        }
        else if (messageTag.tag == "friendreqsentsuccess")
        {
            if (ServerBridge.instance != null)
            {
                ServerBridge.instance.FriendRequestSent();
            }
           
        }
        else if (messageTag.tag == "newFriendReq")
        {
            if (ServerBridge.instance != null)
            {
                ServerBridge.instance.IncomingFriendRequest();
            }

            if (FriendMenuManager.instance != null)
            {
                FriendMenuManager.instance.FriendReqAlaram.SetActive(true);
            }
            
            
        }
        else if (messageTag.tag=="friend_req_not_sent")
        {
            ServerBridge.instance.EnableFriendRequest();
        }
        else if (messageTag.tag == "acceptsuccess")
        {
            ServerBridge.instance.acceptFriendRequestSuccess();
        }
        else if (messageTag.tag == "friendslist")
        {
            print(message);
            friendslist friends = new friendslist();
            friends= JsonConvert.DeserializeObject<friendslist>(message);
            
            print(friends);
            print(friends.friends[0]);
            print(friends.friends[1]);
            FriendMenuManager.instance.AddFriendToList(friends.friends[1],friends.friends[0]);
        }
        else if (messageTag.tag == "friendsReqs")
        {
            print("friendsReqs");
            friendslist friends = new friendslist();
            friends= JsonConvert.DeserializeObject<friendslist>(message);
            
            print(friends);
            print(friends.friends[0]);
            print(friends.friends[1]);
            FriendMenuManager.instance.AddFriendRequests(friends.friends[1],friends.friends[0]);
        }
        else if (messageTag.tag == "usedemail")
        {
            Login.instance.EmailExists();
        }
        else if (messageTag.tag == "usercreated")
        {
            newUser user = new newUser();
            user = JsonConvert.DeserializeObject<newUser>(message);
            UserProfile.instance.SetUserName(user.username);
            UserProfile.instance.setCoin(user.coin);
            UserProfile.instance.setLevel(user.level);
            UserProfile.instance.setEmail(user.email);
            PlayerPrefs.SetString("log","signed_up");
            Login.instance.LoginSuccess();
            print("signup success");
            print(user.username+""+user.coin+""+user.level);
        }
        else if (messageTag.tag == "setstat")
        {

            Login.instance.SocketNotConnected = false;
            newUser obj = JsonConvert.DeserializeObject<newUser>(message);
            UserProfile.instance.setCoin(obj.coin);
            UserProfile.instance.setLevel(obj.level);
            if (homePage.instance != null)
            {
                homePage.instance.SetUserUi();
            }

            if (Login.instance != null)
            {
                Login.instance.HomePage.SetActive(true);
            }
            
            print("new  coin"+obj.coin);
            print("new level"+obj.level);
        }
        else if (messageTag.tag == "st-smile"  || messageTag.tag == "st-cry"|| messageTag.tag == "st-winkle" || messageTag.tag == "st-surprise" || messageTag.tag == "st-laugh")
        {
            print("smile"+messageTag.tag);
            GameUI.instance.ServerRecieveSticker(messageTag.tag,messageTag.username);
        }
        else if (messageTag.tag == "friendoffline")
        {
            homePage.instance.messageboard.ShowMessage("Friend is offline");
        }
        else if (messageTag.tag == "friendonline")
        {
            FriendMenuManager.instance.OpenFriendLobby();
        }
        else if (messageTag.tag == "friendlysug")
        {
            FriendMenuManager.FriendSuggestion fr =
                JsonConvert.DeserializeObject<FriendMenuManager.FriendSuggestion>(message);
            FriendMenuManager.instance.IncomingGameRequest(fr);
        }
        else if (messageTag.tag == "fr-reject")
        {
            FriendMenuManager.instance.CloseFriendLobby();
            homePage.instance.messageboard.ShowMessage("Friend rejected");
        }
        else if (messageTag.tag == "invfail")
        {
            FriendMenuManager.instance.hideIncomingGameDetails();
            homePage.instance.messageboard.ShowMessage("Friend Left");
        }
        else if (messageTag.tag == "friendnotfound")
        {
            FriendMenuManager.instance.SearchResultOpen("",null);
            
        }
        else if (messageTag.tag == "friendfound")
        {
            FriendMenuManager.FriendGameList obj = JsonConvert.DeserializeObject<FriendMenuManager.FriendGameList>(message);
            
            FriendMenuManager.instance.SearchResultOpen(obj.friendName,obj);
        }
        else if (messageTag.tag == "friendAlreadyReqSent")
        {
            
            FriendMenuManager.FriendGameList obj = JsonConvert.DeserializeObject<FriendMenuManager.FriendGameList>(message);
            FriendMenuManager.instance.SearchResultOpen("not found",obj);
        }
        else if(messageTag.tag == "updated")
        {
            //continue
            Login.instance.IWCONotConnected = false;
            GameCheck newCheck = new GameCheck();
            newCheck.tag = "checkForLastGame";
            newCheck.id = PlayerPrefs.GetString("gameid");
            newCheck.username = UserProfile.instance.getUserName();
            print(newCheck.username + "checking for last game" + newCheck.id);
            print(newCheck.username);
            if (newCheck.username != null && newCheck.username != "")
            {
                ServerConnector.instance.AskForListOfFriends();
                instance.AskForFriendShipRequests();
                print("4");
                AskforUserStats();
            }
            else
            {
                Login.instance.SocketNotConnected = false;
            }


            if (newCheck.id != "")
            {
                SendWebSocketMessage(JsonUtility.ToJson(newCheck));
            }
            else
            {
                Login.instance.ActivateLogin();
            }
            Login.instance.LoginSuccess();  
        }
        else if(messageTag.tag== "extratime")
        {
            ExtraTime ext = JsonConvert.DeserializeObject<ExtraTime>(message);
            if (ext != null)
            {
                print($"extra is{ext.extra}");
                if(GameUI.instance != null)
                {
                    GameUI.instance.OppIcon.GetComponent<UserIcon>().StartExtraCounter(ext.extra);
                }
                else
                {
                    OppRemaining=ext.extra;
                }
                
            }
        }
        else if (messageTag.tag == "extratimeselfe")
        {
            ExtraTime ext = JsonConvert.DeserializeObject<ExtraTime>(message);
            if (ext != null)
            {
                print($"extra is{ext.extra}");
                if(GameUI.instance != null)
                {
                    GameUI.instance.playerIcon.GetComponent<UserIcon>().StartExtraCounter(ext.extra);
                }
                else
                {
                    SelfeRemaining=ext.extra;   
                }
               
            }
        }else if(messageTag.tag == "userCount")
        {
            GameCheck chk=JsonConvert.DeserializeObject<GameCheck>(message);
            if(chk.count>0)
            {
                Login.instance.LoginFail.SetActive(true);
            }

        }else if(messageTag.tag== "SyncReq")
        {
            UpdateBoardOnReconnect_Response(); 
        }else if(messageTag.tag== "oppOfline")
        {
            OppOflineWhileReconnect = true;
        }
        else if (messageTag.tag == "EnteringRoom")
        {
            OppOflineWhileReconnect = false;
            if(GameUI.instance!=null)
            {
                GameUI.instance.HideOppIsOffline();
            }
        }else if(messageTag.tag== "askingIfOnline")
        {
            if(BackGamonBoard.instance.BoarDictionary.Count>0)
            {
                StartCoroutine(Co_Declare());
            }
        }else if(messageTag.tag== "finishConfirm")
        {
            GameTouchManager.instance.PlayerMoved=false;
        }


        messageDictioanry.Remove(messageTag);
    }

   /* void TellServerYouAreOnline()
    {
        if(ws!=null)
        {
            MessageTag msg=new MessageTag();
            msg.tag = "ready";
            ws.SendText(JsonUtility.ToJson(msg));
        }
    }*/

    public void  declareEnteringGame()
    {
        StartCoroutine(Co_Declare());
        RemainingMoves.instance.CalculateRamainingMoves();
        
    }

    IEnumerator Co_Declare()
    {
        yield return new WaitForSeconds(2);
        MessageTag msg = new MessageTag();
        msg.tag = "EnterGame";
        ws.SendText(JsonUtility.ToJson(msg));
    }


    public bool OppOflineWhileReconnect;

    public int SelfeRemaining, OppRemaining;
    public class ExtraTime
    {
        public int extra;
    }

    public class  newUser
{
    public string tag;
    public string username;
    public int coin;
    public int level;
    public string email;
}    
    public class friendslist
{
    public string[] friends;
}

    public class Signup
{
    public string tag;
    public string email;
    public string password;
}
    public void AskForListOfFriends()
    {
        AskForFriends mes = new AskForFriends();
        mes.tag = "askForFriendsList";
        mes.username = UserProfile.instance.getUserName();
        print(mes.username+"is asking for friends list");
        SendWebSocketMessage(JsonConvert.SerializeObject(mes));
    }
    
    public void AskForFriendShipRequests()
    {
        AskForFriends mes = new AskForFriends();
        mes.tag = "askForReqList";
        mes.username = UserProfile.instance.getUserName();
        print(mes.username+"is asking for friends request list");
        SendWebSocketMessage(JsonConvert.SerializeObject(mes));
    }

    public void TrySignUp(string email, string password)
    {
        Signup sn = new Signup();
        sn.email = email;
        sn.password = password;
        sn.tag = "signup";
        SendWebSocketMessage(JsonConvert.SerializeObject(sn));
    }
    
    public void SendWinstageToServer()
    {
        MoveObj obj = new MoveObj();
        
        obj.tag = "checkwin";
        obj.step = BackGamonBoard.instance.PlayerAFinalCouter.Count;
        SendWebSocketMessage(JsonConvert.SerializeObject(obj));
    }
    
    
    public void SendWinstageMessageToServer(int i, int j)
    {
        MoveObj obj = new MoveObj();
        obj.tag = "winstage";
        obj.targetI = i;
        obj.targetJ = j;
        SendWebSocketMessage(JsonConvert.SerializeObject(obj));

    }
    public double getTimestamp()
    {
        DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime utcNow = DateTime.UtcNow;
        TimeSpan elapsedTime = utcNow - unixEpoch;
        double millis = elapsedTime.TotalMilliseconds;
        return millis;
    }

    public class userAdetails
    {
        public string username;
        public int Hitted;
        public int Stage;
    }
    
    public class userBdetails
    {
        public string username;
        public int Hitted;
        public int Stage;
    }
    public class BoardUpDate
    {
        public string userturn;
        public int diceA;
        public int diceB;
        public string userA;
        public string userB;
        public int[] board;
        public userAdetails userAdetails;
        public userBdetails UserBdetails;
        public string userarray;
        public double lasttimer;
        public string phase;
        public bool Opp;
        public int room;
    }
    public class Timeout
    {
        public bool won;
        public int point;
    }

    public class MoveObj
    {
        public string tag;
        public int targetI;
        public int targetJ;

        public int OriginI;
        public int OriginJ;

        public int step;

    }
    public class _Dice
    {
        public int a;
        public int b;
        public string state;
    }

    public void RequestDice()
    {
        MessageTag diceReq = new MessageTag()
        {
            tag = "dice"
        };
        
        SendWebSocketMessage(JsonUtility.ToJson(diceReq));
    }

    public async void SendWebSocketMessage(string message)
    {
        if (ws.State == WebSocketState.Open)
        {
            // Sending bytes
            

            // Sending plain text
            await ws.SendText(message);
        }
    }

    IEnumerator MeessageMOnitor()
    {
        while (true)
        {
            if (queue.Count > 0)
            {
                MessageTag tag = queue.Dequeue();
                UseMessage(tag,messageDictioanry[tag]);
            }
            yield return null;
        }
    }

    void checkForOppOffline()
    {
        MessageTag msg = new MessageTag();
        msg.tag = "callopp";
        SendWebSocketMessage(JsonUtility.ToJson(msg));
    }
    private async void OnApplicationQuit()
    {
        //print("quit");
        if (ws != null)
        {
           await ws.Close();
        }
        
    }

    float OppOflineChecker;



    private void  Update()
    {
        if(ws!=null)
        ws.DispatchMessageQueue();



        if(OppOflineWhileReconnect)
        {
            if(OppOflineChecker!=0 && OppOflineChecker>5)
            {
                checkForOppOffline();   
            }

            OppOflineChecker=Time.time; 
        }


        if (Login.instance != null)
        {
            if (ws != null)
            {
                if (ws.State != WebSocketState.Open)
                {
                    Login.instance.loginBtn.SetActive(false);
                }
                else
                {
                   
                    Login.instance.loginBtn.SetActive(true);    
                }
            } 
        }
      
    }
}
