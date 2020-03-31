using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public class SymbolResolver_AbandonedLab : SymbolResolver
	{
		public override bool CanResolve(ResolveParams rp)
		{
			return base.CanResolve(rp);
		}

		public override void Resolve(ResolveParams rp)
		{
			rp.floorDef = TerrainDefOf.Concrete;
			ThingDef plasteel = ThingDefOf.Plasteel;
			ResolveParams resolveParams = rp;
			rp.wallStuff = plasteel;
			int minX = Rand.RangeInclusive(rp.rect.minX + 5, rp.rect.minX + 10);
			int minZ = Rand.RangeInclusive(rp.rect.minZ + 5, rp.rect.minZ + 10);
			int width = Rand.RangeInclusive(25, 35);
			int height = Rand.RangeInclusive(25, 35);
			resolveParams.rect = new CellRect(minX, minZ, width, height);
			resolveParams.floorDef = TerrainDefOf.WoodPlankFloor;
			resolveParams.wallStuff = ThingDefOf.Plasteel;
			ResolveParams resolveParams2 = resolveParams;
			ResolveParams resolveParams3 = resolveParams;
			List<ThingDef> list = new List<ThingDef>();
			list.Add(ThingDef.Named("DrugLab"));
			list.Add(ThingDef.Named("SimpleResearchBench"));
			list.Add(ThingDef.Named("SimpleResearchBench"));
			list.Add(ThingDef.Named("SimpleResearchBench"));
			list.Add(ThingDef.Named("HiTechResearchBench"));
			resolveParams2.floorDef = TerrainDefOf.MetalTile;
			resolveParams2.wallStuff = ThingDefOf.Plasteel;
			resolveParams2.SetCustom<char[]>("hasDoor", new char[]
			{
				'E'
			}, false);
			for (int i = 0; i < 20; i++)
			{
				if (resolveParams2.rect.minZ > resolveParams.rect.maxZ - 2)
				{
					break;
				}
				resolveParams2.rect = new CellRect(resolveParams2.rect.minX, resolveParams2.rect.minZ, 7, 6);
				resolveParams3.rect = resolveParams2.rect.ContractedBy(1);
				resolveParams3.rect = new CellRect(resolveParams3.rect.minX + 1, resolveParams3.rect.minZ + 1, 1, 1);
				resolveParams3.thingRot = new Rot4?(Rot4.West);
				resolveParams3.singleThingDef = list.RandomElement<ThingDef>();
				BaseGen.symbolStack.Push("insertFurnishing", resolveParams3, null);
				BaseGen.symbolStack.Push("roomWithDoor", resolveParams2, null);
				resolveParams2.rect.minZ = resolveParams2.rect.maxZ;
			}
			resolveParams2.rect.minX = resolveParams2.rect.maxX + 3;
			Rot4 rot = Rot4.North;
			if (Rand.Chance(0.5f))
			{
				resolveParams2.rect.minZ = resolveParams.rect.minZ;
				resolveParams2.SetCustom<char[]>("hasDoor", new char[]
				{
					'N'
				}, false);
				resolveParams.SetCustom<char[]>("hasDoor", new char[]
				{
					'E',
					'N'
				}, false);
				rot = Rot4.South;
			}
			else
			{
				resolveParams2.rect.minZ = resolveParams.rect.maxZ - 6;
				resolveParams2.SetCustom<char[]>("hasDoor", new char[]
				{
					'S'
				}, false);
				resolveParams.SetCustom<char[]>("hasDoor", new char[]
				{
					'E',
					'S'
				}, false);
			}
			for (int j = 0; j < 20; j++)
			{
				if (resolveParams2.rect.minX > resolveParams.rect.maxX - 2)
				{
					break;
				}
				resolveParams2.rect = new CellRect(resolveParams2.rect.minX, resolveParams2.rect.minZ, 6, 7);
				resolveParams3.rect = resolveParams2.rect.ContractedBy(1);
				if (rot == Rot4.South)
				{
					resolveParams3.rect = new CellRect(resolveParams3.rect.minX + 1, resolveParams3.rect.minZ + 1, 1, 1);
				}
				else
				{
					resolveParams3.rect = new CellRect(resolveParams3.rect.minX + 1, resolveParams3.rect.maxZ - 1, 1, 1);
				}
				resolveParams3.thingRot = new Rot4?(rot);
				resolveParams3.singleThingDef = list.RandomElement<ThingDef>();
				BaseGen.symbolStack.Push("insertFurnishing", resolveParams3, null);
				BaseGen.symbolStack.Push("roomWithDoor", resolveParams2, null);
				resolveParams2.rect.minX = resolveParams2.rect.maxX;
			}
			ResolveParams resolveParams4 = rp;
			resolveParams4.singleThingDef = ThingDef.Named("Table2x4c");
			resolveParams4.rect = new CellRect(resolveParams.rect.CenterCell.x, resolveParams.rect.CenterCell.z, 1, 1);
			resolveParams4.thingRot = new Rot4?(Rot4.North);
			BaseGen.symbolStack.Push("insertFurnishing", resolveParams4, null);
			for (int k = 0; k < 4; k++)
			{
				resolveParams4.singleThingDef = ThingDef.Named("DiningChair");
				resolveParams4.rect = new CellRect(resolveParams.rect.CenterCell.x - 1, resolveParams.rect.CenterCell.z + 2 - k, 1, 1);
				resolveParams4.thingRot = new Rot4?(Rot4.East);
				BaseGen.symbolStack.Push("insertFurnishing", resolveParams4, null);
			}
			BaseGen.symbolStack.Push("roomWithDoor", resolveParams, null);
			resolveParams.chanceToSkipWallBlock = new float?(0.05f);
			BaseGen.symbolStack.Push("wireOutline", resolveParams, null);
			ResolveParams resolveParams5 = rp;
			resolveParams5.rect = rp.rect.ContractedBy(2);
			resolveParams5.rect = new CellRect(resolveParams5.rect.minX, resolveParams5.rect.minZ, 6, 5);
			resolveParams5.wallStuff = ThingDef.Named("BlocksLimestone");
			resolveParams5.SetCustom<char[]>("hasDoor", new char[]
			{
				'N'
			}, false);
			BaseGen.symbolStack.Push("roomWithDoor", resolveParams5, null);
			BaseGen.symbolStack.Push("wireOutline", resolveParams5, null);
			resolveParams5.rect = resolveParams5.rect.ContractedBy(1);
			resolveParams5.singleThingDef = ThingDef.Named("ChemfuelPoweredGenerator");
			resolveParams5.thingRot = new Rot4?(Rot4.North);
			BaseGen.symbolStack.Push("insertFurnishing", resolveParams5, null);
			BaseGen.symbolStack.Push("edgeFence", rp, null);
			BaseGen.symbolStack.Push("floor", rp, null);
			BaseGen.symbolStack.Push("clear", rp, null);
		}
	}
}

