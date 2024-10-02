using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class Login : Singleton<Login>
{
    public GameObject LoginPanel, HomePage,loginBtn,splashScreen,videoplayer;
    public TMP_InputField Email, password,password2;
    public TextMeshProUGUI err1, err2,err3;
    public Image LoadBar;
    
    public bool SocketNotConnected = true;
    public bool IWCONotConnected = true;
    public GameObject LoginFail;
    private void Start()
    {
        
        profile=new UserData();
        StartCoroutine(startLoad());
        Application.targetFrameRate = 50;
        
    }

    public void videoStarted(bool v)
    {

    }
    public GameObject IntroObject;
    public bool WaitForIntro;
    IEnumerator startLoad()
    {
        //vvsss
        ServerConnector.instance.OppOflineWhileReconnect = false;
        ServerConnector.instance.Resume = false;
        ServerConnector.instance.PlayersTurn=false;
        ServerConnector.instance.DiceA = 0;
        ServerConnector.instance.DiceB = 0;
        ServerConnector.instance.SelfeRemaining = 0;
        ServerConnector.instance.OppRemaining = 0;
        ServerConnector.instance.host = false;
        ServerConnector.instance.numberOfhitted = 0;
        ServerConnector.instance.numberOfStage = 0; 
        ServerConnector.instance.OppnumberOfhitted = 0; 
        ServerConnector.instance.OppnumberOfStage = 0;
        ServerConnector.instance.userArray = "";
        ServerConnector.instance.ServerLastTimer = 0;
        ServerConnector.instance.ServerPhase = "";
        ServerConnector.instance.friendreqSent = false;

        yield return null;
        if (!UserProfile.instance.GameLaunched)
        {
            WaitForIntro = true;
            IntroObject.SetActive(true);
            while (WaitForIntro)
            {                
                yield return null;
            }
        }

       
        print("vid finish");
       // yield return new WaitForSeconds(4);
        MenuAudioManager.instance.enableMusic();
        //videoplayer.SetActive(false);   
        splashScreen.SetActive(true);
        ServerConnector.instance.ServerStart();
        float load = 0;
        SocketNotConnected = true;
        float startConnectingToWebsocket;
        while (SocketNotConnected || load<0.3f)
        {
            
            load += 0.01f;
            if (load > 0.7f && SocketNotConnected)
            {
                load = 0.7f;
            }
//            print(load+""+notconnected);
            LoadBar.fillAmount = load;
            yield return null;
        }
        print("ws connected");
        if (PlayerPrefs.HasKey("email"))
        {
            StartCoroutine(LogIn("https://game.iwco.io/api/auth/login", PlayerPrefs.GetString("email"), PlayerPrefs.GetString("password")));
            while (IWCONotConnected || load < 0.7f)
            {
                load += 0.01f;
                if (load > 0.7f && IWCONotConnected)
                {
                    load = 0.7f;
                }
                //            print(load+""+notconnected);
                LoadBar.fillAmount = load;
                yield return null;
            }
        }
        

        while (true)
        {
           // print("connected");
            load += 0.001f;
            LoadBar.fillAmount = load;
            if (load > 1)
            {
                gamestart();
                yield break;
            }
        }
        
    }
    

    void gamestart()
    {
        print("game start");
        splashScreen.SetActive(false);
        if (PlayerPrefs.HasKey("email"))
        {
           
            return;
        }
        LoginPanel.GetComponent<CanvasGroup>().alpha = 0;
        LeanTween.alphaCanvas(LoginPanel.GetComponent<CanvasGroup>(), 1, 1);
        LoginPanel.SetActive(true);
        HomePage.SetActive(false);
        homePage.instance.PlayConfirm.SetActive(false);
    }

    public void LoginSuccess()
    {
        HomePage.SetActive(true);
        LoginPanel.SetActive(false);
        homePage.instance.RoomAnimation();
        UserProfile.instance.GameLaunched = true;
        homePage.instance.SetUserUi();
    }

    public void ActivateLogin()
    {
        loginBtn.SetActive(true);
    }
    
    

    

    private Action<bool> sendmes;

    public void EmailExists()
    {
        err3.gameObject.SetActive(true);
        err3.text = "Email already Exist";
    }
    
    
    public void TryLogin()
    { 
        MenuAudioManager.instance.playClick();
        err3.gameObject.SetActive(false);
        bool cansignup = true;
        err1.text = "";
        err2.text = "";
        if (password.text == "" )
        {
            cansignup = false;
            if (err1.gameObject.activeInHierarchy)
            {
                if (err2.gameObject.activeInHierarchy)
                {
                    err1.gameObject.SetActive(true);
                    err1.text = "fill  passwords";
                }
                else
                {
                    err2.gameObject.SetActive(true);
                    err2.text = "fill  passwords";
                }
            }else
            {
                err1.gameObject.SetActive(true);
                err1.text = "fill  passwords";
            }
        }
       
        
        
        bool isEmail = Regex.IsMatch(Email.text, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        print(isEmail);

        if (!isEmail)
        {
            
            cansignup = false;
            if (err1.gameObject.activeInHierarchy)
            {
                if (err2.gameObject.activeInHierarchy)
                {
                    err1.gameObject.SetActive(true);
                    err1.text = "incorrect email";
                }
                else
                {
                    err2.gameObject.SetActive(true);
                    err2.text = "incorrect email";
                }
            }else
            {
                err1.gameObject.SetActive(true);
                err1.text = "incorrect email";
            }
        }

        if (password.text.Length < 6)
        {
            
            cansignup = false;
            if (err1.gameObject.activeInHierarchy)
            {
                if (err2.gameObject.activeInHierarchy)
                {
                    err1.gameObject.SetActive(true);
                    err1.text = "password should be at list 6 char";
                }
                else
                {
                    err2.gameObject.SetActive(true);
                    err2.text = "password should be at list 6 char";
                }
            }else
            {
                err1.gameObject.SetActive(true);
                err1.text = "password should be at list 6 char";
            }
        }
        
        if (cansignup)
        {
          //  ServerConnector.instance.TrySignUp(Email.text,password.text); 
        }

        StartCoroutine(LogIn("https://game.iwco.io/api/auth/login",Email.text,password.text));

    }
    public class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            //Simply return true no matter what
            return true;
        }
    }
    IEnumerator LogIn(string url,string _email,string _password)
    {
        WWWForm form = new WWWForm();
        form.AddField("email",_email);
        form.AddField("password",_password);
        
        UnityWebRequest uwr=UnityWebRequest.Post(url,form);
        uwr.certificateHandler = new BypassCertificate();
        yield return uwr.SendWebRequest();
        if (uwr.isNetworkError)
        {
            Debug.Log("//Error While Sending: " + uwr.error);
            StartCoroutine(LogIn("https://game.iwco.io/api/auth/login", PlayerPrefs.GetString("email"), PlayerPrefs.GetString("password")));
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
            LogInResult res = JsonConvert.DeserializeObject<LogInResult>(uwr.downloadHandler.text);
            if(res.access_token==null)
            {
                err1.gameObject.SetActive(true);
                err1.text = "Try again";
                yield break;
            }else
            {
                err1.gameObject.SetActive(false);
            }

            print("setting token");
            //
            ServerConnector.instance.GetRoomsFromServer();
            //
            PlayerPrefs.SetString("token",res.access_token);
            WWWForm form2 = new WWWForm();
           // form2.headers["bearer token"] = res.access_token;
            var url2 = "https://game.iwco.io/api/auth/info";
            
            UnityWebRequest uwr2 = UnityWebRequest.Post(url2,"");
            uwr2.certificateHandler = new BypassCertificate();  
            uwr2.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
            uwr2.SetRequestHeader("Authorization", "Bearer " + res.access_token);
            yield return uwr2.SendWebRequest();
            if (uwr2.isNetworkError)
            {
                Debug.Log("Error While Sending: " + uwr2.error);
                StartCoroutine(LogIn("https://game.iwco.io/api/auth/login", PlayerPrefs.GetString("email"), PlayerPrefs.GetString("password")));
            }
            else
            {
                UserProfile.instance.setEmail(_email);
                UserProfile.instance.SetPassword(_password);

                Debug.Log("Received2: " + uwr2.downloadHandler.text);
                UserData dt=JsonConvert.DeserializeObject<UserData>(uwr2.downloadHandler.text);
                UserProfile.instance.SetUserName(dt.username);
                ServerConnector.instance.CheckForUserCount();
                profile = dt;
                SyncUserDataWithCryptoServer(profile);  
                GetUserCoinFromServer(res.access_token);

            } 
        }
    }
    UserData profile;

    public void GetUserCoinFromServer(string token)
    {
        StartCoroutine(Co_getCoin(token));
    }



    IEnumerator Co_getCoin(string token)
    {
        var CoinUrl = "https://game.iwco.io/api/auth/balance";
        UnityWebRequest uwr3 = UnityWebRequest.Post(CoinUrl, "");
        uwr3.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
        uwr3.SetRequestHeader("Authorization", "Bearer " + token);
        yield return uwr3.SendWebRequest();
        if (uwr3.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr3.error);
        }
        else
        {
            Debug.Log("Received3: " + uwr3.downloadHandler.text);
            UserData coins = JsonConvert.DeserializeObject<UserData>(uwr3.downloadHandler.text);
            UserProfile.instance.setCoin(coins.point);
           // IWCONotConnected = false;
            //LoginSuccess();
        }
    }

    public void SyncUserDataWithCryptoServer(UserData userData)
    {
        ServerConnector.instance.SyncUserData(userData);
    }

    public void LogOut()
    {
        
        StartCoroutine(Co_LogOut(PlayerPrefs.GetString("token")));

    }
    public void GetUserAbility()
    {
        Shop.instance.MaximumPoints = 0;
        StartCoroutine(Co_GetUserAbility(PlayerPrefs.GetString("token")));  
    }

    IEnumerator Co_GetUserAbility(string token)
    {
        var CoinUrl = "https://game.iwco.io/api/auth/balance";
        UnityWebRequest uwr3 = UnityWebRequest.Post(CoinUrl, "");
        uwr3.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
        uwr3.SetRequestHeader("Authorization", "Bearer " + token);
        yield return uwr3.SendWebRequest();
        if (uwr3.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr3.error);
        }
        else
        {
            Debug.Log("Received3: " + uwr3.downloadHandler.text);
            
            UserData dt=JsonConvert.DeserializeObject<UserData>(uwr3.downloadHandler.text);
            Shop.instance.MaximumPoints = dt.ability_point;
            Shop.instance.SetMaximumLabel(dt.ability_point);
            UserProfile.instance.setCoin(dt.point);
            profile.point = dt.point;   
            SyncUserDataWithCryptoServer(profile);
            //LoginSuccess();
        }
    }


    IEnumerator Co_LogOut(string token)
    {
        var CoinUrl = "https://game.iwco.io/api/auth/logout";
        UnityWebRequest uwr3 = UnityWebRequest.Post(CoinUrl, "");
        uwr3.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
        uwr3.SetRequestHeader("Authorization", "Bearer " + token);
        yield return uwr3.SendWebRequest();
        if (uwr3.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr3.error);
        }
        else
        {
            Debug.Log("Received3: " + uwr3.downloadHandler.text);
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(0);

            //LoginSuccess();
        }
    }

    [ContextMenu("token refresh")]
    public void TokenRefresh()
    {
        StartCoroutine(Co_TokenRefresh());
    }

    IEnumerator Co_TokenRefresh()
    {
        string url = "https://game.iwco.io/api/auth/refresh";
        UnityWebRequest uwr = UnityWebRequest.Post(url, "");
       // uwr.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
        uwr.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("token"));
        yield return uwr.SendWebRequest();
        if(uwr.isNetworkError)
        {
            print(uwr.error);
        }
        else
        {
            print(uwr.downloadHandler.text);
            LogInResult rt=JsonConvert.DeserializeObject<LogInResult>(uwr.downloadHandler.text);
            if(rt!=null)
            {
                PlayerPrefs.SetString("token",rt.access_token);
            }
        }

    }


    public class UserData
    {
        public string tag;
        public string username;
        public int point;
        public string uuid;
        public int ability_point;
    }

    


    public class LogInResult
    {
        public bool status;
        public string message;
        public string access_token;
    }

}
