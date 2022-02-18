using System;
using System.Linq;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;

namespace ReconAndDiscovery.HarmonyPatches
{
    // [HarmonyPatch(typeof(Site), nameof(Site.PostMapGenerate))]
    // public class SitePartDefaultParams
    // {
    //     public static bool Prefix(Site __instance)
    //     {
    //         var map = __instance.Map;
    //         for (int index = 0; index < __instance.parts.Count; ++index)
    //             __instance.parts[index].def.Worker.PostMapGenerate(map);
    //         float a = 0.0f;
    //         for (int index = 0; index < __instance.parts.Count; ++index)
    //             a = Mathf.Max(a, __instance.parts[index].def.forceExitAndRemoveMapCountdownDurationDays);
    //         float num = a * MapParentTuning.SiteDetectionCountdownMultiplier.RandomInRange;
    //         if (!__instance.parts.Any<SitePart>((p => p.def.disallowsAutomaticDetectionTimerStart)))
    //         {
    //             int ticks = Mathf.RoundToInt(num * 60000f);
    //             __instance.GetComponent<TimedDetectionRaids>().StartDetectionCountdown(ticks);
    //         }
    //         // __instance.allEnemiesDefeatedSignalSent = false;
    //         
    //         return false;
    //     }
    // }
}