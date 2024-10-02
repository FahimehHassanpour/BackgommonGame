using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCounter : MonoBehaviour
{
   public Sprite a1, a2, a3, a4, a5, a6, a7, a8, a9, a0;
   public SpriteRenderer a, b, c;

   [ContextMenu("calc")]
   public void TurnNumberToImage(int val )
   {
      var dec2 = val / 100;
      var dec1 = (val % 100)/10;
      var dec0 = val % 10;
     // print($"{dec2}-{dec1}-{dec0}");

      switch (dec2)
      {
         case 0:
            a.sprite = null;
            break;
         case 1:
            a.sprite = a1;
            break;
         case 2:
            a.sprite = a2;
            break;
         case 3:
            a.sprite = a3;
            break;
         case 4:
            a.sprite = a4;
            break;
         case 5:
            a.sprite = a5;
            break;
         case 6:
            a.sprite = a6;
            break;
         case 7:
            a.sprite = a7;
            break;
         case 8:
            a.sprite = a8;
            break;
         case 9:
            a.sprite = a9;
            break;

      }
      
      
      switch (dec1)
      {
         case 0:
            if (dec2 == 0)
            {
               b.sprite = null;
            }
            else
            {
               b.sprite = a0;
            }
           
            break;
         case 1:
            b.sprite = a1;
            break;
         case 2:
            b.sprite = a2;
            break;
         case 3:
            b.sprite = a3;
            break;
         case 4:
            b.sprite = a4;
            break;
         case 5:
            b.sprite = a5;
            break;
         case 6:
            b.sprite = a6;
            break;
         case 7:
            b.sprite = a7;
            break;
         case 8:
            b.sprite = a8;
            break;
         case 9:
            b.sprite = a9;
            break;

      }
      
      switch (dec0)
      {
         case 0:
            c.sprite = a0;
            break;
         case 1:
            c.sprite = a1;
            break;
         case 2:
            c.sprite = a2;
            break;
         case 3:
            c.sprite = a3;
            break;
         case 4:
            c.sprite = a4;
            break;
         case 5:
            c.sprite = a5;
            break;
         case 6:
            c.sprite = a6;
            break;
         case 7:
            c.sprite = a7;
            break;
         case 8:
            c.sprite = a8;
            break;
         case 9:
            c.sprite = a9;
            break;

      }
      
   }
}
