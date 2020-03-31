using System;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public class SymbolResolver_WireOutline : SymbolResolver
	{
		public override bool CanResolve(ResolveParams rp)
		{
			return base.CanResolve(rp);
		}

		public override void Resolve(ResolveParams rp)
		{
			float? chanceToSkipWallBlock = rp.chanceToSkipWallBlock;
			float chance = (chanceToSkipWallBlock == null) ? 0f : chanceToSkipWallBlock.Value;
			foreach (IntVec3 loc in rp.rect.EdgeCells)
			{
				if (!Rand.Chance(chance))
				{
					ThingDef powerConduit = ThingDefOf.PowerConduit;
					Thing newThing = ThingMaker.MakeThing(powerConduit, null);
					GenSpawn.Spawn(newThing, loc, BaseGen.globalSettings.map);
				}
			}
		}
	}
}

