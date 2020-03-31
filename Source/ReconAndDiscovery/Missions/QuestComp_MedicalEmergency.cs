using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI.Group;

namespace ReconAndDiscovery.Missions
{
	public class QuestComp_MedicalEmergency : WorldObjectComp, IThingHolder
	{
		public QuestComp_MedicalEmergency()
		{
			this.rewards = new ThingOwner<Thing>(this);
		}

		public bool Active
		{
			get
			{
				return this.active;
			}
		}

		private bool CheckAllStanding()
		{
			foreach (Pawn pawn in this.injured)
			{
				if (!pawn.Dead)
				{
					if (pawn.Downed)
					{
						return false;
					}
					if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Moving))
					{
						return false;
					}
				}
			}
			return true;
		}

		public override string CompInspectStringExtra()
		{
			return null;
		}

		private void CalculateQuestOutcome()
		{
			int num = (from p in this.injured
			where !p.Dead && p.RaceProps.Humanlike
			select p).Count<Pawn>();
			bool giveTech = false;
			if (num - this.maxPawns == 0)
			{
				if ((double)Rand.Value < 0.1 * (double)num)
				{
					giveTech = true;
				}
			}
			int num2 = (from p in this.injured
			where !p.Dead && p.RaceProps.Humanlike && p.Faction != Faction.OfPlayer
			select p).Count<Pawn>();
			bool newFaction = num > 4 && Rand.Value < 0.6f;
			if (num > 0)
			{
				this.GiveRewardsAndSendLetter(giveTech, newFaction);
			}
			this.StopQuest();
		}

		public override void CompTick()
		{
			base.CompTick();
			if (this.active)
			{
				MapParent mapParent = this.parent as MapParent;
				if (mapParent != null && mapParent.Map != null)
				{
					if (this.injured.NullOrEmpty<Pawn>())
					{
						this.injured = (from p in mapParent.Map.mapPawns.AllPawns
						where p.Faction == QuestComp_MedicalEmergency.fac && p.RaceProps.Humanlike
						select p).ToList<Pawn>();
						Log.Message(string.Format("Found {0} injured pawns", this.injured.Count));
					}
					else
					{
						Log.Message(string.Format("Active with {0} in list and max of {1}.", this.injured.Count, this.maxPawns));
						foreach (Pawn pawn in this.injured)
						{
							if (!pawn.Dead && !pawn.Downed && pawn.GetLord() == null)
							{
								LordJob_DefendBase lordJob = new LordJob_DefendBase(QuestComp_MedicalEmergency.fac, pawn.Position);
								List<Pawn> list = new List<Pawn>();
								list.Add(pawn);
								LordMaker.MakeNewLord(QuestComp_MedicalEmergency.fac, lordJob, mapParent.Map, list);
							}
						}
						if (this.CheckAllStanding())
						{
							this.CalculateQuestOutcome();
						}
					}
				}
			}
		}

		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
		}

		public ThingOwner GetDirectlyHeldThings()
		{
			return this.rewards;
		}

		private ThingDef RandomHiTechReward()
		{
			float value = Rand.Value;
			ThingDef result;
			if (value <= 0f)
			{
				result = ThingDef.Named("Gun_ChargeRifle");
			}
			else if (value <= 0.25f)
			{
				result = ThingDef.Named("RD_HolographicEmitter");
			}
			else if (value <= 0.50f)
			{
				result = ThingDef.Named("RD_Teleporter");
			}
			else if (value <= 0.75f) 
			{
				result = ThingDef.Named("RD_GattlingLaser");
			}
			else
			{
				result = ThingDef.Named("Gun_ChargeRifle");
			}
			return result;
		}

		private void CloseMapImmediate()
		{
			MapParent mapParent = this.parent as MapParent;
			if (mapParent != null)
			{
				if (Dialog_FormCaravan.AllSendablePawns(mapParent.Map, true).Any((Pawn x) => x.IsColonist || x.IsPrisonerOfColony || x.Faction == Faction.OfPlayer || x.HostFaction == Faction.OfPlayer))
				{
					foreach (Pawn pawn in mapParent.Map.mapPawns.AllPawnsSpawned)
					{
						if (pawn.RaceProps.Humanlike)
						{
							Lord lord = pawn.GetLord();
							if (lord != null)
							{
								lord.Notify_PawnLost(pawn, PawnLostCondition.ExitedMap);
								pawn.ClearMind(false);
							}
						}
					}
					Messages.Message("MessageYouHaveToReformCaravanNow".Translate(), new GlobalTargetInfo(mapParent.Tile), MessageTypeDefOf.NeutralEvent);
					Current.Game.CurrentMap = mapParent.Map;
					Dialog_FormCaravan window = new Dialog_FormCaravan(mapParent.Map, true, delegate()
					{
						if (mapParent.HasMap)
						{
							Find.WorldObjects.Remove(mapParent);
						}
					}, false);
					List<Pawn> list = mapParent.Map.mapPawns.AllPawnsSpawned.ToList<Pawn>();
					for (int i = 0; i < list.Count; i++)
					{
						Pawn pawn2 = list[i];
						if (!pawn2.HostileTo(Faction.OfPlayer) && (pawn2.Faction == Faction.OfPlayer || pawn2.IsPrisonerOfColony))
						{
							Log.Message(pawn2.Label + " Meets criteria in CaravanUtility.");
						}
						else
						{
							Log.Message(pawn2.Label + " NOT ALLOWED by in CaravanUtility.");
						}
					}
					Find.WindowStack.Add(window);
				}
				else
				{
					List<Pawn> list2 = new List<Pawn>();
					list2.AddRange(from x in mapParent.Map.mapPawns.AllPawns
					where x.IsColonist || x.IsPrisonerOfColony || x.Faction == Faction.OfPlayer || x.HostFaction == Faction.OfPlayer
					select x);
					if (list2.Any<Pawn>())
					{
						if (list2.Any((Pawn x) => CaravanUtility.IsOwner(x, Faction.OfPlayer)))
						{
                            //TODO: check if it works
                            CaravanExitMapUtility.ExitMapAndCreateCaravan(list2, Faction.OfPlayer,
                                mapParent.Tile, mapParent.Tile, mapParent.Tile, false);
							Messages.Message("MessageReformedCaravan".Translate(),
                                MessageTypeDefOf.PositiveEvent);
						}
						else
						{
							StringBuilder stringBuilder = new StringBuilder();
							for (int j = 0; j < list2.Count; j++)
							{
								stringBuilder.AppendLine("    " + list2[j].LabelCap);
							}
                            Find.LetterStack.ReceiveLetter("RD_LetterLabelPawnsLostDueToMapCountdown".Translate(),
                            TranslatorFormattedStringExtensions.Translate("RD_LetterPawnsLostDueToMapCountdown", 
                            new NamedArgument[]
                            {
                                stringBuilder.ToString().TrimEndNewlines()
                            }), LetterDefOf.ThreatSmall, new GlobalTargetInfo(mapParent.Tile), null);
						}
						list2.Clear();
					}
					Find.WorldObjects.Remove(mapParent);
				}
			}
		}

		private void GiveRewardsAndSendLetter(bool giveTech, bool newFaction)
		{
			string text = "RD_GratefulSurvivors".Translate();
			string text2 = "RD_GratefulSurvivorsDesc".Translate(); //"The survivors of the crash are very thankful for your help, and have send some supplies as a gesture of gratitude.";
			if (giveTech)
			{
				ThingDef.Named("Gun_ChargeRifle");
				ThingDef thingDef = this.RandomHiTechReward();
				ThingDef stuff = null;
				if (thingDef.MadeFromStuff)
				{
					stuff = GenStuff.DefaultStuffFor(thingDef);
				}
				this.rewards.TryAdd(ThingMaker.MakeThing(thingDef, stuff), true);
				text2 = "RD_GratefulSurvivorsAmazedDesc".Translate(); //"The survivors of the crash are amazed by your rapid and professional emergency medical response, thanks to which no-one died. In gratitude, they have included a special system removed form the wreck.";
			}
			Find.LetterStack.ReceiveLetter(text, text2, LetterDefOf.PositiveEvent, null);
			Map map = Find.AnyPlayerHomeMap ?? ((MapParent)this.parent).Map;
			QuestComp_MedicalEmergency.tmpRewards.AddRange(this.rewards);
			this.rewards.Clear();
			IntVec3 dropCenter = DropCellFinder.TradeDropSpot(map);
			DropPodUtility.DropThingsNear(dropCenter, map, QuestComp_MedicalEmergency.tmpRewards, 110, false, false, true);
			QuestComp_MedicalEmergency.tmpRewards.Clear();
			if (newFaction)
			{
				int tile = this.parent.Tile;
				this.CloseMapImmediate();

				Faction faction = FactionGenerator.NewGeneratedFaction(FactionDefOf.Ancients);
                map.pawnDestinationReservationManager.GetPawnDestinationSetFor(faction);
				Find.FactionManager.Add(faction);
				Settlement settlement = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
                settlement.SetFaction(faction);
                settlement.Tile = tile;
                settlement.Name = SettlementNameGenerator.GenerateSettlementName(settlement, null);
				Find.WorldObjects.Add(settlement);
				faction.leader = null;
				foreach (Pawn pawn in this.injured)
				{
					if (!pawn.Dead && pawn.Faction != Faction.OfPlayer)
					{
						pawn.SetFactionDirect(faction);
						if (faction.leader == null)
						{
							faction.leader = pawn;
						}
					}
				}
				faction.TryAffectGoodwillWith(Faction.OfPlayer, 100);
				string text3 = "RD_NewFaction".Translate(); //New Faction!
				string text4 = "RD_NewFactionDesc".Translate() + faction.Name; //"The survivors of the crash have decided to make a life for themselves here, and have founded a new faction"
				Find.LetterStack.ReceiveLetter(text3, text4, LetterDefOf.PositiveEvent, null);
			}
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Collections.Look<Pawn>(ref this.injured, true, "injured", LookMode.Reference, new object[0]);
			Scribe_Values.Look<bool>(ref this.active, "active", false, false);
			Scribe_Values.Look<int>(ref this.maxPawns, "maxPawns", 0, false);
			Scribe_Values.Look<float>(ref this.relationsImprovement, "relationsImprovement", 0f, false);
			Scribe_References.Look<Faction>(ref this.requestingFaction, "requestingFaction", false);
			Scribe_Deep.Look<ThingOwner>(ref this.rewards, "rewards", new object[]
			{
				this
			});
		}

		public override void PostPostRemove()
		{
			base.PostPostRemove();
			this.rewards.ClearAndDestroyContents(DestroyMode.Vanish);
		}

		public void StartQuest(List<Thing> rewards)
		{
			this.active = true;
			this.rewards.ClearAndDestroyContents(DestroyMode.Vanish);
			this.rewards.TryAddRangeOrTransfer(rewards, true);
		}

		public void StopQuest()
		{
			this.active = false;
			this.requestingFaction = null;
			this.rewards.ClearAndDestroyContents(DestroyMode.Vanish);
		}

		private static List<Thing> tmpRewards = new List<Thing>();

		private static Faction fac = Find.FactionManager.RandomNonHostileFaction(false, false, true, TechLevel.Spacer);

		private List<Pawn> injured = new List<Pawn>();

		private bool active;

		public int maxPawns;

		public ThingOwner rewards;

		public float relationsImprovement;

		public Faction requestingFaction;
	}
}

