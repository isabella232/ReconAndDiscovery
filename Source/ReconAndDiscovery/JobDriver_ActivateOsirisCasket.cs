using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery
{
	public class JobDriver_ActivateOsirisCasket : JobDriver
	{
		public JobDriver_ActivateOsirisCasket()
		{
			this.rotateToFace = TargetIndex.A;
		}

		private OsirisCasket Casket
		{
			get
			{
				return (OsirisCasket)this.pawn.CurJob.GetTarget(TargetIndex.A).Thing;
			}
		}

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
			Toil tFuel = new Toil();
			tFuel.defaultCompleteMode = ToilCompleteMode.Instant;
			tFuel.AddFailCondition(() => this.Casket.GetComp<CompRefuelable>().Fuel < 50f);
			tFuel.AddFailCondition(() => !this.Casket.GetComp<CompPowerTrader>().PowerOn);
			tFuel.initAction = delegate()
			{
				this.Casket.GetComp<CompRefuelable>().ConsumeFuel(25f);
			};
			yield return tFuel;
			Toil t2 = Toils_General.Wait(6000);
			t2.AddFailCondition(() => !this.Casket.GetComp<CompPowerTrader>().PowerOn);
			t2.initAction = delegate()
			{
				this.Casket.Map.weatherManager.TransitionTo(WeatherDef.Named("RainyThunderstorm"));
			};
			t2 = t2.WithProgressBar(TargetIndex.A, () => (6000f - (float)this.ticksLeftThisToil) / 6000f, false, -0.5f);
			yield return t2;
			yield return new Toil
			{
				defaultCompleteMode = ToilCompleteMode.Instant,
				initAction = delegate()
				{
                    GenExplosion.DoExplosion(this.Casket.Position, this.Casket.Map, 50f, DamageDefOf.EMP, this.Casket,
                        -1, -1f, SoundDefOf.EnergyShield_Broken, null, null, null, null, 0f, 1, false, null, 0f, 1);
					this.Casket.GetComp<CompOsiris>().HealContained();
					this.Casket.Map.weatherManager.TransitionTo(WeatherDef.Named("Rain"));
					IncidentParms incidentParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.FactionArrival, this.Casket.Map);
					incidentParms.forced = true;
					incidentParms.target = this.Casket.Map;
					QueuedIncident qi = new QueuedIncident(new FiringIncident(IncidentDef.Named("ShortCircuit"), null, incidentParms), Find.TickManager.TicksGame + 1);
					Find.Storyteller.incidentQueue.Add(qi);
				}
			};
			yield return Toils_Reserve.Release(TargetIndex.A);
			yield break;
		}

		private const TargetIndex CasketIndex = TargetIndex.A;
	}
}

