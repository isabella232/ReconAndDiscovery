using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery
{
	public class JobDriver_DiscoverStargates : JobDriver
	{
		public JobDriver_DiscoverStargates()
		{
			this.rotateToFace = TargetIndex.A;
		}

		private Building Stargate
		{
			get
			{
				return (Building)this.pawn.CurJob.GetTarget(TargetIndex.A).Thing;
			}
		}

		private void TriggerFindStargate()
		{
			IncidentParms incidentParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, Find.World);
			incidentParms.forced = true;
			QueuedIncident qi = new QueuedIncident(new FiringIncident(ThingDefOfReconAndDiscovery.RD_DiscoveredStargate, null, incidentParms), Find.TickManager.TicksGame + 100);
			Find.Storyteller.incidentQueue.Add(qi);
		}
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, errorOnFailed);
        }
        protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			Toil t2 = Toils_General.Wait(1000);
			t2 = t2.WithEffect(EffecterDefOf.Research, TargetIndex.A);
			float fResearchScore = base.GetActor().GetStatValue(StatDefOf.ResearchSpeed, true);
			t2.tickAction = delegate()
			{
				if (180000f * Rand.Value < fResearchScore)
				{
					this.TriggerFindStargate();
					this.EndJobWith(JobCondition.Succeeded);
				}
			};
			yield return t2;
			yield return Toils_Reserve.Release(TargetIndex.A);
			yield break;
		}

		private const TargetIndex GateIndex = TargetIndex.A;
	}
}

