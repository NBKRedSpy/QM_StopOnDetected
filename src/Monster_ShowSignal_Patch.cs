using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MGSC;
using UnityEngine;

namespace QM_StopOnDetected
{
    /// <summary>
    /// Allows the player to keep walking if they are holding down the control key while a hidden
    /// unit is detected.
    /// 
    /// Resets the Player's HasSpottedEnemyThisAP property if the player was holding down the control key
    /// and something else has not already set the flag.
    /// </summary>
    [HarmonyPatch(typeof(Monster), nameof(Monster.ShowSignal), MethodType.Getter)]
    internal static class Monster_ShowSignal_Patch
    {
        private static bool PreviousHasSpottedEnemyThisAP = false;

        public static void Prefix(Monster __instance)
        {
            //Store this so the Postfix can abort early.
            PreviousHasSpottedEnemyThisAP = __instance._creatures.Player.HasSpottedEnemyThisAP;
        }

        public static void Postfix(Monster __instance)
        {
            //This patch is a little odd.  The will stop movement if the player can see a monster in the action processing
            //function.  However, the game will stop the player from moving if the Player.PreviousHasSpottedEnemyThisAP has
            //been set.  Oddly it is in this getter function that it is set.

            //It's understandable from a quick and dirty implementation since this is the function that determines if the 
            //red asterisk "signal" is shown.  Just odd that it wasn't just put in the other function.

            if (PreviousHasSpottedEnemyThisAP == true) return;  //Something else already triggered the flag.

            //Check if the control keys is being held down            
            if (!(Input.GetKey(KeyCode.LeftControl) ||  Input.GetKey(KeyCode.RightControl))) return;

            __instance._creatures.Player.HasSpottedEnemyThisAP = false;
        }
    }
}
