using System;
using Verse;

namespace ReconAndDiscovery
{
	public class CompProperties_MandatoryMilkable : CompProperties
	{
		public CompProperties_MandatoryMilkable()
		{
			this.compClass = typeof(CompMandatoryMilkable);
		}

		public int ticksUntilDanger = 60000;

		public int milkIntervalDays;

		public int milkAmount = 1;

		public ThingDef milkDef;

		public bool milkFemaleOnly = true;
	}
}

