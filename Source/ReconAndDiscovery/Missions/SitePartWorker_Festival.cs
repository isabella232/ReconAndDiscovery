using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace ReconAndDiscovery.Missions
{
	public class SitePartWorker_Festival : SitePartWorker
	{
		public List<Faction> Factions
		{
			get
			{
				if (this.factions.NullOrEmpty<Faction>())
				{
					this.factions = Find.FactionManager.AllFactionsVisible.ToList<Faction>();
					if (this.factions.Contains(this.hostFaction))
					{
						this.factions.Remove(this.hostFaction);
					}
					this.factions = (from f in this.factions
					where f != Faction.OfPlayer && f.GoodwillWith(this.hostFaction) >= 0f
					select f).ToList<Faction>();
				}
				return this.factions;
			}
		}

		private void IncrementAllGoodwills()
		{
			foreach (Faction faction in this.Factions)
			{
				if (faction.PlayerGoodwill >= 0f)
				{
                    faction.TryAffectGoodwillWith(Faction.OfPlayer, 5);
				}
			}
		}
         
		private List<Pawn> SpawnPawns(IncidentParms parms)
		{
			Map map = parms.target as Map;
			Log.Message(string.Format("Spawning pawns for {0}", parms.faction.Name));
            PawnGroupMakerParms defaultPawnGroupMakerParms = IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Trader, parms, false);
            List<Pawn> list = PawnGroupMakerUtility
                .GeneratePawns(defaultPawnGroupMakerParms, false).ToList<Pawn>();

			foreach (Pawn newThing in list)
			{
				IntVec3 loc = CellFinder.RandomClosewalkCellNear(parms.spawnCenter, map, 5, null);
				GenSpawn.Spawn(newThing, loc, map);
			}
			return list;
		}

		private void MakeTradeCaravan(Faction faction, IntVec3 spot, Map map)
		{
			IncidentParms incidentParms = Find.Storyteller.storytellerComps[0].GenerateParms(IncidentCategoryDefOf.OrbitalVisitor, map);
			incidentParms.points = Mathf.Min(800f, incidentParms.points);
			incidentParms.spawnCenter = spot;
			incidentParms.faction = faction;
			List<Pawn> list = this.SpawnPawns(incidentParms);
			if (list.Count != 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].needs != null && list[i].needs.food != null)
					{
						list[i].needs.food.CurLevel = list[i].needs.food.MaxLevel;
					}
				}
				for (int j = 0; j < list.Count; j++)
				{
					Pawn pawn = list[j];
					if (pawn.TraderKind != null)
					{
						TraderKindDef traderKind = pawn.TraderKind;
						break;
					}
				}
				LordJob_TradeWithColony lordJob = new LordJob_TradeWithColony(incidentParms.faction, spot);
				LordMaker.MakeNewLord(incidentParms.faction, lordJob, map, list);
			}
		}

		private void MakeTradeCaravans(Map map)
		{
			IntVec3 spot;
			if (RCellFinder.TryFindRandomSpotJustOutsideColony(CellFinderLoose.RandomCellWith((IntVec3 c) => c.Standable(map), map, 1000), map, out spot))
			{
				foreach (Faction faction in this.Factions)
				{
					this.MakeTradeCaravan(faction, spot, map);
				}
				this.MakeTradeCaravan(this.hostFaction, spot, map);
			}
		}

		private void MakePartyGroups(Map map)
		{
			List<Thing> list = (from t in map.listerThings.AllThings
			where t.def.IsTable
			select t).ToList<Thing>();
			if (list.Count != 0)
			{
				Thing thing = list.RandomElement<Thing>();
				IntVec3 position = thing.Position;
			}
		}

		public override void PostMapGenerate(Map map)
		{
			base.PostMapGenerate(map);
            this.hostFaction = Find.WorldObjects.MapParentAt(map.Tile).Faction;
			this.MakeTradeCaravans(map);
			this.MakePartyGroups(map);
			this.IncrementAllGoodwills();
		}

		public Faction hostFaction; 

		private List<Faction> factions;
	}
}

