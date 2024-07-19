using HarmonyLib;
using MGSC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace QM_StopOnDetected
{
    public static class Plugin
    {
        public static string ModAssemblyName => Assembly.GetExecutingAssembly().GetName().Name;

        [Hook(ModHookType.AfterConfigsLoaded)]
        public static void AfterConfig(IModContext context)
        {
            new Harmony("_" + ModAssemblyName).PatchAll();
        }
    }
}
