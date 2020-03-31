using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public class SitePartWorker_RadioactiveDust : SitePartWorker
	{
		public override void PostMapGenerate(Map map)
		{
			base.PostMapGenerate(map);
			List<Thing> list = map.listerThings.ThingsInGroup(ThingRequestGroup.Plant).ToList<Thing>();
			foreach (Thing thing in list)
			{
				thing.Destroy(DestroyMode.Vanish);
			}
			GameCondition gameCondition = GameConditionMaker.MakeCondition(GameConditionDef.Named("RD_Radiation"), 3000000);
			map.gameConditionManager.RegisterCondition(gameCondition);
		}
	}
}

