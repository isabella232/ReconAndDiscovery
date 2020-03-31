using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace ReconAndDiscovery.Maps
{
	public class GenStep_MechanoidForces : GenStep
	{
        public override int SeedPart
        {
            get
            {
                return 339641510;
            }
        }
        public override void Generate(Map map, GenStepParams parms)
		{
			IntVec3 intVec;
			if (RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith((IntVec3 x) => x.Standable(map) && !x.Fogged(map) && GridsUtility.GetRoom(x, map, RegionType.Set_Passable).CellCount >= 4, map, out intVec))
			{
				float num = this.pointsRange.RandomInRange;
				List<Pawn> list = new List<Pawn>();
				for (int i = 0; i < 50; i++)
				{
					PawnKindDef pawnKindDef = (from kind in DefDatabase<PawnKindDef>.AllDefsListForReading
					where kind.RaceProps.IsMechanoid
					select kind).RandomElementByWeight((PawnKindDef kind) => 1f / kind.combatPower);
					list.Add(PawnGenerator.GeneratePawn(pawnKindDef, Faction.OfMechanoids));
					num -= pawnKindDef.combatPower;
					if (num <= 0f)
					{
						break;
					}
				}
				IntVec3 point = default(IntVec3);
				for (int j = 0; j < list.Count; j++)
				{
					IntVec3 intVec2 = CellFinder.RandomSpawnCellForPawnNear(intVec, map, 10);
					point = intVec2;
					GenSpawn.Spawn(list[j], intVec2, map, Rot4.Random, WipeMode.Vanish, false);
				}
				LordMaker.MakeNewLord(Faction.OfMechanoids, new LordJob_DefendPoint(point), map, list);
			}
		}

		public FloatRange pointsRange = new FloatRange(450f, 700f);
	}
}

