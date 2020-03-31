using System;
using System.Collections.Generic;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public class SymbolResolver_NestedRoomMaze : SymbolResolver
	{
		public override bool CanResolve(ResolveParams rp)
		{
			return base.CanResolve(rp);
		}

		public override void Resolve(ResolveParams rp)
		{
			int num;
			if (!rp.TryGetCustom<int>("minRoomDimension", out num))
			{
				Log.Error("Could not find a field minRoomDimension");
			}
			else
			{
				int width = rp.rect.Width;
				int height = rp.rect.Height;
				Log.Message(string.Format("Current nested room dimensions -> ({0}:{1})", width, height));
				if (width < 2 * num && height < 2 * num)
				{
					this.MakeRoom(rp);
				}
				else if ((width < 4 * num || height < 4 * num) && Rand.Value < 0.2f)
				{
					ResolveParams rp2 = rp;
					ResolveParams rp3 = rp;
					rp2.rect.Width = rp.rect.Width / 2;
					rp3.rect.minX = rp2.rect.maxX;
					rp3.rect.Width = rp.rect.Width / 2;
					this.MakeRoom(rp2);
					this.MakeRoom(rp3);
				}
				else if (width < 4 * num && height < 4 * num && Rand.Value < 0.1f)
				{
					this.MakeRoom(rp);
				}
				else
				{
					List<CellRect> list = new List<CellRect>();
					List<CellRect> list2 = new List<CellRect>();
					if (width > 2 * num)
					{
						list.Add(new CellRect(rp.rect.minX, rp.rect.minZ, rp.rect.Width / 2, rp.rect.Height));
						list.Add(new CellRect(rp.rect.minX + rp.rect.Width / 2 - 1, rp.rect.minZ, rp.rect.Width / 2, rp.rect.Height));
					}
					else
					{
						list.Add(new CellRect(rp.rect.minX, rp.rect.minZ, rp.rect.Width, rp.rect.Height));
					}
					foreach (CellRect item in list)
					{
						if (height > 2 * num)
						{
							list2.Add(new CellRect(item.minX, item.minZ, item.Width, item.Height / 2));
							list2.Add(new CellRect(item.minX, item.minZ + item.Height / 2 - 1, item.Width, item.Height / 2));
						}
						else
						{
							list2.Add(item);
						}
					}
					foreach (CellRect rect in list2)
					{
						ResolveParams resolveParams = rp;
						resolveParams.rect = rect;
						resolveParams.SetCustom<int>("minRoomDimension", num, false);
						BaseGen.symbolStack.Push("nestedRoomMaze", resolveParams, null);
					}
				}
			}
		}

		private void MakeRoom(ResolveParams rp)
		{
			char[] array = new char[3];
			char[] source = new char[]
			{
				'N',
				'S',
				'E',
				'W'
			};
			array[0] = source.RandomElement<char>();
			array[1] = source.RandomElement<char>();
			if (array[1] == array[0])
			{
				array[1] = 'X';
			}
			rp.SetCustom<char[]>("hasDoor", array, false);
			BaseGen.symbolStack.Push("roomWithDoor", rp, null);
		}
	}
}

