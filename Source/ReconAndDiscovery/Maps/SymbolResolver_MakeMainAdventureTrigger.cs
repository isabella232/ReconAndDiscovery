using System;
using System.Collections.Generic;
using System.Linq;
using ReconAndDiscovery.Triggers;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public class SymbolResolver_MakeMainAdventureTrigger : SymbolResolver
	{
		public override bool CanResolve(ResolveParams rp)
		{
			return base.CanResolve(rp);
		}

		public override void Resolve(ResolveParams rp)
		{
			Map map = BaseGen.globalSettings.map;
			ActivatedActionDef actionDef;
			if (rp.TryGetCustom<ActivatedActionDef>("mainAdventureAction", out actionDef))
			{
				ActionTrigger actionTrigger = null;
				IEnumerable<Thing> source = from t in map.listerThings.AllThings
				where t is ActionTrigger
				select t;
				if (source.Count<Thing>() == 0)
				{
					List<Room> allRooms = map.regionGrid.allRooms;
					if (allRooms.Count == 0)
					{
						Log.Error("Could not find contained room for adventure trigger!");
					}
					else
					{
						Room room = allRooms.RandomElementByWeight((Room r) => 1f / r.GetStat(RoomStatDefOf.Space));
						actionTrigger = new ActionTrigger();
						foreach (IntVec3 item in room.Cells)
						{
							actionTrigger.Cells.Add(item);
						}
						IntVec3 loc = actionTrigger.Cells.RandomElement<IntVec3>();
						GenSpawn.Spawn(actionTrigger, loc, map);
					}
				}
				else
				{
					actionTrigger = (source.RandomElement<Thing>() as ActionTrigger);
				}
				if (actionTrigger != null)
				{
					actionTrigger.actionDef = actionDef;
				}
			}
		}
	}
}

