using System;
using ReconAndDiscovery.Triggers;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public class SitePartWorker_Computer : SitePartWorker
	{
		public override void PostMapGenerate(Map map)
		{
			base.PostMapGenerate(map);
			IntVec3 loc;
			if (RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith((IntVec3 x) => x.Standable(map) && x.Fogged(map) && GridsUtility.GetRoom(x, map, RegionType.Set_Passable).CellCount <= 30, map, out loc))
			{
				ThingDef def = ThingDef.Named("RD_QuestComputerTerminal");
				Thing thing = ThingMaker.MakeThing(def, GenStuff.DefaultStuffFor(def));
				if (this.action != null)
				{
					(thing as Building).GetComp<CompComputerTerminal>().actionDef = this.action;
				}
				GenSpawn.Spawn(thing, loc, map);
			}
		}

		public ActivatedActionDef action;
	}
}

