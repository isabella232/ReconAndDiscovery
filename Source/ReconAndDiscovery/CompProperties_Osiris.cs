using System;
using Verse;

namespace ReconAndDiscovery
{
	public class CompProperties_Osiris : CompProperties
	{
		public CompProperties_Osiris()
		{
			this.compClass = typeof(CompOsiris);
		}

		public float tickCharge = 0.5f;
	}
}

