using System;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public class GenStep_RareBeasts : GenStep
	{
        public override int SeedPart
        {
            get
            {
                return 349231510;
            }
        }
        public override void Generate(Map map, GenStepParams parms)
		{
			PawnKindDef pawnKindDef = ThingDefOfReconAndDiscovery.RD_Devillo;
			if (Rand.Chance(0.4f))
			{
				pawnKindDef = ThingDefOfReconAndDiscovery.RD_Nitralope;
			}

            PawnGenerationRequest request = new PawnGenerationRequest(pawnKindDef, null, 
                PawnGenerationContext.NonPlayer, -1, true, true, false, false, false, false, 
                1f, false, false,
                false, true, true, true, 
                false, false, 0f, null, 1f, null, null, null,
                null, new float?(0f), new float?(1f), new float?(1f), new Gender?(Gender.Male), null, null);

			PawnGenerationRequest request2 = new PawnGenerationRequest(pawnKindDef, null, 
                
                PawnGenerationContext.NonPlayer, -1, true, true, false, false, false, false, 1f, false, false, 
                false, false, false, false, false, false, 0f, null, 1f, null, null, null, 
                null, new float? (0f), new float?(0f), new float?(1f), new Gender?(Gender.Female), null, null);

			IntVec3 intVec;
			if (RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith((IntVec3 x) => x.Standable(map) && x.Fogged(map) && GridsUtility.GetRoom(x, map, RegionType.Set_Passable).CellCount >= 4, map, out intVec))
			{
				Pawn pawn = PawnGenerator.GeneratePawn(request);
				Pawn pawn2 = PawnGenerator.GeneratePawn(request2);
				IntVec3 intVec2 = CellFinder.RandomSpawnCellForPawnNear(intVec, map, 10);
				GenSpawn.Spawn(pawn, intVec2, map, Rot4.Random, WipeMode.Vanish, false);
				intVec2 = CellFinder.RandomSpawnCellForPawnNear(intVec, map, 10);
				GenSpawn.Spawn(pawn2, intVec2, map, Rot4.Random, WipeMode.Vanish, false);
			}
		}
	}
}

