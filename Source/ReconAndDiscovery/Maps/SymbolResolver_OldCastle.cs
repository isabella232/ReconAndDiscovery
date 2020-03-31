using System;
using System.Linq;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public class SymbolResolver_OldCastle : SymbolResolver
	{
		public override bool CanResolve(ResolveParams rp)
		{
			return base.CanResolve(rp);
		}

		public override void Resolve(ResolveParams rp)
		{
			ThingDef thingDef = (from d in DefDatabase<ThingDef>.AllDefs
			where d.IsStuff && d.stuffProps.CanMake(ThingDefOf.Wall) && d.stuffProps.categories.Contains(StuffCategoryDefOf.Stony) && d != ThingDef.Named("Jade")
			select d).ToList<ThingDef>().RandomElementByWeight((ThingDef x) => 3f + 1f / x.BaseMarketValue);
			rp.wallStuff = thingDef;
			rp.floorDef = BaseGenUtility.CorrespondingTerrainDef(thingDef, true);
			int num = Rand.RangeInclusive(30, 40);
			for (int i = 0; i < num; i++)
			{
				int minX = Rand.RangeInclusive(rp.rect.minX, rp.rect.maxX - 1);
				int minZ = Rand.RangeInclusive(rp.rect.minZ, rp.rect.maxZ - 1);
				ResolveParams resolveParams = rp;
				resolveParams.rect = new CellRect(minX, minZ, Rand.RangeInclusive(1, 3), Rand.RangeInclusive(1, 3));
				BaseGen.symbolStack.Push("pathOfDestruction", resolveParams, null);
			}
			for (int j = 0; j < Rand.RangeInclusive(2, 5); j++)
			{
				int minX2 = Rand.RangeInclusive(rp.rect.minX + 4, rp.rect.maxX - 10);
				int minZ2 = Rand.RangeInclusive(rp.rect.minZ + 4, rp.rect.maxZ - 10);
				ResolveParams resolveParams2 = rp;
				resolveParams2.rect = new CellRect(minX2, minZ2, Rand.RangeInclusive(5, 8), Rand.RangeInclusive(5, 8));
				BaseGen.symbolStack.Push("emptyRoom", resolveParams2, null);
			}
			ResolveParams resolveParams3 = rp;
			resolveParams3.rect = resolveParams3.rect.ContractedBy(21);
			ResolveParams resolveParams4 = resolveParams3;
			resolveParams4.rect.Width = (int)(0.25 * (double)resolveParams3.rect.Width);
			resolveParams4.rect.Height = resolveParams4.rect.Width;
			for (int k = 0; k < 4; k++)
			{
				BaseGen.symbolStack.Push("roomWithDoor", resolveParams4, null);
				resolveParams4.rect = new CellRect(resolveParams4.rect.minX, resolveParams4.rect.maxZ, resolveParams4.rect.Width, resolveParams4.rect.Height);
			}
			resolveParams3.SetCustom<char[]>("hasDoor", new char[]
			{
				'N'
			}, false);
			BaseGen.symbolStack.Push("roomWithDoor", resolveParams3, null);
			ResolveParams resolveParams5 = rp;
			resolveParams5.rect = rp.rect.ContractedBy(5);
			ResolveParams resolveParams6 = resolveParams5;
			for (int l = 0; l < 25; l++)
			{
				int width = Rand.RangeInclusive(4, 6);
				int height = Rand.RangeInclusive(4, 7);
				if (resolveParams6.rect.minX > resolveParams5.rect.maxX - 3)
				{
					break;
				}
				resolveParams6.rect = new CellRect(resolveParams6.rect.minX, resolveParams5.rect.minZ, width, height);
				if (Rand.Chance(0.15f))
				{
					resolveParams6.rect.minX = resolveParams6.rect.minX + 3;
				}
				else
				{
					char c = (!Rand.Chance(0.1f)) ? 'N' : 'W';
					resolveParams6.SetCustom<char[]>("hasDoor", new char[]
					{
						c
					}, false);
					BaseGen.symbolStack.Push("roomWithDoor", resolveParams6, null);
					resolveParams6.rect.minX = resolveParams6.rect.maxX;
				}
			}
			resolveParams6 = resolveParams5;
			for (int m = 0; m < 25; m++)
			{
				int width2 = Rand.RangeInclusive(4, 6);
				int num2 = Rand.RangeInclusive(4, 7);
				if (resolveParams6.rect.minX > resolveParams5.rect.maxX - 3)
				{
					break;
				}
				resolveParams6.rect = new CellRect(resolveParams6.rect.minX, resolveParams5.rect.maxZ + 1 - num2, width2, num2);
				if (Rand.Chance(0.15f))
				{
					resolveParams6.rect.minX = resolveParams6.rect.minX + 3;
				}
				else
				{
					char c2 = (!Rand.Chance(0.1f)) ? 'S' : 'W';
					resolveParams6.SetCustom<char[]>("hasDoor", new char[]
					{
						c2
					}, false);
					BaseGen.symbolStack.Push("roomWithDoor", resolveParams6, null);
					resolveParams6.rect.minX = resolveParams6.rect.maxX;
				}
			}
			resolveParams6 = resolveParams5;
			resolveParams6.rect.minZ = resolveParams6.rect.minZ + 4;
			for (int n = 0; n < 25; n++)
			{
				int width3 = Rand.RangeInclusive(4, 6);
				int height2 = Rand.RangeInclusive(4, 7);
				if (resolveParams6.rect.minZ > resolveParams5.rect.maxZ - 4)
				{
					break;
				}
				resolveParams6.rect = new CellRect(resolveParams5.rect.minX, resolveParams6.rect.minZ, width3, height2);
				if (Rand.Chance(0.15f))
				{
					resolveParams6.rect.minZ = resolveParams6.rect.minZ + 3;
				}
				else
				{
					char c3 = (!Rand.Chance(0.1f)) ? 'E' : 'S';
					resolveParams6.SetCustom<char[]>("hasDoor", new char[]
					{
						c3
					}, false);
					BaseGen.symbolStack.Push("roomWithDoor", resolveParams6, null);
					resolveParams6.rect.minZ = resolveParams6.rect.maxZ;
				}
			}
			resolveParams6 = resolveParams5;
			resolveParams6.rect.minZ = resolveParams6.rect.minZ + 4;
			for (int num3 = 0; num3 < 25; num3++)
			{
				int num4 = Rand.RangeInclusive(4, 6);
				int height3 = Rand.RangeInclusive(4, 7);
				if (resolveParams6.rect.minZ > resolveParams5.rect.maxZ - 4)
				{
					break;
				}
				resolveParams6.rect = new CellRect(resolveParams5.rect.maxX + 1 - num4, resolveParams6.rect.minZ, num4, height3);
				if (Rand.Chance(0.15f))
				{
					resolveParams6.rect.minZ = resolveParams6.rect.minZ + 3;
				}
				else
				{
					char c4 = (!Rand.Chance(0.1f)) ? 'W' : 'S';
					resolveParams6.SetCustom<char[]>("hasDoor", new char[]
					{
						c4
					}, false);
					BaseGen.symbolStack.Push("roomWithDoor", resolveParams6, null);
					resolveParams6.rect.minZ = resolveParams6.rect.maxZ;
				}
			}
			ResolveParams resolveParams10;
			ResolveParams resolveParams9;
			ResolveParams resolveParams8;
			ResolveParams resolveParams7 = resolveParams8 = (resolveParams9 = (resolveParams10 = rp));
			CellRect cellRect = new CellRect(rp.rect.maxX, rp.rect.maxZ, 1, 1);
			resolveParams8.rect = cellRect.ExpandedBy(4);
			CellRect cellRect2 = new CellRect(rp.rect.minX, rp.rect.maxZ, 1, 1);
			resolveParams7.rect = cellRect2.ExpandedBy(4);
			CellRect cellRect3 = new CellRect(rp.rect.maxX, rp.rect.minZ, 1, 1);
			resolveParams9.rect = cellRect3.ExpandedBy(4);
			CellRect cellRect4 = new CellRect(rp.rect.minX, rp.rect.minZ, 1, 1);
			resolveParams10.rect = cellRect4.ExpandedBy(4);
			resolveParams8.SetCustom<char[]>("hasDoor", new char[]
			{
				'S',
				'W'
			}, false);
			resolveParams7.SetCustom<char[]>("hasDoor", new char[]
			{
				'S',
				'E'
			}, false);
			resolveParams9.SetCustom<char[]>("hasDoor", new char[]
			{
				'N',
				'W'
			}, false);
			resolveParams10.SetCustom<char[]>("hasDoor", new char[]
			{
				'N',
				'E'
			}, false);
			BaseGen.symbolStack.Push("roomWithDoor", resolveParams8, null);
			BaseGen.symbolStack.Push("roomWithDoor", resolveParams7, null);
			BaseGen.symbolStack.Push("roomWithDoor", resolveParams9, null);
			BaseGen.symbolStack.Push("roomWithDoor", resolveParams10, null);
			ResolveParams resolveParams11 = rp;
			resolveParams11.rect = resolveParams11.rect.ContractedBy(3);
			BaseGen.symbolStack.Push("edgeShields", resolveParams11, null);
			ResolveParams resolveParams12 = rp;
			BaseGen.symbolStack.Push("doors", resolveParams12, null);
			BaseGen.symbolStack.Push("edgeWalls", resolveParams12, null);
			BaseGen.symbolStack.Push("floor", resolveParams12, null);
		}
	}
}

