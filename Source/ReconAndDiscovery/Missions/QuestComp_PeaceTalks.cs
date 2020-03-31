using System;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions
{
    public class QuestComp_PeaceTalks : WorldObjectComp
    {
        public Pawn Negotiator
        {
            get
            {
                if (this.negotiator == null)
                {
                    MapParent mapParent = this.parent as MapParent;
                    if (mapParent != null && mapParent.Map != null)
                    {
                        this.negotiator = (from p in mapParent.Map.mapPawns.AllPawnsSpawned
                                           where p.Faction == this.requestingFaction
                                           select p).RandomElement<Pawn>();
                    }
                }
                return this.negotiator;
            }
            set
            {
                this.negotiator = value;
            }
        }

        public bool Active
        {
            get
            {
                return this.active;
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            try
            {
                if (this.active)
                {
                    MapParent mapParent = this.parent as MapParent;
                    if (mapParent != null && mapParent.Map != null)
                    {
                        if (this.Negotiator != null && this.Negotiator.Spawned)
                        {
                            this.Negotiator.mindState.wantsToTradeWithColony = true;
                            if (this.Negotiator.GetComp<CompNegotiator>() == null)
                            {
                                ThingComp thingComp = new CompNegotiator();
                                thingComp.parent = this.Negotiator;
                                this.Negotiator.AllComps.Add(thingComp);
                            }
                        }
                    }
                }
            }
            catch
            {
                this.StopQuest();
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref this.active, "active", false, false);
            Scribe_Values.Look<int>(ref this.facOriginalRelationship, "facOriginalRelationship", 0, false);
            Scribe_Values.Look<float>(ref this.relationsImprovement, "relationsImprovement", 0f, false);
            Scribe_References.Look<Faction>(ref this.requestingFaction, "requestingFaction", false);
            Scribe_References.Look<Pawn>(ref this.negotiator, "negotiator", false);
        }

        public void ResolveNegotiations(Pawn playerNegotiator, Pawn otherNegotiator)
        {
            if (Rand.Chance(0.95f))
            {
                QualityCategory qualityCategory = QualityUtility.GenerateQualityCreatedByPawn(playerNegotiator.skills.GetSkill(SkillDefOf.Social).Level, false);
                int num = 0;
                string text = "";
                switch (qualityCategory)
                {
                    case QualityCategory.Awful:
                        num = -5;
                        text = TranslatorFormattedStringExtensions.Translate("RD_DiplomacyAwful", playerNegotiator.Label, this.requestingFaction.Name, num); //"The flailing diplomatic "strategy" of {0} seemed chiefly to involve wild swings between aggression and panic, peppered liberally with lewd insults involving the negotiator for {1}'s antecedents. Your already strained relations have, understandably, worsened ({2} to relations).";
                        break;
                    case QualityCategory.Poor:
                        num = -1;
                        text = TranslatorFormattedStringExtensions.Translate("RD_DiplomacyPoor" //"The chief negotiation tactic employed by {0} seemed to be staring bored at the wall. This did little to diffuse tensions and engender a feeling of respect ({2} to relations))";
, playerNegotiator.Label, this.requestingFaction.Name, num);
                        break;
                    case QualityCategory.Normal:
                    case QualityCategory.Good:
                        num = 8;
                        text = TranslatorFormattedStringExtensions.Translate("RD_DiplomacyNormalGood" //"{0}'s negotiation adequately dealt with some minor disputes you have with {1}. Your relations have improved by {2}.";

    , playerNegotiator.Label, this.requestingFaction.Name, num);
                        break;
                    case QualityCategory.Excellent:
                        num = 16;
                        text = TranslatorFormattedStringExtensions.Translate("RD_DiplomacyExcellent" //"{0}'s easy, but unyielding manner dealt well with a number of the negotiator for {1}'s concerns. Your relations have improved by {2}";

    , playerNegotiator.Label, this.requestingFaction.Name, num);
                        break;
                    case QualityCategory.Masterwork:
                    case QualityCategory.Legendary:
                        num = 32;
                        text = TranslatorFormattedStringExtensions.Translate("RD_DiplomacyMasterWorkLegendary" //"{0} made diplomacy look as easy as breathing, with an almost magical ability to make {1}'s negotiator see your perspective. Your relations have undergone a substantial improvement of {2}.";

    , playerNegotiator.Label, this.requestingFaction.Name, num);
                        break;
                }
                DiaNode diaNode = new DiaNode(text);
                DiaOption diaOption = new DiaOption("OK".Translate());
                diaOption.resolveTree = true;
                diaNode.options.Add(diaOption);
                Dialog_NodeTree window = new Dialog_NodeTree(diaNode, false, false, null);
                Find.WindowStack.Add(window);
                this.facOriginalRelationship += num;
                this.active = false;
                ThingComp comp = this.Negotiator.GetComp<CompNegotiator>();
                if (comp != null)
                {
                    this.Negotiator.AllComps.Remove(comp);
                }
            }
            else
            {
                DiaNode diaNode2 = new DiaNode("RD_NegotiationsTrap".Translate()); //"The negotiations are a trap!"
                DiaOption diaOption2 = new DiaOption("OK".Translate());
                diaOption2.resolveTree = true;
                diaNode2.options.Add(diaOption2);
                Dialog_NodeTree window2 = new Dialog_NodeTree(diaNode2, false, false, null);
                Find.WindowStack.Add(window2);
                this.requestingFaction.TryAffectGoodwillWith(Faction.OfPlayer, -101);
            }
        }

        public void StartQuest(Faction faction)
        {
            this.active = true;
            this.requestingFaction = faction;
            this.facOriginalRelationship = (int)faction.PlayerGoodwill;
            this.requestingFaction.TryAffectGoodwillWith(Faction.OfPlayer, 1 - faction.PlayerGoodwill);
        }

        public void StopQuest()
        {
            this.active = false;
            float num = this.requestingFaction.PlayerGoodwill - (float)this.facOriginalRelationship;
            this.requestingFaction.TryAffectGoodwillWith(Faction.OfPlayer, this.facOriginalRelationship);
            this.requestingFaction = null;
        }

        public override void PostPostRemove()
        {
            this.StopQuest();
            base.PostPostRemove();
        }

        private bool active;

        public int facOriginalRelationship;

        public float relationsImprovement;

        public Faction requestingFaction;

        private Pawn negotiator;
    }
}

