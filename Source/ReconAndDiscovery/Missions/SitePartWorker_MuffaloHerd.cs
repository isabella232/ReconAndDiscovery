using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReconAndDiscovery.Missions
{
	public class SitePartWorker_MuffaloHerd : SitePartWorker
	{
		public void QueueFactionArrival(Faction faction, Map map) 
		{
            //TODO: check if it works
			IncidentParms incidentParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.FactionArrival, map);
			incidentParms.forced = true;
			IntVec3 spawnCenter;
			if (RCellFinder.TryFindRandomPawnEntryCell(out spawnCenter, map, 0f, false, (IntVec3 v) => v.Standable(map)))
			{
				incidentParms.spawnCenter = spawnCenter;
			}
			IntVec3 spawnCenter2;
			if (CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => map.reachability.CanReachColony(c), map, CellFinder.EdgeRoadChance_Neutral, out spawnCenter2))
			{
				incidentParms.faction = faction;
				incidentParms.spawnCenter = spawnCenter2;
				incidentParms.points *= 4f;
				incidentParms.points = Math.Max(incidentParms.points, 250f);
				QueuedIncident qi = new QueuedIncident(new FiringIncident(IncidentDefOf.TravelerGroup, null, incidentParms), Find.TickManager.TicksGame + Rand.RangeInclusive(5000, 15000));
				Find.Storyteller.incidentQueue.Add(qi);
			}
		}

		public void QueueArrivals(Map map)
		{
			List<Faction> list = (from f in Find.FactionManager.AllFactionsVisible
			where f != Faction.OfPlayer && f.def.techLevel == TechLevel.Neolithic
			select f).ToList<Faction>();
			if (list.Count != 0)
			{
				int num = Rand.RangeInclusive(1, list.Count);
				list.Shuffle<Faction>();
				for (int i = 0; i < num; i++)
				{
					Faction faction = list[i];
					this.QueueFactionArrival(faction, map);
				}
			}
		}

		public void MakeMuffalo(Map map)
		{
			int num = Rand.RangeInclusive(80, 150);
			for (int i = 0; i < num; i++)
			{
				IntVec3 loc = CellFinderLoose.RandomCellWith((IntVec3 c) => c.Standable(map) && !c.Fogged(map), map, 1000);
				Pawn pawn = PawnGenerator.GeneratePawn(PawnKindDef.Named("Muffalo"), null);
				if ((double)Rand.Value < 0.3)
				{
					pawn.ageTracker.AgeBiologicalTicks = (long)Rand.RangeInclusive(1000, 900000);
				}
				GenSpawn.Spawn(pawn, loc, map);
			}
		}

		public override void PostMapGenerate(Map map)
		{
			base.PostMapGenerate(map);
            
			this.MakeMuffalo(map);
			this.QueueArrivals(map);
			if (Rand.Chance(0.05f))
			{
				IEnumerable<Pawn> source = from p in map.mapPawns.FreeColonistsSpawned
				where p.story.traits.HasTrait(TraitDef.Named("PsychicSensitivity"))
				select p;
				if (source.Count<Pawn>() > 0)
				{
					Pawn pawn = source.RandomElement<Pawn>();
					Find.LetterStack.ReceiveLetter("RD_ManhunterDanger".Translate(), "RD_MalevolentPsychicDesc".Translate(pawn.Named("PAWN")) //"{0} believes that a malevolent psychic energy is massing, and that this peaceful herd of muffalo are on the brink of a mass insanity."
, LetterDefOf.ThreatSmall, null);
				}
                //TODO: check if it works 
                IncidentParms incidentParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.FactionArrival, map);
				incidentParms.forced = true;
				incidentParms.points = 100f;
				QueuedIncident qi = new QueuedIncident(new FiringIncident(IncidentDef.Named("RD_MuffaloMassInsanity"), null, incidentParms), Find.TickManager.TicksGame + Rand.RangeInclusive(10000, 45000));
				Find.Storyteller.incidentQueue.Add(qi);
			}
		}
	}
}

