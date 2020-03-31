using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery
{
	public class JobDriver_SacrificeAtAltar : JobDriver
	{
		public JobDriver_SacrificeAtAltar()
		{
			this.rotateToFace = TargetIndex.B;
		}

		private Pawn Animal
		{
			get
			{
				return (Pawn)this.pawn.CurJob.GetTarget(TargetIndex.A).Thing;
			}
		}

		private Building Altar
		{
			get
			{
				return (Building)this.pawn.CurJob.GetTarget(TargetIndex.B).Thing;
			}
		}
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_Reserve.Reserve(TargetIndex.A, 1, 1, null);
			yield return Toils_Reserve.Reserve(TargetIndex.B, 1, 1, null);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
			yield return Toils_Haul.StartCarryThing(TargetIndex.A, false, false);
			yield return Toils_Haul.CarryHauledThingToCell(TargetIndex.B);
			yield return Toils_General.Wait(1500).WithProgressBar(TargetIndex.B, () => 1f - 1f * (float)this.ticksLeftThisToil / 1500f, false, -0.5f);
			yield return new Toil
			{
				defaultCompleteMode = ToilCompleteMode.Instant,
				initAction = delegate()
				{
					DamageInfo value = new DamageInfo(DamageDefOf.ExecutionCut, 500, -1f, -1f, base.GetActor(), null, null, 0);
					this.Animal.Kill(new DamageInfo?(value));
					this.Altar.GetComp<CompPsionicEmanator>().DoPsychicShockwave();
				}
			};
			yield return Toils_Reserve.Release(TargetIndex.B);
			yield break;
		}

		private const TargetIndex AnimalIndex = TargetIndex.A;

		private const TargetIndex AltarIndex = TargetIndex.B;
	}
}

