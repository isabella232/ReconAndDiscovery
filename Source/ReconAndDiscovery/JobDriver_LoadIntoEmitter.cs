using System;
using System.Collections.Generic;
using System.Linq;
using ReconAndDiscovery.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery
{
	public class JobDriver_LoadIntoEmitter : JobDriver
	{
		public JobDriver_LoadIntoEmitter()
		{
			this.rotateToFace = TargetIndex.B;
		}

		private Pawn MakeGeniusPawn()
		{
            PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDefOf.SpaceRefugee, Faction.OfPlayer, 
                PawnGenerationContext.PlayerStarter, -1, true, false, false, false, false, false, 0f, 
                true, true, true, false, false);
			Pawn pawn = PawnGenerator.GeneratePawn(request);
			List<Trait> list = new List<Trait>();
			foreach (Trait trait in pawn.story.traits.allTraits)
			{
				if (trait.def == TraitDefOf.Psychopath || trait.def == TraitDefOf.Cannibal || trait.def == TraitDefOf.Pyromaniac)
				{
					list.Add(trait);
				}
			}
			foreach (Trait item in list)
			{
				pawn.story.traits.allTraits.Remove(item);
			}
			List<SkillRecord> list2 = (from s in pawn.skills.skills
			where !s.TotallyDisabled
			select s).ToList<SkillRecord>();
			SkillRecord skillRecord = list2.RandomElement<SkillRecord>();
			skillRecord.Level = 20;
			skillRecord.passion = Passion.Major;
			list2.Remove(skillRecord);
			skillRecord = list2.RandomElement<SkillRecord>();
			skillRecord.Level = 20;
			skillRecord.passion = Passion.Major;
			return pawn;
		}

		private Thing Disk
		{
			get
			{
				return this.pawn.CurJob.GetTarget(TargetIndex.A).Thing;
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
					Pawn simPawn = this.MakeGeniusPawn();
					this.Emitter.GetComp<CompHoloEmitter>().SimPawn = simPawn;
					this.Emitter.GetComp<CompHoloEmitter>().SetUpPawn();
					this.Disk.Destroy(DestroyMode.Vanish);
				}
			};
			yield return Toils_Reserve.Release(TargetIndex.B);
			yield break;
		}

		private const TargetIndex CorpseIndex = TargetIndex.A;

		private const TargetIndex GraveIndex = TargetIndex.B;
	}
}

