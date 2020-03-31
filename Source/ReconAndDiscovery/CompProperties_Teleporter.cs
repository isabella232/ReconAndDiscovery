using System;
using Verse;

namespace ReconAndDiscovery
{
	public class CompProperties_Teleporter : CompProperties
	{
		public CompProperties_Teleporter()
		{
			this.compClass = typeof(CompTeleporter);
		}

		public float tickCharge = 0.005f;
	}
}

