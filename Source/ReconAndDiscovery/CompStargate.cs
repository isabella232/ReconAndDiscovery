using System;
using System.Collections.Generic;
using System.Linq;
using ReconAndDiscovery.Maps;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery
{
	[StaticConstructorOnStartup]
	public class CompStargate : ThingComp
	{
		public Thing LinkedGate
		{
			get
			{
				Thing result;
				if (this.link == null || !this.link.Spawned || this.link.Destroyed)
				{
					result = null;
				}
				else
				{
					result = this.link;
				}
				return result;
			}
		}

		public Site LinkedSite
		{
			get
			{
				Site result;
				if (this.linkedSite == null || this.linkedSite.Tile == -1 || !Find.WorldObjects.AnyMapParentAt(this.linkedSite.Tile))
				{
					result = null;
				}
				else
				{
					result = this.linkedSite;
				}
				return result;
			}
		}

		public override void PostDestroy(DestroyMode mode, Map previousMap)
		{
			base.PostDestroy(mode, previousMap);
			if (mode == DestroyMode.Deconstruct)
			{
				GenSpawn.Spawn(ThingDef.Named("RD_ExoticMatter"), this.parent.Position, previousMap);
			}
		}

		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			if ((this.link != null && !this.link.Destroyed && this.link.Spawned) || this.linkedSite != null)
			{
				list.Add(new FloatMenuOption("RD_TravelToTargetGate".Translate(), delegate()
					{
						Job job = new Job(JobDefOfReconAndDiscovery.RD_TravelThroughStargate, this.parent);
						job.playerForced = true;
						selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
					}
				));
			}
			return list;
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			yield return this.LinkCommand();
			yield break;
		}

		public Command LinkCommand()
		{
			return new Command_Action
			{
				defaultLabel = "RD_LinkGate".Translate(), //"Link a gate."
				defaultDesc = "RD_LinkGateDesc".Translate(), //"Link this gate to another."
				icon = CompStargate.teleSym,
				action = delegate()
				{
					this.StartChoosingTarget();
				}
			};
		}

		private void StartChoosingTarget()
		{
			CameraJumper.TryJump(CameraJumper.GetWorldTarget(this.parent));
			Find.WorldSelector.ClearSelection();
			int tile = this.parent.Map.Tile;
			Find.WorldTargeter.BeginTargeting(new Func<GlobalTargetInfo, bool>(this.ChoseWorldTarget), true, null, true, null, null);
		}

		private bool ChoseWorldTarget(GlobalTargetInfo target)
		{
			bool result;
			if (!target.IsValid)
			{
				Messages.Message("MessageTransportPodsDestinationIsInvalid".Translate(), MessageTypeDefOf.RejectInput);
				result = false;
			}
			else
			{
				MapParent mapParent = target.WorldObject as MapParent;
				if (mapParent != null)
				{
					if (mapParent.HasMap)
					{
						Map map = this.parent.Map;
						Map map2 = mapParent.Map;
						Current.Game.CurrentMap = map2;
						if (map2.listerThings.ThingsOfDef(ThingDef.Named("RD_Stargate")).Count<Thing>() > 0)
						{
							this.MakeLink(map2.listerThings.ThingsOfDef(ThingDef.Named("RD_Stargate")).FirstOrDefault<Thing>());
							result = true;
						}
						else
						{
							Messages.Message("RD_StargateNoEvidence".Translate(), MessageTypeDefOf.RejectInput); // "There is no evidence of a stargate there."
							result = false;
						}
					}
					else
					{
						Site site = mapParent as Site;
						if (site != null && site.parts.Select(x => x.def).Contains(SiteDefOfReconAndDiscovery.RD_Stargate))
						{
							this.MakeLink(site);
							result = true;
						}
						else
						{
							Messages.Message("RD_StargateNoEvidence".Translate(), MessageTypeDefOf.RejectInput);
							result = false;
						}
					}
				}
				else
				{
					Messages.Message("RD_StargateNoEvidence".Translate(), MessageTypeDefOf.RejectInput);
					result = false;
				}
			}
			return result;
		}

		public void MakeLink(Thing stargate)
		{
			this.link = null;
			this.linkedSite = null;
			this.link = stargate;
			Messages.Message("RD_StargateLinked".Translate(), MessageTypeDefOf.PositiveEvent); // "Stargate linked to destination."
		}

		public void MakeLink(Site stargateSite)
		{
			this.link = null;
			this.linkedSite = null;
			this.linkedSite = stargateSite;
			Messages.Message("RD_StargateLinked".Translate(), MessageTypeDefOf.PositiveEvent);
		}

		public override void PostExposeData()
		{
			Scribe_References.Look<Thing>(ref this.link, "link", false);
			Scribe_References.Look<Site>(ref this.linkedSite, "linkedSite", false);
		}

		public bool CheckSetupGateAndMap(Pawn p)
		{
			Log.Message(string.Format("Checking for link", new object[0]));
			bool drafted = p.Drafted;
			bool flag = Find.Selector.IsSelected(p);
			bool result;
			if (this.LinkedGate == null)
			{
				if (this.LinkedSite == null)
				{
					Messages.Message("RD_StargateNotLinked".Translate(), MessageTypeDefOf.RejectInput); //"Stargate is not linked to a destination!"
					result = false;
				}
				else if (this.LinkedSite.HasMap)
				{
					IEnumerable<Thing> source = this.LinkedSite.Map.listerThings.ThingsOfDef(ThingDef.Named("RD_Stargate"));
					if (source.Count<Thing>() == 0)
					{
						Messages.Message("RD_StargateNotLinked".Translate(), MessageTypeDefOf.RejectInput);
						result = false;
					}
					else
					{
						this.MakeLink(source.FirstOrDefault<Thing>());
						Log.Message(string.Format("Linked extant, unlinked gate!", new object[0]));
						result = true;
					}
				}
				else
				{
					List<Pawn> pawns = new List<Pawn>();
					pawns.Add(p);
					LongEventHandler.QueueLongEvent(delegate()
					{
						SitePartWorker_Stargate.tmpPawnsToSpawn.AddRange(pawns);
						Map orGenerateMap = GetOrGenerateMapUtility.GetOrGenerateMap(this.LinkedSite.Tile, this.LinkedSite.Map.Size, null);
					}, "GeneratingMapForNewEncounter", false, null);
					result = false;
				}
			}
			else
			{
				Log.Message(string.Format("Linked already!", new object[0]));
				result = true;
			}
			return result;
		}

		public void SendPawnThroughStargate(Pawn p)
		{
			bool drafted = p.Drafted;
			bool flag = Find.Selector.IsSelected(p);
			Log.Message(string.Format("Attempting to send {0} through gate", p.Label));
			if (this.CheckSetupGateAndMap(p))
			{
				if (this.LinkedGate == null || !this.LinkedGate.Spawned || this.LinkedGate.Destroyed)
				{
					Messages.Message("RD_OtherGateBuried".Translate(), MessageTypeDefOf.RejectInput); //"The other gate has been buried! We cannot transit!"
				}
				else
				{
					if (p.Spawned)
					{
						p.DeSpawn();
					}
					GenSpawn.Spawn(p, this.LinkedGate.Position, this.LinkedGate.Map);
					if (drafted)
					{
						p.drafter.Drafted = true;
					}
					if (flag)
					{
						Current.Game.CurrentMap = p.Map;
						Find.CameraDriver.JumpToCurrentMapLoc(this.LinkedGate.Position);
					}
				}
			}
		}

		private Thing link;

		private Site linkedSite;

		private static readonly Texture2D teleSym = ContentFinder<Texture2D>.Get("UI/StargateSymbol", true);
	}
}

