using System;
using ReconAndDiscovery.Triggers;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public class SitePartWorker_WeatherSat : SitePartWorker
	{
		public override void PostMapGenerate(Map map)
		{
			base.PostMapGenerate(map);
			IntVec3 loc;
			if (RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith((IntVec3 x) => x.Standable(map) && x.Fogged(map) && GridsUtility.GetRoom(x, map, RegionType.Set_Passable).CellCount <= 30, map, out loc))
			{
				Thing newThing = ThingMaker.MakeThing(ThingDef.Named("RD_WeatherSat"), null);
				GenSpawn.Spawn(newThing, loc, map);
			}
		}

		public ActivatedActionDef action;
	}
}

