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

            //Ignore if shift is being held
            if(Input.GetKey(KeyCode.LeftControl) ||  Input.GetKey(KeyCode.LeftControl))
            {
                return;
            }

            if (__result == true) return;

            //--- Check if a signal is being shown

            for (int i = 0; i < creatures.Monsters.Count; i++)
            {
                Creature creature = creatures.Monsters[i];
                Monster monster = creature as Monster;

                if (monster == null) continue;

                MapCell cell = mapGrid.GetCell(creature.pos, checkBorders: false);
                if (!creature.IsAlly(creatures.Player) && monster.ShowSignal)
                {
                    __result = true;
                    break;
                }
            }
        }
    }
}
