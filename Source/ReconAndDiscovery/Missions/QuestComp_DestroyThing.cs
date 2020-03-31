using System;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions
{
	public class QuestComp_DestroyThing : WorldObjectComp
	{
		public Thing ThingToDestroy
		{
			get
			{
				if (this.thingToDestroy == null)
				{
					if (this.targetDef == null)
					{
						return null;
					}
					MapParent mapParent = this.parent as MapParent;
					if (mapParent != null && mapParent.HasMap)
					{
						this.thingToDestroy = mapParent.Map.listerThings.ThingsOfDef(this.targetDef).RandomElement<Thing>();
					}
				}
				return this.thingToDestroy;
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
						if (this.ThingToDestroy != null)
						{
							if (this.ThingToDestroy.Destroyed)
							{
								this.StopQuest();
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
			Scribe_Values.Look<int>(ref this.worldTileAffected, "worldTileAffected", 0, false);
			Scribe_Defs.Look<GameConditionDef>(ref this.gameConditionCaused, "gameConditionCaused");
			Scribe_Defs.Look<ThingDef>(ref this.targetDef, "targetDef");
			Scribe_References.Look<Thing>(ref this.thingToDestroy, "thingToDestroy", false);
		}

		public void StartQuest(ThingDef targetDef)
		{
			this.targetDef = targetDef;
			this.active = true;
		}

		public void StopQuest()
		{
			this.active = false;
			if (this.ThingToDestroy.Destroyed)
			{
				Settlement settlement = Find.World.worldObjects.SettlementAt(this.worldTileAffected);
				if (settlement != null && settlement.HasMap)
				{
					Log.Message(string.Format("Found player base named {0}", settlement.TraderName));
					GameConditionManager gameConditionManager = settlement.Map.gameConditionManager;
					if (gameConditionManager.ConditionIsActive(this.gameConditionCaused))
					{
						gameConditionManager.ActiveConditions.Remove(gameConditionManager.GetActiveCondition(this.gameConditionCaused));
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

		public GameConditionDef gameConditionCaused;

		private Thing thingToDestroy;

		private ThingDef targetDef;
	}
}

