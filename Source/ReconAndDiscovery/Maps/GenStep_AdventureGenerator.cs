using System;
using System.Collections.Generic;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public class GenStep_AdventureGenerator : GenStep
	{

        public override int SeedPart
        {
            get
            {
                return 149641510;
            }
        }
        public override void Generate(Map map, GenStepParams parms)
		{
			int minX = map.Size.x / 5;
			int width = 3 * map.Size.x / 5;
			int minZ = map.Size.z / 5;
			int height = 3 * map.Size.z / 5;
			this.adventureRegion = new CellRect(minX, minZ, width, height);
			this.adventureRegion.ClipInsideMap(map);
			BaseGen.globalSettings.map = map;
			this.randomRoomEvents.Clear();
			IntVec3 playerStartSpot;
			CellFinder.TryFindRandomEdgeCellWith((IntVec3 v) => v.Standable(map), map, 0f, out playerStartSpot);
			MapGenerator.PlayerStartSpot = playerStartSpot;
			this.baseResolveParams = default(ResolveParams);
			foreach (string text in this.randomRoomEvents.Keys)
			{
				this.baseResolveParams.SetCustom<float>(text, this.randomRoomEvents[text], false);
			}
		}

		protected AdventureWorker adventure = null;

		protected Dictionary<string, float> randomRoomEvents = new Dictionary<string, float>();

		protected CellRect adventureRegion;

		protected ResolveParams baseResolveParams;
	}
}

