using System;
using System.Collections.Generic;
using System.Linq;
using ReconAndDiscovery.Things;
using RimWorld;
using Verse;

namespace ReconAndDiscovery
{
	[StaticConstructorOnStartup]
	public class CompHoloEmitter : ThingComp
	{
		public Pawn SimPawn
		{
			get
			{
				return this.pawn;
			}
			set
			{
				this.pawn = value;
			}
		}

		public override void PostDestroy(DestroyMode mode, Map previousMap)
		{
			base.PostDestroy(mode, previousMap);
			if (this.pawn != null)
			{
                DamageInfo value = new DamageInfo(DamageDefOf.Blunt, 1000, -1f, -1f, null, null, null, 0);
				this.SimPawn.Kill(new DamageInfo?(value));
				this.SimPawn.Corpse.Destroy(DestroyMode.Vanish);
			}
		}

		public override void PostDeSpawn(Map map)
		{
			if (this.pawn != null && this.pawn.Spawned)
			{
				this.pawn.DeSpawn();
			}
			base.PostDeSpawn(map);
		}

		private HoloEmitter Emitter
		{
			get
			{
				return this.parent as HoloEmitter;
			}
		}

		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			if (this.pawn != null)
			{
                FloatMenuOption floatMenuOption = new FloatMenuOption("RD_FormatOccupant".Translate(), delegate()
				{
					DamageInfo value = new DamageInfo(DamageDefOf.ExecutionCut, 1000, -1f, -1f, selPawn, null, null, 0);
					this.pawn.Kill(new DamageInfo?(value));
					this.pawn.Corpse.Destroy(DestroyMode.Vanish);
					this.pawn = null;
				});
				if (selPawn != this.pawn)
				{
					list.Add(floatMenuOption);
				}
			}
			else if (selPawn.story.traits.HasTrait(TraitDef.Named("RD_Holographic")))
			{
				FloatMenuOption floatMenuOption2 = new FloatMenuOption("RD_TransferToThisEmitter".Translate(), delegate()
				{
					foreach (Thing thing in this.parent.Map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("RD_HolographicEmitter")))
					{
						HoloEmitter holoEmitter = thing as HoloEmitter;
						if (holoEmitter == null)
						{
							break;
						}
						if (holoEmitter.GetComp<CompHoloEmitter>().SimPawn == selPawn)
						{
							holoEmitter.GetComp<CompHoloEmitter>().SimPawn = null;
							this.pawn = selPawn;
							this.parent.Map.areaManager.AllAreas.Remove(this.pawn.playerSettings.AreaRestriction);
							this.MakeValidAllowedZone();
							break;
						}
					}
				});
				if (this.pawn == null)
				{
					list.Add(floatMenuOption2);
				}
			}
			return list;
		}

		public override void PostExposeData()
		{
			Scribe_References.Look<Pawn>(ref this.pawn, "simulatedPawn", false);
		}

		public void SetUpPawn()
		{
			if (this.pawn.Spawned)
			{
				this.pawn.DeSpawn();
			}
			this.pawn.health.Reset();
			this.pawn.story.traits.GainTrait(new Trait(TraitDef.Named("RD_Holographic"), 0, false));
			GenSpawn.Spawn(this.pawn, this.parent.Position, this.parent.Map);
			this.MakeValidAllowedZone();
		}

		private void HoloTickPawn()
		{
			if (this.pawn != null)
			{
				if (this.pawn.Dead)
				{
					Log.Message(string.Format("{0} is dead.", this.pawn.Label));
					if (this.pawn.Corpse.holdingOwner != this.Emitter.GetDirectlyHeldThings())
					{
						if (this.Emitter.TryAcceptThing(this.pawn.Corpse, true))
						{
						}
					}
				}
				else
				{
					if (!this.pawn.story.traits.HasTrait(TraitDef.Named("RD_Holographic")))
					{
						this.SetUpPawn();
					}
					if (!this.pawn.Spawned)
					{
						GenSpawn.Spawn(this.pawn, this.parent.Position, this.parent.Map);
					}
					this.pawn.needs.food.CurLevel = 1f;
					if (!this.pawn.Position.InHorDistOf(this.parent.Position, 12f) || !GenSight.LineOfSight(this.parent.Position, this.pawn.Position, this.parent.Map, true, null, 0, 0))
					{
						this.pawn.inventory.DropAllNearPawn(this.pawn.Position, false, false);
						this.pawn.equipment.DropAllEquipment(this.pawn.Position, false);
						this.pawn.DeSpawn();
						GenSpawn.Spawn(this.pawn, this.parent.Position, this.parent.Map);
					}
					this.pawn.health.Reset();
				}
			}
		}

		public void Scan(Corpse c)
		{
			if (this.Emitter.TryAcceptThing(c, true))
			{
				c.InnerPawn.story.traits.GainTrait(new Trait(TraitDef.Named("RD_Holographic"), 0, false));
			}
		}

		public void MakeValidAllowedZone()
		{
			IEnumerable<IntVec3> enumerable = from cell in GenRadial.RadialCellsAround(this.parent.Position, 18f, true)
			where cell.InHorDistOf(this.parent.Position, 12f) && GenSight.LineOfSight(this.parent.Position, cell, this.parent.Map, true, null, 0, 0)
			select cell;
			Area_Allowed area_Allowed;
			this.parent.Map.areaManager.TryMakeNewAllowed(out area_Allowed);
			foreach (IntVec3 c in enumerable)
			{
				area_Allowed[this.parent.Map.cellIndices.CellToIndex(c)] = true;
			}
			area_Allowed.SetLabel("RD_HoloEmitterAreaFor".Translate(this.pawn.Named("PAWN"))); //"HoloEmitter area for {0}."
			this.pawn.playerSettings.AreaRestriction = area_Allowed;
		}

		public override void CompTickRare()
		{
			base.CompTickRare();
			if (this.pawn != null)
			{
				foreach (Designation designation in this.parent.Map.designationManager.AllDesignationsOn(this.parent))
				{
					if (designation.def == DesignationDefOf.Uninstall)
					{
						if (this.pawn.Spawned)
						{
							this.pawn.DeSpawn();
						}
						return;
					}
				}
				if (this.parent.GetComp<CompPowerTrader>().PowerOn)
				{
					this.HoloTickPawn();
				}
				else if (this.pawn.Spawned)
				{
					this.pawn.DeSpawn();
				}
			}
		}

		private Pawn pawn;
	}
}

