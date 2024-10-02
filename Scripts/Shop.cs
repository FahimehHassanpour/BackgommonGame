using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static Login;

public class Shop : Singleton<Shop>
{
    public int MaximumPoints;

    public TMPro.TMP_InputField userInput;
    public void BuySelectedCoin()
    {
        Int32.TryParse(userInput.text, out int userAmount);
        BuyCoin(userAmount);
    }
    string ShopUrl = "/money-collector/credit-to-point";
    string baseUrl = "https://game.iwco.io/api";
    public void BuyCoin(int amount)
    {
        print(amount-1);  
        if (amount > MaximumPoints || amount < 1)
        {
            //cant buy
            MenuAudioManager.instance.PlayError();
            userInput.text = "";
        }
        else
        {
            StartCoroutine(Co_Buy(PlayerPrefs.GetString("token"),amount));
            
            //buy from server
        }
    }

    IEnumerator Co_Buy(string token,int amount)
    {
        print(token);
        var shopString = baseUrl + ShopUrl;
        WWWForm form = new WWWForm();
        form.AddField("point", amount.ToString());
        print(shopString);
        UnityWebRequest uwr3 = UnityWebRequest.Post(shopString, form);
        
     // uwr3.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
        uwr3.SetRequestHeader("Authorization", "Bearer " + token);
        yield return uwr3.SendWebRequest();
        if (uwr3.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr3.error);
        }
        else
        {
            Debug.Log("Received3: " + uwr3.downloadHandler.text);

            Login.instance.GetUserAbility();
            
            //LoginSuccess();
        }
    }


    public TMPro.TextMeshProUGUI label;
    public void SetMaximumLabel(int val)
    {
        if(val==0)
        {
            label.text = " PLEASE RECHARGE YOUR WALLET!";
        }
        else
        {
            label.text = "You can Buy:  " + val.ToString();
        }
        
    }

}
