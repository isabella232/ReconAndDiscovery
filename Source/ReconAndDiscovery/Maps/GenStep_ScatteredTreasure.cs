using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public class GenStep_ScatteredTreasure : GenStep
	{
        public override int SeedPart
        {
            get
            {
                return 319964010;
            }
        }
        public override void Generate(Map map, GenStepParams parms)
		{
			float num = Mathf.Min(18000f, Mathf.Max(new float[]
			{
				Mathf.Exp(Rand.Gaussian(8f, 0.65f))
			}));
            ThingSetMakerParams value = default(ThingSetMakerParams);
            value.techLevel = TechLevel.Spacer;
            int count = Rand.RangeInclusive(2, 10);
            value.countRange = new IntRange?(new IntRange(count, count));
            value.totalMarketValueRange = new FloatRange?(new FloatRange(num, num));
            value.validator = ((ThingDef t) => t.defName != "Silver");
			if (num > 10000f)
			{
				value.countRange = new IntRange?(new IntRange(1, 1));
			}
			List<Thing> list = ThingSetMakerDefOf.Reward_ItemsStandard.root.Generate(value);
            foreach (Thing thing in list)
			{
				if (thing.stackCount > thing.def.stackLimit)
				{
					thing.stackCount = thing.def.stackLimit;
				}
				IntVec3 intVec;
				if (CellFinderLoose.TryGetRandomCellWith((IntVec3 x) => x.Standable(map) && x.Fogged(map) && GridsUtility.GetRoom(x, map, RegionType.Set_Passable).CellCount >= 2, map, 1000, out intVec))
				{
                    // TODO: check if it works
                    GenSpawn.Spawn(thing, intVec, map, Rot4.Random, WipeMode.Vanish, false);
				}
			}
		}
	}
}

