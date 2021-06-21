using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery
{
    public class JobDriver_LayDown_Scyther : JobDriver_LayDown
    {
        public override string GetReport()
        {
            string result;
            if (asleep)
            {
                result = "ReportSleeping".Translate();
            }
            else if (GetActor().RaceProps.IsMechanoid)
            {
                result = "RD_EnteringRepairCycle".Translate(); //"Entering Repair Cycle"
            }
            else
            {
                result = base.GetReport();
            }

            return result;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            var result = GetActor().RaceProps.IsMechanoid ? MechToils() : base.MakeNewToils();

            return result;
        }

        private IEnumerable<Toil> MechToils()
        {
            var toil = Toils_General.Wait(6000);
            toil.socialMode = RandomSocialMode.Off;
            toil.initAction = delegate
            {
                ticksLeftThisToil = 6000;
                var firstBuilding = GetActor().Position.GetFirstBuilding(GetActor().Map);
                if (firstBuilding is Building_Bed)
                {
                    //TODO: check if it works
                    JobMaker.MakeJob(JobDefOf.LayDown, firstBuilding);

                    //base.GetActor().jobs.curDriver.layingDown = 2;
                }
            };
            toil.tickAction = delegate
            {
                if (!Rand.Chance(0.0004f))
                {
                    return;
                }

                var injuriesTendable = GetActor().health.hediffSet.GetInjuriesTendable();
                if (!injuriesTendable.Any())
                {
                    return;
                }

                var hediff_Injury = injuriesTendable.RandomElement();
                hediff_Injury.Heal(Rand.RangeInclusive(1, 3));
            };
            toil.AddEndCondition(delegate
            {
                var result = !GetActor().health.hediffSet.GetInjuriesTendable().Any()
                    ? JobCondition.Succeeded
                    : JobCondition.Ongoing;

                return result;
            });
            yield return toil;
        }
    }
}