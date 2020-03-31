using System;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public class GenStep_MilitaryBase : GenStep_AdventureGenerator
	{
		public override void Generate(Map map, GenStepParams parms)
		{
			if (!map.TileInfo.WaterCovered)
			{
				base.Generate(map, parms);
				CellRect rect = new CellRect(Rand.RangeInclusive(this.adventureRegion.minX, this.adventureRegion.maxX - 60), Rand.RangeInclusive(this.adventureRegion.minZ, this.adventureRegion.maxZ - 60), 60, 60);
				rect.ClipInsideMap(map);
				ResolveParams baseResolveParams = this.baseResolveParams;
				baseResolveParams.rect = rect;
				BaseGen.symbolStack.Push("oldMilitaryBase", baseResolveParams, null);
				BaseGen.Generate();
				MapGenUtility.MakeDoors(new ResolveParams
				{
					wallStuff = ThingDefOf.Steel
				}, map);
				MapGenUtility.UnfogFromRandomEdge(map);
				MapGenUtility.ResolveCustomGenSteps(map, parms);
				int num = Rand.RangeInclusive(1, 2);
				MapGenUtility.ScatterWeaponsWhere(baseResolveParams.rect, num, map, (ThingDef thing) => thing.IsRangedWeapon && !thing.destroyOnDrop && thing.weaponTags != null && thing.weaponTags.Contains("Gun") && (thing.weaponTags.Contains("GunHeavy") || !thing.weaponTags.Contains("AdvancedGun")));
			}
		}
	}
}

