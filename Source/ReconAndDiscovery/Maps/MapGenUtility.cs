using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public static class MapGenUtility
	{
		public static void AddGenStep(GenStepDef step)
		{
			MapGenUtility.customGenSteps.Add(step);
		}

		public static void ResolveCustomGenSteps(Map map, GenStepParams parms)
		{
			foreach (GenStepDef genStepDef in MapGenUtility.customGenSteps)
			{
				genStepDef.genStep.Generate(map, parms);
			}
			MapGenUtility.customGenSteps.Clear();
		}

		public static bool TryFindRandomCellWhere(IEnumerable<IntVec3> candidates, Predicate<IntVec3> validator, out IntVec3 loc)
		{
			loc = default(IntVec3);
			IEnumerable<IntVec3> source = from v in candidates
			where validator(v)
			select v;
			bool result;
			if (source.Count<IntVec3>() > 0)
			{
				loc = source.RandomElement<IntVec3>();
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public static void MakeRoom(Map map, CellRect rect, RoomStructure rs)
		{
			if (rs.wallMaterial == null)
			{
                Faction faction = Find.FactionManager.RandomNonHostileFaction(false, false, true, TechLevel.Spacer);
                rs.wallMaterial = BaseGenUtility.RandomCheapWallStuff(faction, false);
			}
			if (rs.floorMaterial == null)
			{
				rs.floorMaterial = BaseGenUtility.CorrespondingTerrainDef(rs.wallMaterial, true);
			}
			if (rs.floorMaterial == null)
			{
				rs.floorMaterial = BaseGenUtility.RandomBasicFloorDef(Faction.OfMechanoids, false);
			}
			MapGenUtility.TileArea(map, rect, rs.floorMaterial, rs.floorChance);
			MapGenUtility.MakeLongWall(new IntVec3(rect.minX, 1, rect.minZ), map, rect.Width, true, rs.wallS, rs.wallMaterial);
			MapGenUtility.MakeLongWall(new IntVec3(rect.minX, 1, rect.maxZ), map, rect.Width, true, rs.wallN, rs.wallMaterial);
			MapGenUtility.MakeLongWall(new IntVec3(rect.minX, 1, rect.minZ), map, rect.Height, false, rs.wallW, rs.wallMaterial);
			MapGenUtility.MakeLongWall(new IntVec3(rect.maxX, 1, rect.minZ), map, rect.Height, false, rs.wallE, rs.wallMaterial);
			for (int i = 0; i < rs.doorN; i++)
			{
				MapGenUtility.RandomAddDoor(new IntVec3(rect.minX + 2, 1, rect.maxZ), map, rect.Width - 3, true, rs.wallMaterial);
			}
			for (int j = 0; j < rs.doorS; j++)
			{
				MapGenUtility.RandomAddDoor(new IntVec3(rect.minX + 2, 1, rect.minZ), map, rect.Width - 3, true, rs.wallMaterial);
			}
			for (int k = 0; k < rs.doorE; k++)
			{
				MapGenUtility.RandomAddDoor(new IntVec3(rect.minX, 1, rect.minZ + 2), map, rect.Height - 3, false, rs.wallMaterial);
			}
			for (int l = 0; l < rs.doorW; l++)
			{
				MapGenUtility.RandomAddDoor(new IntVec3(rect.maxX, 1, rect.minZ + 2), map, rect.Height - 3, false, rs.wallMaterial);
			}
			map.MapUpdate();
		}

		public static List<ThingStuffPair> GetWeapons(Predicate<ThingDef> validator)
		{
			List<ThingStuffPair> list = new List<ThingStuffPair>();
			if (MapGenUtility.weapons.NullOrEmpty<ThingStuffPair>())
			{
				MapGenUtility.FillPossibleObjectLists();
			}
			foreach (ThingStuffPair item in MapGenUtility.weapons)
			{
				if (validator(item.thing))
				{
					list.Add(item);
				}
			}
			return list;
		}

		public static void MakeTriangularRoom(Map map, ResolveParams rp)
		{
			if (rp.wallStuff == null)
			{
                Faction faction = Find.FactionManager.RandomNonHostileFaction(false, false, true, TechLevel.Spacer);

                rp.wallStuff = BaseGenUtility.RandomCheapWallStuff(faction, false);
			}
			if (rp.floorDef == null)
			{
				rp.floorDef = BaseGenUtility.CorrespondingTerrainDef(rp.wallStuff, true);
			}
			if (rp.floorDef == null)
			{
				rp.floorDef = BaseGenUtility.RandomBasicFloorDef(Faction.OfMechanoids, false);
			}
			ResolveParams resolveParams = rp;
			resolveParams.rect = new CellRect(rp.rect.minX, rp.rect.minZ, 1, rp.rect.Height);
			BaseGen.symbolStack.Push("edgeWalls", resolveParams, null);
			for (int i = 0; i <= rp.rect.Width; i++)
			{
				int num = rp.rect.minX + i;
				int num2 = (int)Math.Floor((double)(0.5f * (float)i));
				int num3 = (int)Math.Ceiling((double)(0.5f * (float)i));
				for (int j = rp.rect.minZ + num2; j < rp.rect.minZ + rp.rect.Width - num2; j++)
				{
					foreach (Thing thing in map.thingGrid.ThingsAt(new IntVec3(num, 1, j)))
					{
                        thing.TakeDamage(new DamageInfo(DamageDefOf.Blunt, 10000, -1f, -1f, null, null, null, 0));
					}
					MapGenUtility.TryToSetFloorTile(new IntVec3(num, 1, j), map, rp.floorDef);
					if (j == rp.rect.minZ + num2 || j == rp.rect.minZ + num3 || j == rp.rect.minZ + rp.rect.Width - (num2 + 1) || j == rp.rect.minZ + rp.rect.Width - (num3 + 1))
					{
						ResolveParams resolveParams2 = rp;
						resolveParams2.rect = new CellRect(num, j, 1, 1);
						BaseGen.symbolStack.Push("edgeWalls", resolveParams2, null);
					}
				}
			}
			map.MapUpdate();
			RoofGrid roofGrid = BaseGen.globalSettings.map.roofGrid;
			RoofDef def = rp.roofDef ?? RoofDefOf.RoofConstructed;
			for (int k = 0; k <= rp.rect.Width; k++)
			{
				int newX = rp.rect.minX + k;
				int num4 = (int)Math.Floor((double)(0.5f * (float)k));
				for (int l = rp.rect.minZ + num4; l < rp.rect.minZ + rp.rect.Width - num4; l++)
				{
					IntVec3 c = new IntVec3(newX, 1, l);
					if (!roofGrid.Roofed(c))
					{
						roofGrid.SetRoof(c, def);
					}
				}
			}
		}

		public static void DestroyAllInArea(Map map, CellRect rect)
		{
			for (int i = rect.minX; i <= rect.maxX; i++)
			{
				for (int j = rect.minZ; j <= rect.maxZ; j++)
				{
					IntVec3 c = new IntVec3(i, 1, j);
					MapGenUtility.DestroyAllAtLocation(c, map);
					//rect.GetIterator().MoveNext();
					//c = rect.GetIterator().Current;
				}
			}
		}

		public static void TileArea(Map map, CellRect rect, TerrainDef floorMaterial = null, float floorIntegrity = 1f)
		{
			if (floorMaterial == null)
			{
				floorMaterial = BaseGenUtility.RandomBasicFloorDef(Faction.OfMechanoids, false);
			}
			for (int i = rect.minX; i <= rect.maxX; i++)
			{
				for (int j = rect.minZ; j <= rect.maxZ; j++)
				{
					IntVec3 c = new IntVec3(i, 1, j);
					if (Rand.Value <= floorIntegrity)
					{
						MapGenUtility.TryToSetFloorTile(c, map, floorMaterial);
						//rect.GetIterator().MoveNext();
						//c = rect.GetIterator().Current;
					}
				}
			}
		}

		public static void RoofArea(Map map, CellRect rect, float roofIntegrity = 1f)
		{
			for (int i = rect.minX; i <= rect.maxX; i++)
			{
				for (int j = rect.minZ; j <= rect.maxZ; j++)
				{
					IntVec3 c = new IntVec3(i, 1, j);
					if (Rand.Value <= roofIntegrity)
					{
						if (!map.roofGrid.Roofed(c))
						{
							map.roofGrid.SetRoof(c, RoofDefOf.RoofConstructed);
						}
					}
				}
			}
		}

		public static void RandomAddDoor(IntVec3 start, Map map, int extent, bool horizontal, ThingDef material = null)
		{
			if (material == null)
			{
				material = BaseGenUtility.RandomCheapWallStuff(Find.FactionManager.RandomNonHostileFaction(false, false, true, TechLevel.Spacer), false);
			}
			int num = Rand.RangeInclusive(0, extent);
			if (horizontal)
			{
				start.x += num;
			}
			else
			{
				start.z += num;
			}
			MapGenUtility.TryMakeDoor(start, map, material);
		}

		public static void FillPossibleObjectLists()
		{
			if (MapGenUtility.weapons.NullOrEmpty<ThingStuffPair>())
			{
				MapGenUtility.weapons = ThingStuffPair.AllWith((ThingDef td) => td.IsWeapon && !td.weaponTags.NullOrEmpty<string>());
			}
		}

		public static void ScatterWeaponsWhere(CellRect within, int num, Map map, Predicate<ThingDef> validator)
		{
			List<ThingStuffPair> source = MapGenUtility.GetWeapons(validator);
			for (int i = 0; i < num; i++)
			{
				ThingStuffPair thingStuffPair = source.RandomElement<ThingStuffPair>();
				Thing thing = ThingMaker.MakeThing(thingStuffPair.thing, thingStuffPair.stuff);
				CompQuality compQuality = thing.TryGetComp<CompQuality>();
				if (compQuality != null)
				{
					compQuality.SetQuality(QualityUtility.GenerateQualityCreatedByPawn(12, false), ArtGenerationContext.Outsider);
				}
				IntVec3 loc2;
				if (thing != null && CellFinder.TryFindRandomCellInsideWith(within, (IntVec3 loc) => loc.Standable(map), out loc2))
				{
					GenSpawn.Spawn(thing, loc2, map);
				}
			}
		}

		public static void MakeLongWall(IntVec3 start, Map map, int extendDist, bool horizontal, float integrity, ThingDef stuffDef)
		{
			IntVec3 c = start;
			for (int i = 0; i < extendDist; i++)
			{
				if (!c.InBounds(map))
				{
					break;
				}
				if (Rand.Value < integrity)
				{
					MapGenUtility.TrySetCellAsWall(c, map, stuffDef);
				}
				if (horizontal)
				{
					c.x++;
				}
				else
				{
					c.z++;
				}
			}
		}

		private static void TrySetCellAsWall(IntVec3 c, Map map, ThingDef stuffDef)
		{
			List<Thing> thingList = c.GetThingList(map);
			for (int i = 0; i < thingList.Count; i++)
			{
				if (!thingList[i].def.destroyable)
				{
					return;
				}
			}
			for (int j = thingList.Count - 1; j >= 0; j--)
			{
				thingList[j].Destroy(DestroyMode.Vanish);
			}
			map.terrainGrid.SetTerrain(c, BaseGenUtility.CorrespondingTerrainDef(stuffDef, true));
			Thing newThing = ThingMaker.MakeThing(ThingDefOf.Wall, stuffDef);
			GenSpawn.Spawn(newThing, c, map);
		}

		public static void DestroyAllAtLocation(IntVec3 c, Map map)
		{
			List<Thing> thingList = c.GetThingList(map);
			for (int i = 0; i < thingList.Count; i++)
			{
				if (!thingList[i].def.destroyable)
				{
					return;
				}
			}
			for (int j = thingList.Count - 1; j >= 0; j--)
			{
				thingList[j].Destroy(DestroyMode.Vanish);
			}
		}

		private static void TryToSetFloorTile(IntVec3 c, Map map, TerrainDef floorDef)
		{
			List<Thing> thingList = c.GetThingList(map);
			for (int i = 0; i < thingList.Count; i++)
			{
				if (!thingList[i].def.destroyable)
				{
					return;
				}
			}
			for (int j = thingList.Count - 1; j >= 0; j--)
			{
				thingList[j].Destroy(DestroyMode.Vanish);
			}
			map.terrainGrid.SetTerrain(c, floorDef);
		}

		private static void TryMakeDoor(IntVec3 c, Map map, ThingDef doorStuff = null)
		{
			if (doorStuff == null)
			{
				doorStuff = BaseGenUtility.RandomCheapWallStuff(Find.FactionManager.RandomNonHostileFaction(false, false, true, TechLevel.Spacer), false);
			}
			List<Thing> thingList = c.GetThingList(map);
			for (int i = 0; i < thingList.Count; i++)
			{
				if (!thingList[i].def.destroyable)
				{
					return;
				}
			}
			for (int j = thingList.Count - 1; j >= 0; j--)
			{
				thingList[j].Destroy(DestroyMode.Vanish);
			}
			Thing newThing = ThingMaker.MakeThing(ThingDefOf.Door, doorStuff);
			GenSpawn.Spawn(newThing, c, map);
		}

		public static void UnfogFromRandomEdge(Map map)
		{
			MapGenUtility.FogAll(map);
			MapGenerator.rootsToUnfog.Clear();
			foreach (Pawn pawn in map.mapPawns.FreeColonists)
			{
				MapGenerator.rootsToUnfog.Add(pawn.Position);
			}
			MapGenerator.rootsToUnfog.Add(CellFinderLoose.RandomCellWith((IntVec3 loc) => loc.Standable(map) && (loc.x < 4 || loc.z < 4 || loc.x > map.Size.x - 5 || loc.z > map.Size.z - 5), map, 1000));
			foreach (IntVec3 root in MapGenerator.rootsToUnfog)
			{
				FloodFillerFog.FloodUnfog(root, map);
			}
		}

		public static void FogAll(Map map)
		{
			FogGrid fogGrid = map.fogGrid;
			if (fogGrid != null)
			{
				CellIndices cellIndices = map.cellIndices;
				if (fogGrid.fogGrid == null)
				{
					fogGrid.fogGrid = new bool[cellIndices.NumGridCells];
				}
				foreach (IntVec3 c in map.AllCells)
				{
					fogGrid.fogGrid[cellIndices.CellToIndex(c)] = true;
				}
				if (Current.ProgramState == ProgramState.Playing)
				{
					map.roofGrid.Drawer.SetDirty();
				}
			}
		}

		public static ThingDef RandomExpensiveWallStuff(Faction faction, bool notVeryFlammable = true)
		{
			ThingDef result;
			if (faction != null && faction.def.techLevel.IsNeolithicOrWorse())
			{
				result = ThingDefOf.WoodLog;
			}
			else
			{
				result = (from d in DefDatabase<ThingDef>.AllDefsListForReading
				where d.IsStuff && d.stuffProps.CanMake(ThingDefOf.Wall) && (!notVeryFlammable || d.BaseFlammability < 0.5f) && d.BaseMarketValue / d.VolumePerUnit > 8f
				select d).RandomElement<ThingDef>();
			}
			return result;
		}

		public static void PushDoor(IntVec3 loc)
		{
			MapGenUtility.DoorPosList.Add(loc);
		}

		public static void MakeDoors(ResolveParams rp, Map map)
		{
			foreach (IntVec3 intVec in MapGenUtility.DoorPosList)
			{
				MapGenUtility.DestroyAllAtLocation(intVec, map);
				Thing thing = ThingMaker.MakeThing(ThingDefOf.Door, rp.wallStuff);
				if (map.ParentFaction != null)
				{
					thing.SetFaction(map.ParentFaction, null);
				}
				else
				{
                    Faction faction = Find.FactionManager.RandomEnemyFaction(true, false, true, TechLevel.Spacer);
                    thing.SetFaction(faction, null);
				}
				GenSpawn.Spawn(thing, intVec, map);
			}
			MapGenUtility.DoorPosList.Clear();
		}

		private static List<IntVec3> DoorPosList = new List<IntVec3>();

		private static List<ThingStuffPair> weapons; 

		private static List<GenStepDef> customGenSteps = new List<GenStepDef>();
	}
}

