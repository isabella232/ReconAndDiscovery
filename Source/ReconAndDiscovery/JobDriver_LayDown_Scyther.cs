using System;
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
			if (this.asleep)
			{
				result = "ReportSleeping".Translate();
			}
			else if (base.GetActor().RaceProps.IsMechanoid)
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
			IEnumerable<Toil> result;
			if (base.GetActor().RaceProps.IsMechanoid)
			{
				result = this.MechToils();
			}
			else
			{
				result = base.MakeNewToils();
			}
			return result;
		}

		private IEnumerable<Toil> MechToils()
		{
			Toil toil = Toils_General.Wait(6000);
			toil.socialMode = RandomSocialMode.Off;
			toil.initAction = delegate()
			{
				this.ticksLeftThisToil = 6000;
				Building firstBuilding = base.GetActor().Position.GetFirstBuilding(base.GetActor().Map);
				if (firstBuilding is Building_Bed)
				{
                    //TODO: check if it works
                    JobMaker.MakeJob(JobDefOf.LayDown, firstBuilding);

					//base.GetActor().jobs.curDriver.layingDown = 2;
				}
			};
			toil.tickAction = delegate()
			{
				if (Rand.Chance(0.0004f))
				{
					IEnumerable<Hediff_Injury> injuriesTendable = base.GetActor().health.hediffSet.GetInjuriesTendable();
					if (injuriesTendable.Count<Hediff_Injury>() > 0)
					{
						Hediff_Injury hediff_Injury = injuriesTendable.RandomElement<Hediff_Injury>();
						hediff_Injury.Heal((float)Rand.RangeInclusive(1, 3));
					}
				}
			};
			toil.AddEndCondition(delegate
			{
				JobCondition result;
				if (base.GetActor().health.hediffSet.GetInjuriesTendable().Count<Hediff_Injury>() == 0)
				{
					result = JobCondition.Succeeded;
				}
				else
				{
					result = JobCondition.Ongoing;
				}
				return result;
			});
			yield return toil;
			yield break;
		}
	}
}

