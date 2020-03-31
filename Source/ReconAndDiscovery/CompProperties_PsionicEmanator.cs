using System;
using Verse;

namespace ReconAndDiscovery
{
	public class CompProperties_PsionicEmanator : CompProperties
	{
		public CompProperties_PsionicEmanator()
		{
			this.compClass = typeof(CompPsionicEmanator);
		}

		public float tickCharge = 0.5f;
	}
}

