using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ReconAndDiscovery.Things
{
	public class HoloEmitter : Building, IThingHolder
	{
		public HoloEmitter()
		{
			this.innerContainer = new ThingOwner<Thing>(this, false, LookMode.Deep);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.Look<ThingOwner>(ref this.innerContainer, "innerContainer", new object[]
			{
				this
			});
		}

		public override void TickRare()
		{
			base.TickRare();
			if (this.innerContainer == null)
			{
				this.innerContainer = new ThingOwner<Thing>(this, false, LookMode.Deep);
			}
			IEnumerable<Thing> enumerable = from t in this.innerContainer
			where t is Corpse
			select t;
			foreach (Thing thing in enumerable)
			{
				Corpse corpse = thing as Corpse;
				if (corpse != null)
				{
					Pawn innerPawn = corpse.InnerPawn;
					if (innerPawn == null)
					{
						break;
					}
					if (innerPawn.Dead)
					{
						CompOsiris.Ressurrect(innerPawn, this);
						CompHoloEmitter comp = base.GetComp<CompHoloEmitter>();
						if (comp.SimPawn != innerPawn)
						{
							comp.SimPawn = innerPawn;
							comp.MakeValidAllowedZone();
						}
					}
				}
			}
		}

		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
		}

		public ThingOwner GetDirectlyHeldThings()
		{
			return this.innerContainer;
		}

		public virtual bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true)
		{
			bool result;
			if (!(thing is Corpse) && !(thing is Pawn))
			{
				result = false;
			}
			else
			{
				bool flag;
				if (thing.holdingOwner != null)
				{
					thing.holdingOwner.TryTransferToContainer(thing, this.innerContainer, thing.stackCount, true);
					flag = true;
				}
				else
				{
					flag = this.innerContainer.TryAdd(thing, true);
				}
				result = flag;
			}
			return result;
		}

		private ThingOwner innerContainer;
	}
}

