using System;
using System.Linq;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace ReconAndDiscovery.HarmonyPatches
{
    // [HarmonyPatch(typeof(Pawn_TraderTracker), nameof(Pawn_TraderTracker.CanTradeNow), methodType: MethodType.Getter)]
    // public class SitePartDefaultParams
    // {
    //     public static bool Prefix(Pawn_TraderTracker __instance, Pawn ___pawn, TraderKindDef ___traderKind, ref bool __result)
    //     {
    //         var pawn = ___pawn;
    //         var traderKind = ___traderKind;
    //
    //         Log.Message($"{traderKind.defName}");
    //
    //         if (pawn.Dead || !pawn.Spawned || !pawn.mindState.wantsToTradeWithColony ||
    //             !pawn.CanCasuallyInteractNow() || pawn.Downed || pawn.IsPrisoner || pawn.Faction == Faction.OfPlayer ||
    //             pawn.Faction != null && pawn.Faction.HostileTo(Faction.OfPlayer))
    //         {
    //             __result = false;
    //             return false;
    //         }
    //
    //         // __result = __instance.Goods.Any<Thing>((Func<Thing, bool>) (x => traderKind.WillTrade(x.def))) || traderKind.tradeCurrency == TradeCurrency.Favor;
    //         return false;
    //     }
    // }
}