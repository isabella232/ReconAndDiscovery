using System;
using Verse;

namespace ReconAndDiscovery
{
	public class CompProperties_Stargate : CompProperties
	{
		public CompProperties_Stargate()
		{
			this.compClass = typeof(CompStargate);
		}

		public float tickCharge = 0.5f;
	}
}

