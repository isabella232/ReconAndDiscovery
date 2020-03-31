using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery
{
	public class WorkGiver_Sacrifice : WorkGiver_Scanner
	{
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
                return ThingRequest.ForDef(ThingDef.Named("RD_PsionicEmanator"));
			}
		}

		public override bool Prioritized
		{
			get
			{
				return true;
			}
		}

		public override float GetPriority(Pawn pawn, TargetInfo t)
		{
			return 0f;
		}

		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
            Building building_PsionicEmanator = t as Building;
			bool result;
            if (building_PsionicEmanator == null)
			{
				result = false;
			}
            else if (pawn.Map.designationManager.SpawnedDesignationsOfDef(DesignationDefOf.Slaughter).Count() == 0)
			{
				result = false;
			}
            else if (pawn.story != null && pawn.WorkTagIsDisabled(WorkTags.Social))
			{
				JobFailReason.Is("IsIncapableOfViolenceShort".Translate());
				result = false;
			}
			else
			{
				result = ReservationUtility.CanReserve(pawn, building_PsionicEmanator, 1, -1, null, forced);
			}
			return result;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
            Building building_PsionicEmanator = t as Building;
			Job result;
			if (ReservationUtility.CanReserveAndReach(pawn, building_PsionicEmanator, PathEndMode.ClosestTouch, Danger.Some, 1, -1, null, false))
			{
                var victim = (Thing)pawn.Map.designationManager.SpawnedDesignationsOfDef(DesignationDefOf.Slaughter).RandomElement().target;
                result = new Job(JobDefOfReconAndDiscovery.RD_SacrificeAtAltar, victim, building_PsionicEmanator);
			}
			else
			{
				result = null;
			}
			return result;
		}
	}
}

