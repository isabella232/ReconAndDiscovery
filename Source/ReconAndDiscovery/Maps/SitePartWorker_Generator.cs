using System;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public class SitePartWorker_Generator : SitePartWorker
	{
		public override void PostMapGenerate(Map map)
		{
			base.PostMapGenerate(map);
            if (RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith((IntVec3 x) => x.Standable(map) && !x.Fogged(map), map, out IntVec3 loc))
            {
                Thing newThing = ThingMaker.MakeThing(ThingDefOf.GeothermalGenerator, null);
                GenSpawn.Spawn(newThing, loc, map);
            }
        }
	}
}

