using System;
using System.Collections.Generic;
using System.Linq;
using ReconAndDiscovery.Triggers;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReconAndDiscovery
{
	public class CompComputerTerminal : ThingComp
	{
		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			List<FloatMenuOption> list = base.CompFloatMenuOptions(selPawn).ToList<FloatMenuOption>();
			if (this.actionDef != null && this.parent.GetComp<CompPowerTrader>().PowerOn)
			{
                list.Add(new FloatMenuOption("RD_UseComputer".Translate(), delegate ()
                    {
                        selPawn.jobs.TryTakeOrderedJob(this.UseComputerJob(), JobTag.Misc);
                    }));
			}
			return list;
		}

		public Job UseComputerJob()
		{
			return new Job(JobDefOfReconAndDiscovery.RD_UseComputer, this.parent);
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Defs.Look<ActivatedActionDef>(ref this.actionDef, "actionDef");
		}

		public ActivatedActionDef actionDef;
	}
}

