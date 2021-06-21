using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Things
{
    public class HediffComp_Seraphites : HediffComp
    {
        private readonly List<Hediff> toRemove = new List<Hediff>();

        public HediffCompProperties_Seraphites Props => (HediffCompProperties_Seraphites) props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (parent.ageTicks % 100 == 0)
            {
                FixLuciferumHediffs();
            }
        }

        private void FixLuciferumHediffs()
        {
            toRemove.Clear();
            foreach (var hediff in Pawn.health.hediffSet.hediffs)
            {
                if (hediff is Hediff_Addiction)
                {
                    toRemove.Add(hediff);
                }

                if (hediff.def == HediffDef.Named("LuciferiumHigh"))
                {
                    toRemove.Add(hediff);
                }
            }

            foreach (var hediff2 in toRemove)
            {
                Pawn.health.RemoveHediff(hediff2);
            }
        }
    }
}