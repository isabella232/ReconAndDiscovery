using System;
using ReconAndDiscovery.Triggers;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public class SymbolResolver_PlaceTrigger : SymbolResolver
	{
		public override bool CanResolve(ResolveParams rp)
		{
			return base.CanResolve(rp);
		}

		public override void Resolve(ResolveParams rp)
		{
			Map map = BaseGen.globalSettings.map;
			RectActionTrigger rectActionTrigger;
			if (rp.TryGetCustom<RectActionTrigger>("trigger", out rectActionTrigger))
			{
				GenSpawn.Spawn(rectActionTrigger, rectActionTrigger.Rect.CenterCell, map);
			}
		}
	}
}

