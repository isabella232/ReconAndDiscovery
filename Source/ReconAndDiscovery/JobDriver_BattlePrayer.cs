using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery
{
	public class JobDriver_BattlePrayer : JobDriver
	{
		public JobDriver_BattlePrayer()
		{
			this.rotateToFace = TargetIndex.A;
		}

		private Building Building
		{
			get
			{
				return (Building)this.pawn.CurJob.GetTarget(TargetIndex.A).Thing;
			}
		}

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, errorOnFailed);
        }
        protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_Reserve.Reserve(TargetIndex.A, 4, 0, null);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			Toil t2 = Toils_General.Wait(10000);
			t2.tickAction = delegate()
			{
				if (this.ticksLeftThisToil % 100 == 50)
				{
					this.Building.GetComp<CompPsionicEmanator>().DoBattlePrayer();
				}
			};
			yield return t2;
			yield return Toils_Reserve.Release(TargetIndex.A);
			yield break;
		}

		private const TargetIndex GateIndex = TargetIndex.A;
	}
}

