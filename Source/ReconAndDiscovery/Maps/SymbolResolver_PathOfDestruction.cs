using RimWorld.BaseGen;

namespace ReconAndDiscovery.Maps
{
    public class SymbolResolver_PathOfDestruction : SymbolResolver
    {
        public override void Resolve(ResolveParams rp)
        {
            BaseGen.symbolStack.Push("clear", rp);
        }
    }
}