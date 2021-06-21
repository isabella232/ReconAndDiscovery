using ReconAndDiscovery.Triggers;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps
{
    public class SymbolResolver_PlaceTrigger : SymbolResolver
    {
        public override void Resolve(ResolveParams rp)
        {
            var map = BaseGen.globalSettings.map;
            if (rp.TryGetCustom("trigger", out RectActionTrigger rectActionTrigger))
            {
                GenSpawn.Spawn(rectActionTrigger, rectActionTrigger.Rect.CenterCell, map);
            }
        }
    }
}