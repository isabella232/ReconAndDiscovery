using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery
{
    public class JobGiver_MechDowned : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            var result = pawn.InBed() ? new Job(JobDefOf.LayDown) : new Job(JobDefOf.Wait_Downed);

            return result;
        }
    }
}