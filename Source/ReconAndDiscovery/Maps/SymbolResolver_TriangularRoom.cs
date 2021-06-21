using RimWorld.BaseGen;

namespace ReconAndDiscovery.Maps
{
    public class SymbolResolver_TriangularRoom : SymbolResolver
    {
        public override void Resolve(ResolveParams rp)
        {
            var map = BaseGen.globalSettings.map;
            MapGenUtility.MakeTriangularRoom(map, rp);
        }
    }
}