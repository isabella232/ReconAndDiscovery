using System;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public class SymbolResolver_WallDoor : SymbolResolver
	{
		public override bool CanResolve(ResolveParams rp)
		{
			return base.CanResolve(rp);
		}

		public override void Resolve(ResolveParams rp)
		{
			if (rp.rect.Width <= 1 || rp.rect.Height <= 1)
			{
				if (rp.wallStuff == null)
				{
					rp.wallStuff = BaseGenUtility.RandomCheapWallStuff(Faction.OfPlayer, false);
				}
				IntVec3 randomCell = rp.rect.RandomCell;
				this.TryPlaceDoor(randomCell, rp.wallStuff, null);
			}
		}

		private void TryPlaceDoor(IntVec3 loc, ThingDef doorStuff, Faction faction = null)
		{
			MapGenUtility.PushDoor(loc);
		}

		private bool IsOutdoorsAt(IntVec3 c)
		{
			Map map = BaseGen.globalSettings.map;
			return GridsUtility.GetRegion(c, map, RegionType.Set_Passable) != null && GridsUtility.GetRegion(c, map, RegionType.Set_Passable).Room.PsychologicallyOutdoors;
		}
	}
}

