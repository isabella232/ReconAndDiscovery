using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace ReconAndDiscovery
{
	[StaticConstructorOnStartup]
	public class CompTeleporter : ThingComp
	{
		public bool ReadyToTransport
		{
			get
			{
				return this.fCharge >= 1f;
			}
		}

		public void ResetCharge()
		{
			this.fCharge = 0f;
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (this.ReadyToTransport)
			{
				yield return this.TeleportCommand();
			}
			yield break;
		}

		public Command TeleportCommand()
		{
			return new Command_Action
			{
				defaultLabel = "RD_Teleport".Translate(), //Teleport."
				defaultDesc = "RD_TeleportDesc".Translate(), //"Teleport a person or animal."
				icon = CompTeleporter.teleSym,
				action = delegate()
				{
					this.StartChoosingTarget();
				}
			};
		}

		public override void CompTick()
		{
			if (this.parent.GetComp<CompPowerTrader>().PowerOn)
			{
				this.fCharge += 0.0001f;
				if (this.fCharge > 1f)
				{
					this.fCharge = 1f;
				}
			}
			else
			{
				this.fCharge -= 0.0004f;
				if (this.fCharge < 0f)
				{
					this.fCharge = 0f;
				}
			}
		}

		private void StartChoosingTarget()
		{
			CameraJumper.TryJump(CameraJumper.GetWorldTarget(this.parent));
			Find.WorldSelector.ClearSelection();
			int tile = this.parent.Map.Tile;
			Find.WorldTargeter.BeginTargeting(new Func<GlobalTargetInfo, bool>(this.ChoseWorldTarget), true, CompTeleporter.TargeterMouseAttachment, true, null, null);
		}

		private bool ChoseWorldTarget(GlobalTargetInfo target)
		{
			bool result;
			if (!this.ReadyToTransport)
			{
				result = true;
			}
			else if (!target.IsValid)
			{
				Messages.Message("MessageTransportPodsDestinationIsInvalid".Translate(), MessageTypeDefOf.RejectInput);
				result = false;
			}
			else
			{
				MapParent mapParent = target.WorldObject as MapParent;
				if (mapParent != null && mapParent.HasMap)
				{
					Map myMap = this.parent.Map;
					Map map = mapParent.Map;
					Current.Game.CurrentMap = map;
					Targeter targeter = Find.Targeter;
					Action actionWhenFinished = delegate()
					{
						if (Find.Maps.Contains(myMap))
						{
							Current.Game.CurrentMap = myMap;
						}
					};
					TargetingParameters targetParams = new TargetingParameters
					{
						canTargetPawns = true,
						canTargetItems = false,
						canTargetSelf = false,
						canTargetLocations = false,
						canTargetBuildings = false,
						canTargetFires = false
					};
					targeter.BeginTargeting(targetParams, delegate(LocalTargetInfo x)
					{
						if (this.ReadyToTransport)
						{
							this.TryTransport(x.ToGlobalTargetInfo(map));
						}
					}, null, actionWhenFinished, CompTeleporter.TargeterMouseAttachment);
					result = true;
				}
				else
				{
					
					Messages.Message("RD_YouCannotLock".Translate(), MessageTypeDefOf.RejectInput); //"You cannot lock onto anything there."
					result = false;
				}
			}
			return result;
		}

		private static void AddAnaphylaxisIfAppropriate(Pawn pawn)
		{
			Rand.PushState();
			Rand.Seed = pawn.thingIDNumber * 724;
			float value = Rand.Value;
			Rand.PopState();
			if (value < 0.12f)
			{
				pawn.health.AddHediff(HediffDef.Named("RD_Anaphylaxis"), null, null);
			}
		}

		public void TryTransport(GlobalTargetInfo target)
		{
			Map map = this.parent.Map;
			IntVec3 position = this.parent.Position;
			Map map2 = target.Map;
			Pawn pawn = target.Thing as Pawn;
			IntVec3 position2 = pawn.Position;
			if (map2.roofGrid.Roofed(position2) && map2.roofGrid.RoofAt(position2) == RoofDefOf.RoofRockThick)
			{
				
				Messages.Message("RD_TeleporterLannotLockThickRock".Translate(), MessageTypeDefOf.RejectInput); //Teleporter cannot lock on through the thick rock overhead!
			}
			else
			{
				MoteMaker.ThrowMetaPuff(position2.ToVector3(), map2);
				MoteMaker.ThrowMetaPuff(position.ToVector3(), map);
				pawn.DeSpawn();
				Fire fire = (Fire)GenSpawn.Spawn(ThingDefOf.Fire, position2, map2);
				fire.fireSize = 1f;
				GenSpawn.Spawn(pawn, position, map, this.parent.Rotation, WipeMode.Vanish, false);
				CompTeleporter.AddAnaphylaxisIfAppropriate(pawn);
				this.fCharge = 0f;
			}
		}

		protected string SaveKey
		{
			get
			{
				return "transCharge";
			}
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<float>(ref this.fCharge, this.SaveKey, 0f, false);
		}

		public override string CompInspectStringExtra()
		{
			return "RD_Charge".Translate() + this.fCharge.ToStringPercent(); //"Charge: "
		}

		private float fCharge = 0f;

		private static readonly Texture2D TargeterMouseAttachment = ContentFinder<Texture2D>.Get("UI/TeleportMouseAttachment", true);

		private static readonly Texture2D teleSym = ContentFinder<Texture2D>.Get("UI/TeleportSymbol", true);
	}
}

