using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace ReconAndDiscovery
{
    public class IncidentWorker_RaidTeleporter : IncidentWorker_RaidEnemy
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            var map = (Map) parms.target;
            var source = from p in map.mapPawns.AllPawnsSpawned
                where p.Faction == Faction.OfMechanoids && !p.Downed
                select p;
            bool result;
            if (!source.Any())
            {
                result = false;
            }
            else
            {
                var enumerable =
                    from b in map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("RD_Teleporter"))
                    where b.GetComp<CompPowerTrader>().PowerOn && b.GetComp<CompTeleporter>().ReadyToTransport
                    select b;
                if (!enumerable.Any())
                {
                    result = false;
                }
                else
                {
                    var list = new List<Pawn>();
                    var p2 = source.RandomElement();
                    foreach (var building in enumerable)
                    {
                        var pawn = PawnGenerator.GeneratePawn(PawnKindDef.Named("Scyther"), Faction.OfMechanoids);
                        GenSpawn.Spawn(pawn, building.Position, building.Map);
                        building.GetComp<CompTeleporter>().ResetCharge();
                        list.Add(pawn);
                        p2.GetLord().AddPawn(pawn);
                    }

                    SendStandardLetter(parms, list.FirstOrDefault());
                    Find.TickManager.slower.SignalForceNormalSpeedShort();
                    Find.StoryWatcher.statsRecord.numRaidsEnemy++;
                    result = true;
                }
            }

            return result;
        }
    }
}