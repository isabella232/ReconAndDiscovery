using System;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public class GenStep_CrashedShip : GenStep_AdventureGenerator
	{
		public override void Generate(Map map, GenStepParams parms)
		{
			base.Generate(map, parms);
			CellRect rect = new CellRect(Rand.RangeInclusive(this.adventureRegion.minX, this.adventureRegion.maxX - 60), Rand.RangeInclusive(this.adventureRegion.minZ + 15, this.adventureRegion.maxZ - 15), 20, 20);
			rect.ClipInsideMap(map);
			ResolveParams baseResolveParams = this.baseResolveParams;
			baseResolveParams.rect = rect;
			BaseGen.symbolStack.Push("crashedShip", baseResolveParams, null);
			BaseGen.Generate();
			MapGenUtility.MakeDoors(new ResolveParams
			{
				wallStuff = ThingDefOf.Plasteel
			}, map);
			MapGenUtility.ResolveCustomGenSteps(map, parms);
		}
	}
}

