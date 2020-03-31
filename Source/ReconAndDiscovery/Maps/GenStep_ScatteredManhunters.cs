using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public class GenStep_ScatteredManhunters : GenStep
	{
        public override int SeedPart
        {
            get
            {
                return 349640110;
            }
        }
        public override void Generate(Map map, GenStepParams parms)
		{
			float num = this.pointsRange.RandomInRange;
			List<Pawn> list = new List<Pawn>();
			for (int i = 0; i < 50; i++)
			{
				PawnKindDef pawnKindDef;
				if (!ManhunterPackIncidentUtility.TryFindManhunterAnimalKind(this.pointsRange.RandomInRange, map.Tile, out pawnKindDef))
				{
					return;
				}
				list.Add(PawnGenerator.GeneratePawn(pawnKindDef, null));
				num -= pawnKindDef.combatPower;
				if (num <= 0f)
				{
					break;
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				IntVec3 intVec;
				if (CellFinderLoose.TryGetRandomCellWith((IntVec3 x) => x.Standable(map) && x.Fogged(map) && GridsUtility.GetRoom(x, map, RegionType.Set_Passable).CellCount >= 4, map, 1000, out intVec))
				{
					IntVec3 intVec2 = CellFinder.RandomSpawnCellForPawnNear(intVec, map, 10);
					GenSpawn.Spawn(list[j], intVec2, map, Rot4.Random, WipeMode.Vanish, false);
					list[j].mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent, null, false, false, null);
				}
			}
		}

		public FloatRange pointsRange = new FloatRange(250f, 700f);
	}
}

