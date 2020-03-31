using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery
{
	public class WorkGiver_PsychicPrayer : WorkGiver_Scanner
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
			bool result;
			if (!(t is Building))
			{
				result = false;
			}
			else if (!pawn.story.traits.HasTrait(TraitDef.Named("PsychicSensitivity")))
			{
				JobFailReason.Is("RD_OnlyPsychicCanBroadcast".Translate()); //"Only psychic pawns can broadcast a battle prayer"
				result = false;
			}
			else
			{
				result = ReservationUtility.CanReserveAndReach(pawn, t, PathEndMode.Touch, Danger.Some, 1, -1, null, forced);
			}
			return result;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return new Job(JobDefOfReconAndDiscovery.RD_PsychicPrayer, t);
		}
	}
}

