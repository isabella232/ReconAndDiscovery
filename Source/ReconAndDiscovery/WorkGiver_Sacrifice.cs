using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery
{
    public class WorkGiver_Sacrifice : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest =>
            ThingRequest.ForDef(ThingDef.Named("RD_PsionicEmanator"));

        public override bool Prioritized => true;

        public override float GetPriority(Pawn pawn, TargetInfo t)
        {
            return 0f;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            bool result;
            if (!(t is Building building_PsionicEmanator))
            {
                result = false;
            }
            else if (!pawn.Map.designationManager.SpawnedDesignationsOfDef(DesignationDefOf.Slaughter).Any())
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
                result = pawn.CanReserve(building_PsionicEmanator, 1, -1, null, forced);
            }

            return result;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            var building_PsionicEmanator = t as Building;
            Job result;
            if (pawn.CanReserveAndReach(building_PsionicEmanator, PathEndMode.ClosestTouch, Danger.Some))
            {
                var victim = (Thing) pawn.Map.designationManager.SpawnedDesignationsOfDef(DesignationDefOf.Slaughter)
                    .RandomElement().target;
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