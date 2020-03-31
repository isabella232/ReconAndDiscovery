using System;
using RimWorld;
using RimWorld.BaseGen;
using Verse;

namespace ReconAndDiscovery.Maps
{
	public class SymbolResolver_InsertFurnishing : SymbolResolver
	{
		public override bool CanResolve(ResolveParams rp)
		{
			return base.CanResolve(rp);
		}

		public override void Resolve(ResolveParams rp)
		{
			ThingDef stuff = rp.wallStuff ?? ThingDefOf.Steel;
			Rot4? thingRot = rp.thingRot;
			Rot4 rot = (thingRot == null) ? Rot4.East : thingRot.Value;
			if (rp.singleThingDef.rotatable)
			{
				if (rp.singleThingDef.MadeFromStuff)
				{
					Thing thing = ThingMaker.MakeThing(rp.singleThingDef, stuff);
					GenSpawn.Spawn(thing, rp.rect.RandomCell, BaseGen.globalSettings.map, rot, WipeMode.Vanish, false);
				}
				else
				{
					Thing thing2 = ThingMaker.MakeThing(rp.singleThingDef, null);
					GenSpawn.Spawn(thing2, rp.rect.RandomCell, BaseGen.globalSettings.map, rot, WipeMode.Vanish, false);
				}
			}
			else if (rp.singleThingDef.MadeFromStuff)
			{
				Thing thing3 = ThingMaker.MakeThing(rp.singleThingDef, stuff);
				GenSpawn.Spawn(thing3, rp.rect.RandomCell, BaseGen.globalSettings.map, rot, WipeMode.Vanish, false);
			}
			else
			{
				Thing thing4 = ThingMaker.MakeThing(rp.singleThingDef, null);
				GenSpawn.Spawn(thing4, rp.rect.RandomCell, BaseGen.globalSettings.map, rot, WipeMode.Vanish, false);
			}
		}
	}
}

