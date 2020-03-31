using System;
using System.Collections.Generic;
using System.Linq;
using ReconAndDiscovery.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery
{
	public class WorkGiver_LoadIntoEmitter : WorkGiver_Scanner
	{
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.ClosestTouch;
			}
		}

		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForDef(ThingDef.Named("RD_HoloDisk"));
			}
		}

		private HoloEmitter FindEmitter(Pawn p, Thing corpse)
		{
			IEnumerable<ThingDef> enumerable = from def in DefDatabase<ThingDef>.AllDefs
			where typeof(HoloEmitter).IsAssignableFrom(def.thingClass)
			select def;
			foreach (ThingDef singleDef in enumerable)
			{
				Predicate<Thing> predicate = (Thing x) => ((HoloEmitter)x).GetComp<CompHoloEmitter>().SimPawn == null && ReservationUtility.CanReserve(p, x, 1, -1, null, false);
				HoloEmitter holoEmitter = (HoloEmitter)GenClosest.ClosestThingReachable(p.Position, p.Map, ThingRequest.ForDef(singleDef), PathEndMode.InteractionCell, TraverseParms.For(p, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, predicate, null, 0, -1, false, RegionType.Set_Passable, false);
				if (holoEmitter != null)
				{
					return holoEmitter;
				}
			}
			return null;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Job result;
			if (t.def.defName != "RD_HoloDisk")
			{
				result = null;
			}
			else if (!ReservationUtility.CanReserveAndReach(pawn, t, PathEndMode.Touch, Danger.Deadly, 1, 1, null, false))
			{
				result = null;
			}
			else
			{
				HoloEmitter holoEmitter = this.FindEmitter(pawn, t);
				if (holoEmitter == null)
				{
					result = null;
				}
				else if (holoEmitter.GetComp<CompHoloEmitter>().SimPawn != null)
				{
					result = null;
				}
				else
				{
					result = new Job(JobDefOfReconAndDiscovery.RD_LoadIntoEmitter, t, holoEmitter)
					{
						count = 1
					};
				}
			}
			return result;
		}
        public override bool ShouldSkip(Pawn pawn, bool forced = false)
		{
			return pawn.Map.listerThings.ThingsOfDef(ThingDef.Named("RD_HoloDisk")).Count == 0;
		}
	}
}

