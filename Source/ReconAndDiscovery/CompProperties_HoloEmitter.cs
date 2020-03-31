using System;
using Verse;

namespace ReconAndDiscovery
{
	public class CompProperties_HoloEmitter : CompProperties
	{
		public CompProperties_HoloEmitter()
		{
			this.compClass = typeof(CompHoloEmitter);
		}

		public float tickCharge = 0.5f;
	}
}

