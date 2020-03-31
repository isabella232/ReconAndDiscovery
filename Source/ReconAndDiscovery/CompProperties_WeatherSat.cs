using System;
using Verse;

namespace ReconAndDiscovery
{
	public class CompProperties_WeatherSat : CompProperties
	{
		public CompProperties_WeatherSat()
		{
			this.compClass = typeof(CompWeatherSat);
		}

		public float tickCharge = 0.5f;
	}
}

