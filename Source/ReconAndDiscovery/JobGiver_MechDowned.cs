using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery
{
	public class JobGiver_MechDowned : ThinkNode_JobGiver
	{
		protected override Job TryGiveJob(Pawn pawn)
		{
			Job result;
			if (pawn.InBed())
			{
				result = new Job(JobDefOf.LayDown);
			}
			else
			{
				result = new Job(JobDefOf.Wait_Downed);
			}
			return result;
		}
	}
}

