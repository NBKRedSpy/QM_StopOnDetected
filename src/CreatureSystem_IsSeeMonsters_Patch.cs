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
    [HarmonyPatch(typeof(CreatureSystem), nameof(CreatureSystem.IsSeeMonsters))]
    internal class Patch_Test
    {

        public static void Postfix(Creatures creatures, MapGrid mapGrid, ref bool __result)
        {

            //Ignore if control is being held
            if(Input.GetKey(KeyCode.LeftControl) ||  Input.GetKey(KeyCode.RightControl))
            {
                return;
            }

            if (__result == true) return;

            //--- Check if a signal is being shown

            Player player = creatures.Player;

            for (int i = 0; i < creatures.Monsters.Count; i++)
            {
                Creature creature = creatures.Monsters[i];
                Monster monster = creature as Monster;

                if (monster == null) continue;

                MapCell cell = mapGrid.GetCell(creature.CreatureData.Position, checkBorders: false);


                //Previous state.  Required since ShowSignal sets the player's HasSpottedEnemyThisAP property.
                //  This avoids needing to copy the entire ShowSignal method, minus the HasSpottedEnemyThisAP state change.

                //  if HasSpottedEnemyThisAP is set, it prevents the player from being able to change speed.
                //  It appears that the game expects the ShowSignal to be called at specific times.

                // Current state before ShowSignal is called
                bool lastHasSpottedEnemyThisAP = player.HasSpottedEnemyThisAP;

                if (!creature.IsAlly(creatures.Player) && monster.ShowSignal)   //WARNING: As per above, ShowSignal changes Player state.
                {
                    __result = true;
                    player.HasSpottedEnemyThisAP = lastHasSpottedEnemyThisAP;
                    break;
                }

                player.HasSpottedEnemyThisAP = lastHasSpottedEnemyThisAP;
            }
        }

        private static bool IsShowingSignal(Monster monster)
        {
            if (monster.IsSeenByPlayer)
            {
                return false;
            }
            if (monster.CreatureData.EffectsController.HasAnyEffect<Spotted>())
            {
                return true;
            }
            if (!monster._isSpotedByPlayer && monster._creatures.Player != null && monster._creatures.Player.IsAbleToSpotAnEnemy())
            {
                float num = (monster._creatures.Player.CreatureData.EffectsController.HasAnyEffect<NeuralSonar>() ? monster._creatures.Player.CreatureData.EffectsController.SumEffectsValue((NeuralSonar e) => e.Value) : ((float)Data.Global.NotifyHiddenEnemiesRadius));
                num += monster._creatures.Player.CreatureData.EffectsController.SumEffectsValue((WoundEffectSpottedRadius w) => w.Value);
                if (monster._creatures.Player.CreatureData.Position.Distance(monster.CreatureData.Position) <= num)
                {
                    //monster._creatures.Player.HasSpottedEnemyThisAP = true;
                    return true;
                }
            }
            return monster._isSpotedByPlayer;
        }
    }
}
