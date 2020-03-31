using System;
using Verse;

namespace ReconAndDiscovery.Triggers
{
	public class ActivatedAction_LuciferiumGas : ActivatedAction
	{
		protected override void DoAnyFurtherActions(Pawn activatedBy, Map map, Thing trigger)
		{
			foreach (IntVec3 c in base.GetEffectArea(activatedBy.Position))
			{
				foreach (Thing thing in c.GetThingList(map))
				{
					if (thing.def.category == ThingCategory.Pawn && thing.def.race.intelligence == Intelligence.Humanlike)
					{
						Pawn pawn = thing as Pawn;
						if (pawn != null)
						{
							Hediff hediff = HediffMaker.MakeHediff(HediffDef.Named("LuciferiumHigh"), pawn, null);
							Hediff hediff2 = HediffMaker.MakeHediff(HediffDef.Named("LuciferiumAddiction"), pawn, null);
							pawn.health.AddHediff(hediff, null, null);
							pawn.health.AddHediff(hediff2, null, null);
						}
					}
				}
			}
			base.DoAnyFurtherActions(activatedBy, map, trigger);
		}
	}
}

