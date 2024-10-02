using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserProfile : Singleton<UserProfile>
{
    private int coin,level;
    private string userName,OppuserName,Opplevel,email,password;

    #region coin methods
    public void AddToCoin(int val)
    {
        coin += val;
        homePage.instance.SetCoinText();
    }
    public void RemoveCoin(int val)
    {
        coin = coin-val>=0?coin-val:0;
        homePage.instance.SetCoinText();
    }
    public int GetCoin()
    {
        coin = PlayerPrefs.GetInt("coin");
        return coin;
    }
    public void setCoin(int val)
    {
        coin = val;
        PlayerPrefs.SetInt("coin",val);
        homePage.instance.SetCoinText();
       // homePage.instance.SetUserUi();
    }
    
    [ContextMenu("testDecimal")]
    public void testc()
    {
        TurnNumberToDecimalpointSeperator(1234567890);
    }

    public string TurnNumberToDecimalpointSeperator(string val)
    {
        string res = "";
        var input = val;
        int c = 0;
        for (int i = input.Length - 1; i > -1; i--)
        {
            res += input[i].ToString();
            if (c == 2)
            {
                if (i - 1 >= 0)
                {
                    res += ",";
                }

                c = -1;
            }
            c++;
        }

        char[] charArray = res.ToCharArray();
        Array.Reverse(charArray);
        res = new string(charArray);
        print(res);
        return res;
    }

    public string TurnNumberToDecimalpointSeperator(int val)
    {
        return(TurnNumberToDecimalpointSeperator(val.ToString()));
    }

    #endregion

    #region UserName

    public string getUserName()
    {        
        userName = PlayerPrefs.GetString("name");        
        return userName;
    }

    public void SetUserName(string val)
    {

        userName = val;
        homePage.instance.SetuserNameText();
       PlayerPrefs.SetString("name",val);
       
    }
    #endregion

    #region userLevel
    
    public int GetLevel()
    {
        if (level == 0)
        {
            level = PlayerPrefs.GetInt("level");
        }
        return level;
    }
    public void setLevel(int val)
    {
        level = val;
        PlayerPrefs.SetInt("level",val);
    }
    

    #endregion

    #region OppRegion

    public string GetOppName()
    {
        var name= PlayerPrefs.GetString("opUsername");
        return name;
    }

    public void SetOppUserName(string val)
    {
        OppuserName = val;
        print("setting opp Name"+val);
        PlayerPrefs.SetString("opUsername", val);
    }

    public string getOppLevel()
    {

        return Opplevel;
    }

    public void SetOppLevel(string val)
    {
        Opplevel = val;
    }
    
    #endregion

    public string getEmail()
    {
        if (email == null)
        {
            email = PlayerPrefs.GetString("email");
        }
        return email;
    }

    public void setEmail(string val)
    {
        email = val;
        PlayerPrefs.SetString("email",val);
    }

    public void SetPassword(string val)
    {
        password = val;
        PlayerPrefs.SetString ("password",val); 
    }

    public string GetPassword()
    {
        if(password == null)
        { password = PlayerPrefs.GetString("password");}
        return password;
    }

    public bool GameLaunched;
}
