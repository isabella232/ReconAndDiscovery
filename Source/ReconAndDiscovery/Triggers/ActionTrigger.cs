using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Triggers
{
	public class ActionTrigger : Thing
	{
		public virtual ICollection<IntVec3> Cells
		{
			get
			{
				return this.cells;
			}
		}

		protected void ActivatedBy(Pawn p)
		{
			if (this.actionDef.ActivatedAction.TryAction(p, base.Map, this))
			{
				if (!base.Destroyed)
				{
					this.Destroy(DestroyMode.Vanish);
				}
			}
		}

		public override void ExposeData()
		{
			Scribe_Defs.Look<ActivatedActionDef>(ref this.actionDef, "actionDef");
			base.ExposeData();
		}

		public override void Tick()
		{
			Pawn pawn = null;
			if (this.IsHashIntervalTick(20))
			{
				Map map = base.Map;
				foreach (IntVec3 c in this.Cells)
				{
					List<Thing> thingList = c.GetThingList(map);
					for (int i = 0; i < thingList.Count; i++)
					{
						if (thingList[i].def.category == ThingCategory.Pawn && thingList[i].def.race.intelligence == Intelligence.Humanlike && thingList[i].Faction == Faction.OfPlayer)
						{
							pawn = (thingList[i] as Pawn);
							break;
						}
					}
					if (pawn != null)
					{
						break;
					}
				}
				if (pawn != null)
				{
					this.ActivatedBy(pawn);
				}
			}
		}

		public ActivatedActionDef actionDef;

		private ICollection<IntVec3> cells = new List<IntVec3>();
	}
}

