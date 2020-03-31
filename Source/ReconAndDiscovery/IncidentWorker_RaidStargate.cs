using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace ReconAndDiscovery
{
	public class IncidentWorker_RaidStargate : IncidentWorker_RaidEnemy
	{
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			IEnumerable<Building> source = map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("RD_Stargate"));
			bool result;
			if (source.Count<Building>() == 0)
			{
				result = false;
			}
			else
			{
				Building building = source.FirstOrDefault<Building>();
				this.ResolveRaidPoints(parms);
				if (!this.TryResolveRaidFaction(parms))
				{
					result = false;
				}
				else
				{

                    this.ResolveRaidStrategy(parms, PawnGroupKindDefOf.Combat);
					this.ResolveRaidArriveMode(parms);
                    if (!parms.raidArrivalMode.Worker.TryResolveRaidSpawnCenter(parms))
					{
						result = false;
					}
					else
					{
                        PawnGroupMakerParms defaultPawnGroupMakerParms = IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, parms, true);
						List<Pawn> list = PawnGroupMakerUtility.GeneratePawns(defaultPawnGroupMakerParms, true).ToList<Pawn>();
						if (list.Count == 0)
						{
							Log.Error("Got no pawns spawning raid from parms " + parms);
							result = false;
						}
						else
						{
							TargetInfo target = TargetInfo.Invalid;
							foreach (Pawn pawn in list)
							{
								IntVec3 position = building.Position;
								GenSpawn.Spawn(pawn, position, map, parms.spawnRotation, WipeMode.Vanish, false);
								target = pawn;
							}
                            // TODO: Check if these parameters are correct in this raid
                            Lord lord = LordMaker.MakeNewLord(parms.faction, new LordJob_AssaultColony(parms.faction, true, true, false, false, true), map, list);
                            
                            // not sure what to write here instead, while commented out
                            //AvoidGridMaker.RegenerateAvoidGridsFor(parms.faction, map);

							LessonAutoActivator.TeachOpportunity(ConceptDefOf.EquippingWeapons, OpportunityType.Critical);
							if (!PlayerKnowledgeDatabase.IsComplete(ConceptDefOf.ShieldBelts))
							{
								for (int i = 0; i < list.Count; i++)
								{
									Pawn pawn2 = list[i];
									if (pawn2.apparel.WornApparel.Any((Apparel ap) => ap is ShieldBelt))
									{
										LessonAutoActivator.TeachOpportunity(ConceptDefOf.ShieldBelts, OpportunityType.Critical);
										break;
									}
								}
							}
							base.SendStandardLetter(parms, target, new NamedArgument[]
							{
								parms.faction.Name
							});
							Find.TickManager.slower.SignalForceNormalSpeedShort();
							Find.StoryWatcher.statsRecord.numRaidsEnemy++;
							result = true;
						}
					}
				}
			}
			return result;
		}
	}
}

