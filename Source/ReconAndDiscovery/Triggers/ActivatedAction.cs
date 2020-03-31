using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Triggers
{
	public class ActivatedAction
	{
		public virtual bool TryAction(Pawn activatedBy, Map map, Thing trigger)
		{
			if (this.def.numThingsToSpawn.max > 0 && this.def.spawnedThings != null && this.def.spawnedThings.Count > 0)
			{
				this.SpawnThings(activatedBy, map, trigger);
			}
			if (this.def.numPawnsToSpawn.max > 0 && this.def.pawnKinds != null && this.def.pawnKinds.Count > 0)
			{
				this.SpawnPawns(activatedBy, map, trigger);
			}
			this.DoAnyFurtherActions(activatedBy, map, trigger);
			if (this.def.hasDialog)
			{
				this.DisplayDialog(activatedBy, map, trigger);
			}
			return true;
		}

		protected virtual void DisplayDialog(Pawn activatedBy, Map map, Thing trigger)
		{
			DiaNode diaNode = new DiaNode(this.def.dialogText);
			DiaOption diaOption = new DiaOption("OK".Translate());
			diaOption.resolveTree = true;
			diaNode.options.Add(diaOption);
			Find.WindowStack.Add(new Dialog_NodeTree(diaNode, false, false, null));
		}

		protected IEnumerable<IntVec3> GetEffectArea(IntVec3 centre)
		{
			return GenRadial.RadialCellsAround(centre, (float)this.def.effectRadius, true);
		}

		protected virtual void DoAnyFurtherActions(Pawn activatedBy, Map map, Thing trigger)
		{
		}

		private void SpawnThings(Pawn activatedBy, Map map, Thing trigger)
		{
			int randomInRange = this.def.numThingsToSpawn.RandomInRange;
			for (int i = 0; i < randomInRange; i++)
			{
				ThingDef thingDef = this.def.spawnedThings.RandomElement<ThingDef>();
				Thing thing = ThingMaker.MakeThing(thingDef, null);
				thing.stackCount = Math.Min(thingDef.stackLimit, this.def.spawnedThingStackSize.RandomInRange);
			}
		}

		private void SpawnPawns(Pawn activatedBy, Map map, Thing trigger)
		{
			int randomInRange = this.def.numPawnsToSpawn.RandomInRange;
			PawnKindDef pawnKindDef = this.def.pawnKinds.RandomElement<PawnKindDef>();
			for (int i = 0; i < randomInRange; i++)
			{
				Faction faction = null;
				if (!pawnKindDef.RaceProps.Animal)
				{
                    faction = Find.FactionManager.RandomNonHostileFaction(false, false, true, TechLevel.Spacer);
				}
				PawnGenerationRequest request = new PawnGenerationRequest(pawnKindDef, faction, PawnGenerationContext.NonPlayer, -1, false, false, false, false, true, false, 1f, false, true, true, false, false);
				Pawn pawn = PawnGenerator.GeneratePawn(request);
				if (!pawn.RaceProps.Humanlike)
				{
					pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter, null, true, false, null);
				}
				else if (pawn.Faction == null || !pawn.Faction.HostileTo(Faction.OfPlayer))
				{
					pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk, null, true, false, null);
				}
			}
		}

		public ActivatedActionDef def;
	}
}

