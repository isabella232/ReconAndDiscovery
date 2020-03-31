using System;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public class GenStep_AbandonedLab : GenStep_AdventureGenerator
	{
		public override void Generate(Map map, GenStepParams parms)
		{
			if (!map.TileInfo.WaterCovered)
			{
				base.Generate(map, parms);
				CellRect rect = new CellRect(Rand.RangeInclusive(this.adventureRegion.minX, this.adventureRegion.maxX - 50), Rand.RangeInclusive(this.adventureRegion.minZ, this.adventureRegion.maxZ - 50), 50, 50);
				rect.ClipInsideMap(map);
				ResolveParams baseResolveParams = this.baseResolveParams;
				baseResolveParams.rect = rect;
				BaseGen.symbolStack.Push("abandonedLab", baseResolveParams, null);
				BaseGen.Generate();
				MapGenUtility.MakeDoors(new ResolveParams
				{
					wallStuff = ThingDefOf.Plasteel
				}, map);
				MapGenUtility.ResolveCustomGenSteps(map, parms);
			}
		}
	}
}

