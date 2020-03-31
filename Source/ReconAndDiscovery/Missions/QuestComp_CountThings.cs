using System;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions
{
	public class QuestComp_CountThings : WorldObjectComp
	{
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
						int num = mapParent.Map.listerThings.ThingsOfDef(this.targetDef).Count<Thing>();
						if (num > this.targetNumber)
						{
							if (this.ticksHeld > this.ticksTarget)
							{
								this.StopQuest();
							}
							else
							{
								this.ticksHeld++;
							}
						}
						else
						{
							this.ticksHeld = 0;
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
			Scribe_Values.Look<int>(ref this.worldTileAffected, "worldTileAffected", 0, false);
			Scribe_Defs.Look<GameConditionDef>(ref this.gameConditionCaused, "gameConditionCaused");
			Scribe_Defs.Look<ThingDef>(ref this.targetDef, "targetDef");
			Scribe_Values.Look<int>(ref this.targetNumber, "targetNumber", 0, false);
			Scribe_Values.Look<int>(ref this.ticksHeld, "ticksHeld", 0, false);
			Scribe_Values.Look<int>(ref this.ticksTarget, "tickTarget", 0, false);
		}

		public void StartQuest(ThingDef targetDef)
		{
			this.targetDef = targetDef;
			this.active = true;
		}

		public void StopQuest()
		{
			this.active = false;
			MapParent mapParent = this.parent as MapParent;
			if (mapParent != null && mapParent.Map != null)
			{
				int num = mapParent.Map.listerThings.ThingsOfDef(this.targetDef).Count<Thing>();
				if (num > this.targetNumber && this.ticksHeld > this.ticksTarget)
				{
					if (mapParent.Map.gameConditionManager.ConditionIsActive(this.gameConditionCaused))
					{
						mapParent.Map.gameConditionManager.ActiveConditions.Remove(mapParent.Map.gameConditionManager.GetActiveCondition(this.gameConditionCaused));
					}
					Settlement settlement = Find.World.worldObjects.SettlementAt(this.worldTileAffected);
					if (settlement != null && settlement.HasMap)
					{
						GameConditionManager gameConditionManager = settlement.Map.gameConditionManager;
						if (gameConditionManager.ConditionIsActive(this.gameConditionCaused))
						{
							gameConditionManager.ActiveConditions.Remove(gameConditionManager.GetActiveCondition(this.gameConditionCaused));
						}
					}
				}
			}
		}

		public override void PostPostRemove()
		{
			this.StopQuest();
			base.PostPostRemove();
		}

		private bool active;

		public int worldTileAffected;

		public int targetNumber;

		public int ticksHeld;

		public int ticksTarget;

		public GameConditionDef gameConditionCaused;

		private ThingDef targetDef;
	}
}

