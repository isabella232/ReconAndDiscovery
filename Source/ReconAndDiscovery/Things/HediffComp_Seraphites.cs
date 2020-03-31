using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Things
{
	public class HediffComp_Seraphites : HediffComp
	{
		public HediffCompProperties_Seraphites Props
		{
			get
			{
				return (HediffCompProperties_Seraphites)this.props;
			}
		}

		public override void CompPostTick(ref float severityAdjustment)
		{
			if (this.parent.ageTicks % 100 == 0)
			{
				this.FixLuciferumHediffs();
			}
		}

		private void FixLuciferumHediffs()
		{
			this.toRemove.Clear();
			foreach (Hediff hediff in base.Pawn.health.hediffSet.hediffs)
			{
				if (hediff is Hediff_Addiction)
				{
					this.toRemove.Add(hediff);
				}
				if (hediff.def == HediffDef.Named("LuciferiumHigh"))
				{
					this.toRemove.Add(hediff);
				}
			}
			foreach (Hediff hediff2 in this.toRemove)
			{
				base.Pawn.health.RemoveHediff(hediff2);
			}
		}

		private List<Hediff> toRemove = new List<Hediff>();
	}
}

