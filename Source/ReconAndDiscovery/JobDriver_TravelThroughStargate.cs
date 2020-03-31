using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery
{
	public class JobDriver_TravelThroughStargate : JobDriver
	{
		public JobDriver_TravelThroughStargate()
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

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
			yield return new Toil
			{
				defaultCompleteMode = ToilCompleteMode.Instant,
				initAction = delegate()
				{
					CompStargate comp = this.Stargate.GetComp<CompStargate>();
					comp.SendPawnThroughStargate(base.GetActor());
				}
			};
			yield break;
		}

		private const TargetIndex StargateIndex = TargetIndex.A;
	}
}

