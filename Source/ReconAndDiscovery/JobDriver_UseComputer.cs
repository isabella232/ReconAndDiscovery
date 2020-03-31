using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery
{
	public class JobDriver_UseComputer : JobDriver
	{
		public override string GetReport()
		{
			return "RD_UsingComputer".Translate();
		}

		public Building Computer
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
					this.Computer.GetComp<CompComputerTerminal>().actionDef.ActivatedAction.TryAction(base.GetActor(), base.GetActor().Map, null);
				}
			};
			yield break;
		}
	}
}

