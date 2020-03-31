using System;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public class SymbolResolver_OldMilitaryBase : SymbolResolver
	{
		public override bool CanResolve(ResolveParams rp)
		{
			return base.CanResolve(rp);
		}

		public override void Resolve(ResolveParams rp)
		{ 
			ResolveParams resolveParams = rp;
			resolveParams.rect = rp.rect.ContractedBy(1);
			resolveParams.wallStuff = ThingDefOf.BlocksGranite;
			resolveParams.SetCustom<int>("minRoomDimension", 6, false);
			BaseGen.symbolStack.Push("nestedRoomMaze", resolveParams, null);
			BaseGen.symbolStack.Push("edgeWalls", resolveParams, null);
			rp.wallStuff = ThingDefOf.Steel;
			BaseGen.symbolStack.Push("edgeWalls", rp, null);
			BaseGen.symbolStack.Push("floor", rp, null);
			BaseGen.symbolStack.Push("clear", rp, null);
		}
	}
}

