using System;
using ReconAndDiscovery.Triggers;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public class SymbolResolver_RoomWithDoor : SymbolResolver
	{
		public override bool CanResolve(ResolveParams rp)
		{
			return base.CanResolve(rp);
		}

		public override void Resolve(ResolveParams rp)
		{
			char[] array;
			if (rp.TryGetCustom<char[]>("hasDoor", out array))
			{
				if (rp.rect.Width < 3 && rp.rect.Height < 3)
				{
					return;
				}
				ResolveParams resolveParams = rp;
				char[] array2 = array;
				int i = 0;
				while (i < array2.Length)
				{
					char c = array2[i];
					if (c == 'N')
					{
						resolveParams.thingRot = new Rot4?(Rot4.North);
						resolveParams.rect = new CellRect(rp.rect.minX + 1, rp.rect.maxZ, rp.rect.Width - 2, 1);
						goto IL_1AA;
					}
					if (c == 'S')
					{
						resolveParams.thingRot = new Rot4?(Rot4.South);
						resolveParams.rect = new CellRect(rp.rect.minX + 1, rp.rect.minZ, rp.rect.Width - 2, 1);
						goto IL_1AA;
					}
					if (c == 'E')
					{
						resolveParams.thingRot = new Rot4?(Rot4.East);
						resolveParams.rect = new CellRect(rp.rect.maxX, rp.rect.minZ + 1, 1, rp.rect.Height - 2);
						goto IL_1AA;
					}
					if (c == 'W')
					{
						resolveParams.thingRot = new Rot4?(Rot4.West);
						resolveParams.rect = new CellRect(rp.rect.minX, rp.rect.minZ + 1, 1, rp.rect.Height - 2);
						goto IL_1AA;
					}
					IL_1BB:
					i++;
					continue;
					IL_1AA:
					BaseGen.symbolStack.Push("wallDoor", resolveParams, null);
					goto IL_1BB;
				}
			}
			float chance;
			rp.TryGetCustom<float>("madAnimalChance", out chance);
			float chance2;
			rp.TryGetCustom<float>("luciferiumGasChance", out chance2);
			float chance3;
			rp.TryGetCustom<float>("psionicLandmineChance", out chance3);
			float chance4;
			rp.TryGetCustom<float>("smallGoldChance", out chance4);
			float chance5;
			rp.TryGetCustom<float>("smallSilverChance", out chance5);
			RectActionTrigger rectActionTrigger = ThingMaker.MakeThing(ThingDefOfReconAndDiscovery.RD_RectActionTrigger, null) as RectActionTrigger;
			rectActionTrigger.Rect = rp.rect;
			if (Rand.Chance(chance))
			{
				rectActionTrigger.actionDef = ActionDefOfReconAndDiscovery.RD_MadAnimal;
			}
			else if (Rand.Chance(chance2))
			{
				rectActionTrigger.actionDef = ActionDefOfReconAndDiscovery.RD_LuciferiumGas;
			}
			else if (Rand.Chance(chance3))
			{
				rectActionTrigger.actionDef = ActionDefOfReconAndDiscovery.RD_PsionicLandmine;
			}
			else if (Rand.Chance(chance4))
			{
				rectActionTrigger.actionDef = ActionDefOfReconAndDiscovery.RD_SmallGold;
			}
			else if (Rand.Chance(chance5))
			{
				rectActionTrigger.actionDef = ActionDefOfReconAndDiscovery.RD_SmallSilver;
			}
			else
			{
				rectActionTrigger.actionDef = ActionDefOfReconAndDiscovery.RD_BaseActivatedAction;
			}
			if (rectActionTrigger.actionDef != null)
			{
				ResolveParams resolveParams2 = rp;
				resolveParams2.SetCustom<RectActionTrigger>("trigger", rectActionTrigger, false);
				BaseGen.symbolStack.Push("placeTrigger", resolveParams2, null);
			}
			BaseGen.symbolStack.Push("emptyRoom", rp, null);
		}
	}
}

