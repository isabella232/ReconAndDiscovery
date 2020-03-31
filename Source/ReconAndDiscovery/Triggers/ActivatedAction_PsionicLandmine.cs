using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Triggers
{
	public class ActivatedAction_PsionicLandmine : ActivatedAction
	{
		protected override void DisplayDialog(Pawn activatedBy, Map map, Thing trigger)
		{
			this.bestPsychic = PawnTalentUtility.FindBestPsychic(base.GetEffectArea(activatedBy.Position), map);
            DiaNode diaNode = new DiaNode("RD_PsionicLandmine".Translate()); //A psionic landmine in the room generates a short-range psychic shock!"); //"A psionic landmine in the room generates a short-range psychic shock!"
			if (this.bestPsychic != null)
			{
				if (this.bestPsychic.story.traits.DegreeOfTrait(TraitDef.Named("PsychicSensitivity")) == 2)
				{
					DiaNode diaNode2 = diaNode;
					diaNode2.text += "RD_FortunatelyDissipateAttack".Translate(this.bestPsychic.Named("PAWN")); //" Fortunately, {0} was able to dissipate the attack with their psychic capabilities.
				}
				else
				{
					DiaNode diaNode3 = diaNode;
					diaNode3.text += "RD_FortunatelyDissipateAttackShock".Translate( this.bestPsychic.Named("PAWN")); //" {0} was able to channel the attack, preventing harm to others, but is now in psychic shock!
				}
			}
			DiaOption diaOption = new DiaOption("OK".Translate());
			diaOption.resolveTree = true;
			diaNode.options.Add(diaOption);
			Find.WindowStack.Add(new Dialog_NodeTree(diaNode, true, true, null));
		}

		protected override void DoAnyFurtherActions(Pawn activatedBy, Map map, Thing trigger)
		{
			this.bestPsychic = PawnTalentUtility.FindBestPsychic(base.GetEffectArea(activatedBy.Position), map);
			if (this.bestPsychic != null)
			{
				if (this.bestPsychic.story.traits.DegreeOfTrait(TraitDef.Named("PsychicSensitivity")) != 2)
				{
					Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.PsychicShock, this.bestPsychic, null);
					this.bestPsychic.health.AddHediff(hediff, null, null);
				}
			}
			else
			{
				List<Pawn> list = new List<Pawn>();
				foreach (IntVec3 c in base.GetEffectArea(activatedBy.Position))
				{
					foreach (Thing thing in c.GetThingList(map))
					{
						if (thing.def.category == ThingCategory.Pawn && thing.def.race.intelligence == Intelligence.Humanlike)
						{
							Pawn pawn = thing as Pawn;
							int num = pawn.story.traits.DegreeOfTrait(TraitDef.Named("PsychicSensitivity"));
							if (num >= 0)
							{
								list.Add(pawn);
							}
						}
					}
				}
				foreach (Pawn pawn2 in list)
				{
					float value = Rand.Value;
					if ((double)value < 0.25)
					{
						pawn2.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk, null, true, false, null);
					}
					else if ((double)value < 0.85)
					{
						Hediff hediff2 = HediffMaker.MakeHediff(HediffDefOf.PsychicShock, pawn2, null);
						pawn2.health.AddHediff(hediff2, null, null);
					}
				}
			}
			ActionTrigger actionTrigger = trigger as ActionTrigger;
			if (actionTrigger != null)
			{
				IEnumerable<IntVec3> cells = actionTrigger.Cells;
				IntVec3 center = cells.RandomElement<IntVec3>();
                //TODO: check if it works
                GenExplosion.DoExplosion(center, map, 2f, DamageDefOf.Flame, null, -1, -1f, null, null, null, null, ThingDefOf.ChunkSlagSteel, 0.4f, 1, true, null, 0f, 1);
			}
		}

		private Pawn bestPsychic = null;
	}
}

