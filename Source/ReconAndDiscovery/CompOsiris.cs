using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ReconAndDiscovery.Things;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace ReconAndDiscovery
{
    [StaticConstructorOnStartup]
    public class CompOsiris : ThingComp
    {
        public Building_Casket Casket
        {
            get
            {
                return this.parent as Building_Casket;
            }
        }

        private bool ReadyToHeal
        {
            get
            {
                return this.Casket.ContainedThing is Pawn || (this.Casket.ContainedThing is Corpse && !(this.Casket.ContainedThing as Corpse).IsNotFresh() && this.parent.GetComp<CompPowerTrader>().PowerOn && this.parent.GetComp<CompRefuelable>().Fuel >= 50f);
            }
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            if (mode == DestroyMode.Deconstruct)
            {
                GenSpawn.Spawn(ThingDef.Named("RD_OsirisAI"), this.parent.Position, previousMap);
            }
        }

        public static void Ressurrect(Pawn pawn, Thing thing)
        {
            if (thing is HoloEmitter)
            {
                if (pawn.Corpse.holdingOwner != null)
                {
                    pawn.Corpse.GetDirectlyHeldThings().TryTransferToContainer(pawn, pawn.Corpse.holdingOwner, true);
                }
                else if (pawn.Corpse.Spawned)
                {
                    ResurrectionUtility.Resurrect(pawn);
                    PawnDiedOrDownedThoughtsUtility.RemoveDiedThoughts(pawn);
                    PawnComponentsUtility.AddAndRemoveDynamicComponents(pawn, false);
                    CompOsiris.FixPawnRelationships(pawn);
                    pawn.health.Reset();
                    if (pawn.Corpse != null && pawn.Corpse.Spawned)
                    {
                        pawn.Corpse.DeSpawn();
                    }
                    GenSpawn.Spawn(pawn, pawn.Corpse.Position, pawn.Corpse.Map);
                    GiveSideEffects(pawn);
                }
                if (pawn.Corpse != null)
                {
                    pawn.Corpse.Destroy(DestroyMode.Vanish);
                }
            }
            else
            {
                ResurrectionUtility.Resurrect(pawn);
                PawnDiedOrDownedThoughtsUtility.RemoveDiedThoughts(pawn);
                CompOsiris.FixPawnRelationships(pawn);
                pawn.health.Reset();
                if (pawn.Corpse != null && pawn.Corpse.Spawned)
                {
                    pawn.Corpse.DeSpawn();
                }
                GenSpawn.Spawn(pawn, thing.Position, thing.Map);
                GiveSideEffects(pawn);

                Building_Casket building_Casket = thing as Building_Casket;
                if (building_Casket != null)
                {
                    building_Casket.GetDirectlyHeldThings().Clear();
                }
            }
        }

        public void HealContained()
        {
            if (this.Casket.ContainedThing != null)
            {
                Corpse corpse = this.Casket.ContainedThing as Corpse;
                Pawn pawn;
                if (corpse != null)
                {
                    pawn = corpse.InnerPawn;
                }
                else
                {
                    pawn = (this.Casket.ContainedThing as Pawn);
                }
                if (pawn != null)
                {
                    if (pawn.Dead)
                    {
                        CompOsiris.Ressurrect(pawn, this.parent);
                    }
                    else
                    {
                        pawn.health.Reset();
                    }
                    if (pawn.RaceProps.Humanlike)
                    {
                        pawn.ageTracker.AgeBiologicalTicks = 90000000L;
                        if (Rand.Value < 0.65f)
                        {
                            pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("RD_ReturnedFromTheDeadBad"), null);
                        }
                        else
                        {
                            pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("RD_ReturnedFromTheDeadGood"), null);
                        }
                    }
                    else if (pawn.RaceProps.Animal)
                    {
                        pawn.ageTracker.AgeBiologicalTicks = (long)(pawn.RaceProps.lifeStageAges[2].minAge * 3600000f);
                    }
                    pawn.health.AddHediff(HediffDef.Named("LuciferiumAddiction"), null, null);
                    pawn.health.AddHediff(HediffDef.Named("LuciferiumHigh"), null, null);
                }
            }
        }

        private static void FixPawnRelationships(Pawn p)
        {
            foreach (Pawn pawn in PawnsFinder.AllCaravansAndTravelingTransportPods_Alive)
            {
                if (pawn != p)
                {
                    if (pawn != null && pawn.needs != null && pawn.needs.mood != null && pawn.needs.mood.thoughts != null && pawn.needs.mood.thoughts.memories != null)
                    {
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.KnowColonistDied, p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.PawnWithBadOpinionDied, p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.PawnWithGoodOpinionDied, p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("BondedAnimalDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MySonDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyDaughterDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyHusbandDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyWifeDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyFianceDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyFianceeDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyLoverDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyBrotherDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MySisterDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyGrandchildDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyFatherDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyMotherDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyNieceDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyNephewDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyHalfSiblingDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyAuntDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyUncleDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyGrandparentDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyCousinDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyKinDied"), p);
                    }
                }
            }
        }

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            FloatMenuOption floatMenuOption = new FloatMenuOption("RD_ResurrectContained".Translate(), delegate ()
            {
                Job job = new Job(JobDefOfReconAndDiscovery.RD_ActivateOsirisCasket, this.parent);
                job.playerForced = true;
                selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }); ;
            if (this.ReadyToHeal)
            {
                list.Add(floatMenuOption);
            }
            return list;
        }

        public static bool TryRandomlyMissingColonist(out Pawn pawn)
        {
            bool result = false;
            pawn = GenCollection.RandomElement<Pawn>(Enumerable.Where<Pawn>(Find.WorldPawns.AllPawnsDead, (Pawn x) => x.Faction == Faction.OfPlayer && x.Corpse == null));
            if (pawn != null)
            {
                result = true;
            }
            return result;
        }

        public static void GiveSideEffects(Pawn pawn)
        {
            BodyPartRecord brain = pawn.health.hediffSet.GetBrain();
            Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.ResurrectionSickness, pawn, null);
            if (!pawn.health.WouldDieAfterAddingHediff(hediff))
            {
                pawn.health.AddHediff(hediff, null, null, null);
            }
            if (Rand.Chance(0.8f) && brain != null)
            {
                Hediff hediff2 = HediffMaker.MakeHediff(HediffDefOf.Dementia, pawn, brain);
                if (!pawn.health.WouldDieAfterAddingHediff(hediff2))
                {
                    pawn.health.AddHediff(hediff2, null, null, null);
                }
            }
            if (Rand.Chance(0.8f))
            {
                foreach (BodyPartRecord bodyPartRecord in Enumerable.Where<BodyPartRecord>(pawn.health.hediffSet.GetNotMissingParts(0, 0, null, null), (BodyPartRecord x) => x.def == BodyPartDefOf.Eye))
                {
                    Hediff hediff3 = HediffMaker.MakeHediff(HediffDefOf.Blindness, pawn, bodyPartRecord);
                    pawn.health.AddHediff(hediff3, null, null, null);
                }
            }
            if (brain != null && Rand.Chance(0.8f))
            {
                Hediff hediff4 = HediffMaker.MakeHediff(HediffDefOf.ResurrectionPsychosis, pawn, brain);
                if (!pawn.health.WouldDieAfterAddingHediff(hediff4))
                {
                    pawn.health.AddHediff(hediff4, null, null, null);
                }
            }
            if (pawn.Dead)
            {
                Log.Error("The pawn has died while being resurrected.", false);
                ResurrectionUtility.Resurrect(pawn);
            }
        }
    }
}
