using System;
using System.Collections.Generic;
using System.Linq;
using ReconAndDiscovery.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery
{
	public class WorkGiver_ScanAtEmitter : WorkGiver_Scanner
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
				return ThingRequest.ForGroup(ThingRequestGroup.Corpse);
			}
		}

		private HoloEmitter FindEmitter(Pawn p, Corpse corpse)
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
			Corpse corpse = t as Corpse;
			Job result;
			if (corpse != null)
			{
				if (corpse.InnerPawn.Faction != Faction.OfPlayer || !corpse.InnerPawn.RaceProps.Humanlike)
				{
					result = null;
				}
				else if (!HaulAIUtility.PawnCanAutomaticallyHaul(pawn, t, forced))
				{
					result = null;
				}
				else
				{
					HoloEmitter holoEmitter = this.FindEmitter(pawn, corpse);
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
						result = new Job(JobDefOfReconAndDiscovery.RD_ScanAtEmitter, t, holoEmitter)
						{
							count = corpse.stackCount
						};
					}
				}
			}
			else
			{
				result = null;
			}
			return result;
		}

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
		{
			return pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse).Count == 0;
		}
	}
}

