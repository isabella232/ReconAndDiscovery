using System.Linq;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Missions
{
    public class IncidentWorker_MuffaloMassInsanity : IncidentWorker
    {
        private static void DriveInsane(Pawn p)
        {
            p.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter, null, true);
        }

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return CanFireNow(parms);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            bool result;
            if (!(parms.target is Map map))
            {
                result = false;
            }
            else
            {
                var animalDef = PawnKindDef.Named("Muffalo");
                var list = (from p in map.mapPawns.AllPawnsSpawned
                    where p.kindDef == animalDef && Rand.Chance(0.5f)
                    select p).ToList();
                if (list.Count < 5)
                {
                    result = false;
                }
                else
                {
                    list.Shuffle();
                    foreach (var p2 in list)
                    {
                        DriveInsane(p2);
                    }

                    string text = "LetterLabelAnimalInsanityMultiple".Translate() + ": " + animalDef.LabelCap;
                    string text2 = "AnimalInsanityMultiple".Translate(new NamedArgument[]
                    {
                        animalDef.label
                    });
                    Find.LetterStack.ReceiveLetter(text, text2, LetterDefOf.NegativeEvent);
                    if (map == Find.CurrentMap)
                    {
                        Find.CameraDriver.shaker.DoShake(1f);
                    }

                    result = true;
                }
            }

            return result;
        }
    }
}