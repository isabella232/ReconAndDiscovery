using System;
using ReconAndDiscovery.Triggers;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public class SitePartWorker_Osiris : SitePartWorker
	{
		public override void PostMapGenerate(Map map)
		{
			base.PostMapGenerate(map);
			IntVec3 loc;
			if (RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith((IntVec3 x) => x.Standable(map) && x.Fogged(map) && GridsUtility.GetRoom(x, map, RegionType.Set_Passable).CellCount <= 30, map, out loc))
			{
				Thing thing = ThingMaker.MakeThing(ThingDef.Named("RD_OsirisCasket"), null);
				GenSpawn.Spawn(thing, loc, map);
				OsirisCasket osirisCasket = thing as OsirisCasket;
                Faction faction = Find.FactionManager.RandomEnemyFaction(false, false, true, TechLevel.Spacer);
                Pawn thing2 = PawnGenerator.GeneratePawn(PawnKindDefOf.AncientSoldier, Find.FactionManager.RandomNonHostileFaction(false, false, true, TechLevel.Spacer));
				osirisCasket.TryAcceptThing(thing2, true);
			}
		}

		public ActivatedActionDef action;
	}
}

