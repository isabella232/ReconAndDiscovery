using System;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public class GenStep_OldCastle : GenStep_AdventureGenerator
	{
		public override void Generate(Map map, GenStepParams parms)
		{
			if (!map.TileInfo.WaterCovered)
			{
				base.Generate(map, parms);
				int num = Rand.RangeInclusive(55, 80);
				int num2 = Rand.RangeInclusive(55, 80);
				CellRect rect = new CellRect(Rand.RangeInclusive(this.adventureRegion.minX, this.adventureRegion.maxX - num), Rand.RangeInclusive(this.adventureRegion.minZ, this.adventureRegion.maxZ - num2), num, num2);
				ResolveParams baseResolveParams = this.baseResolveParams;
				baseResolveParams.rect = rect;
				BaseGen.globalSettings.map = map;
				BaseGen.symbolStack.Push("oldCastle", baseResolveParams, null);
				BaseGen.Generate();
				MapGenUtility.MakeDoors(new ResolveParams
				{
					wallStuff = ThingDefOf.WoodLog
				}, map);
				BaseGen.Generate();
				MapGenUtility.ScatterWeaponsWhere(baseResolveParams.rect, Rand.RangeInclusive(7, 11), map, (ThingDef thing) => thing.weaponTags != null && thing.equipmentType == EquipmentType.Primary && !thing.destroyOnDrop && !thing.weaponTags.Contains("Gun"));
				MapGenUtility.ResolveCustomGenSteps(map, parms);
			}
		}
	}
}

