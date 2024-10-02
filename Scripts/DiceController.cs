using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceController : MonoBehaviour
{
    public GameObject ShadowCatcher;

    public void EnableShadowCatcher()
    {
        ShadowCatcher.SetActive(true);
    }

    public void DisableShadowCatcher()
    {
        ShadowCatcher?.SetActive(false);
    }
}
