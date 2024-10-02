using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HomeMessageBoard : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void ShowMessage(string val)
    {
        gameObject.SetActive(true);
        text.text = val;
        StartCoroutine(hideText());
    }


    IEnumerator hideText()
    {
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
    }

}
