using System;
using System.Collections.Generic;
using ReconAndDiscovery.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery
{
	public class JobDriver_ScanAtEmitter : JobDriver
	{
		public JobDriver_ScanAtEmitter()
		{
			this.rotateToFace = TargetIndex.B;
		}

		private Corpse Corpse
		{
			get
			{
				return (Corpse)this.pawn.CurJob.GetTarget(TargetIndex.A).Thing;
			}
		}

		private HoloEmitter Emitter
		{
			get
			{
				return (HoloEmitter)this.pawn.CurJob.GetTarget(TargetIndex.B).Thing;
			}
		}

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, errorOnFailed);
        }
        protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
			yield return Toils_Reserve.Reserve(TargetIndex.B, 1, -1, null);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
			yield return Toils_Haul.StartCarryThing(TargetIndex.A, false, false);
			yield return Toils_Haul.CarryHauledThingToCell(TargetIndex.B);
			Toil t2 = Toils_General.Wait(1000);
			t2.AddFailCondition(() => !this.Emitter.GetComp<CompPowerTrader>().PowerOn);
			t2 = t2.WithProgressBar(TargetIndex.A, () => (1000f - (float)this.ticksLeftThisToil) / 1000f, false, -0.5f);
			yield return t2;
			yield return new Toil
			{
				defaultCompleteMode = ToilCompleteMode.Instant,
				initAction = delegate()
				{
					this.Emitter.GetComp<CompHoloEmitter>().Scan(this.Corpse);
				}
			};
			yield return Toils_Reserve.Release(TargetIndex.B);
			yield break;
		}

		private const TargetIndex CorpseIndex = TargetIndex.A;

		private const TargetIndex GraveIndex = TargetIndex.B;
	}
}

