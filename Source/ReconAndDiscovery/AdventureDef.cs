using System;
using ReconAndDiscovery.Triggers;
using RimWorld;
using Verse;

namespace ReconAndDiscovery
{
	public class AdventureDef : IncidentDef
	{
		public EventDistanceType eventDistance = EventDistanceType.PLANETWIDE;

		public MapGeneratorDef mapGenerator = null;

		public AdventureDetectionType detectionType = AdventureDetectionType.NONE;

		public GameConditionDef targetGameCondition = null;

		public GameConditionDef adventureGameCondition = null;

		public ActivatedActionDef mainAdventureAction = null;

		public float luciferiumGasChance = 0f;

		public float psionicLandmineChance = 0f;

		public float madAnimalChance = 0f;

		public float smallGoldChance = 0f;

		public float smallSilverChance = 0f;

		public string worldLabel = "Adventure";

		public string mapPinPath = "";

		public string zoomedMapPinPath = "";

		public bool hasInitiatedDialog = false;

		public string initiatedText = "";

		public bool hasMapEnterDialog = false;

		public string mapEnterText = "";

		public bool hasWorldMapDialog = false;

		public string worldMapText = "";
	}
}

