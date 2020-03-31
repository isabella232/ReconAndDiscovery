using System;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Missions
{
	public class IncidentWorker_RaidEnemyQuest : IncidentWorker_RaidEnemy
	{
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = parms.target as Map;
			bool result;
			if (map == null)
			{
				result = false;
			}
			else if (!Find.WorldObjects.AnyMapParentAt(map.Tile))
			{
				result = false;
			}
			else if (!Find.WorldObjects.MapParentAt(map.Tile).HasMap)
			{
				result = false;
			}
			else
			{
				try
				{
					result = base.TryExecute(parms);
				}
				catch
				{
					result = false;
				}
			}
			return result;
		}
	}
}

