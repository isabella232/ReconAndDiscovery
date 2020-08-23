using System;
using System.Reflection;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace ExpandedIncidents.Settings
{
    // Token: 0x0200000F RID: 15
    internal class WorldComp : WorldComponent
    {
        public WorldComp(World world) : base(world)
        {
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            Log.Message("Recon And Discovery - Settings loaded", false);
            RaD_ModSettings.ChangeDefPost();
        }
    }
}
