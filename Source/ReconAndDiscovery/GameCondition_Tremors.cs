using System;
using RimWorld;
using Verse;

namespace ReconAndDiscovery
{
	public class GameCondition_Tremors : GameCondition
	{
		public override void End()
		{
			base.End();
		}

		public override void ExposeData()
		{
			base.ExposeData();
		}

		private void CollapseRandomRoof()
		{
            foreach (Map map in AffectedMaps)
            {
                IntVec3 intVec;
                if (CellFinderLoose.TryGetRandomCellWith((IntVec3 c) => c.Standable(map) && map.roofGrid.Roofed(c), map, 500, out intVec))
                {
                    map.roofCollapseBuffer.MarkToCollapse(intVec);
                    IntVec3[] array = new IntVec3[]
                    {
                    intVec + IntVec3.West,
                    intVec + IntVec3.East,
                    intVec + IntVec3.South,
                    intVec + IntVec3.North
                    };
                    foreach (IntVec3 c2 in array)
                    {
                        if (c2.Standable(map) && map.roofGrid.Roofed(c2))
                        {
                            map.roofCollapseBuffer.MarkToCollapse(c2);
                        }
                    }
                }
            }

		}

		public override void GameConditionTick()
		{
			if (Rand.Chance(8.333333E-05f))
			{
				this.CollapseRandomRoof();
			}
		}

		public override void Init()
		{
			this.CollapseRandomRoof();
			base.Init();
		}
	}
}

