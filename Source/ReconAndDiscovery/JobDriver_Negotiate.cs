using System;
using System.Collections.Generic;
using ReconAndDiscovery.Missions;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery
{
	public class JobDriver_Negotiate : JobDriver
	{
		public override string GetReport()
		{
			return base.GetReport();
		}

		public Pawn OtherParty
		{
			get
			{
				return (Pawn)this.pawn.CurJob.GetTarget(TargetIndex.A).Thing;
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
					Find.WorldObjects.MapParentAt(this.OtherParty.Map.Tile).GetComponent<QuestComp_PeaceTalks>().ResolveNegotiations(base.GetActor(), this.OtherParty);
				}
			};
			yield break;
		}
	}
}

