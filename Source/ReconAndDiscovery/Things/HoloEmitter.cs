using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ReconAndDiscovery.Things
{
    public class HoloEmitter : Building, IThingHolder
    {
        private ThingOwner innerContainer;

        public HoloEmitter()
        {
            innerContainer = new ThingOwner<Thing>(this, false);
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return innerContainer;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
        }

        public override void TickRare()
        {
            base.TickRare();
            if (innerContainer == null)
            {
                innerContainer = new ThingOwner<Thing>(this, false);
            }

            var enumerable = from t in innerContainer
                where t is Corpse
                select t;
            foreach (var thing in enumerable)
            {
                if (thing is not Corpse corpse)
                {
                    continue;
                }

                var innerPawn = corpse.InnerPawn;
                if (innerPawn == null)
                {
                    break;
                }

                if (!innerPawn.Dead)
                {
                    continue;
                }

                CompOsiris.Ressurrect(innerPawn, this);
                var comp = GetComp<CompHoloEmitter>();
                if (comp.SimPawn == innerPawn)
                {
                    continue;
                }

                comp.SimPawn = innerPawn;
                comp.MakeValidAllowedZone();
            }
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
                    thing.holdingOwner.TryTransferToContainer(thing, innerContainer, thing.stackCount);
                    flag = true;
                }
                else
                {
                    flag = innerContainer.TryAdd(thing);
                }

                result = flag;
            }

            return result;
        }
    }
}