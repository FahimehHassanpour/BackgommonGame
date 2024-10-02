using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchState : Singleton<MatchState>
{
   private int MatchPrize;

   public int GetMatchPrize()
   {
      return MatchPrize;
   }

   public void SetMatchPrize(int val)
   {
      MatchPrize = val;
   }

   private void Start()
   {
      SetMatchPrize(PlayerPrefs.GetInt("coin"));
   }
}
