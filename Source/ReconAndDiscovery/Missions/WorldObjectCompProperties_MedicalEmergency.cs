using System;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Missions
{
	public class WorldObjectCompProperties_MedicalEmergency : WorldObjectCompProperties
	{
		public WorldObjectCompProperties_MedicalEmergency()
		{
			this.compClass = typeof(QuestComp_MedicalEmergency);
		}
	}
}

