using System;
using RimWorld;
using Verse;

namespace ReconAndDiscovery
{
	public class CompMandatoryMilkable : CompHasGatherableBodyResource
	{
		public bool Overfull
		{
			get
			{
				return this.ticksOverFull * 2L >= (long)this.Props.ticksUntilDanger;
			}
		}

		protected override bool Active
		{
			get
			{
				bool result;
				if (!base.Active)
				{
					result = false;
				}
				else
				{
					Pawn pawn = this.parent as Pawn;
					result = ((!this.Props.milkFemaleOnly || pawn == null || pawn.gender == Gender.Female) && (pawn == null || pawn.ageTracker.CurLifeStage.milkable));
				}
				return result;
			}
		}

		public CompProperties_MandatoryMilkable Props
		{
			get
			{
				return (CompProperties_MandatoryMilkable)this.props;
			}
		}

		protected override int GatherResourcesIntervalDays
		{
			get
			{
				return this.Props.milkIntervalDays;
			}
		}

		protected override int ResourceAmount
		{
			get
			{
				return this.Props.milkAmount;
			}
		}

		protected override ThingDef ResourceDef
		{
			get
			{
				return this.Props.milkDef;
			}
		}

		protected override string SaveKey
		{
			get
			{
				return "milkFullness";
			}
		}

		public override void CompTick()
		{
			base.CompTick();
			if (base.Fullness >= 1f)
			{
				this.ticksOverFull += 1L;
			}
			else
			{
				this.ticksOverFull = 0L;
			}
			if (this.ticksOverFull > (long)this.Props.ticksUntilDanger)
			{
				if (Rand.Chance(2.5E-05f))
				{
					DamageInfo value = new DamageInfo(DamageDefOf.Bomb, 100, -1f, -1f, null, null, null, 0);
					this.parent.Kill(new DamageInfo?(value));
				}
			}
		}

		public override string CompInspectStringExtra()
		{
			string result;
			if (!this.Active)
			{
				result = null;
			}
			else
			{
				string text = "MilkFullness".Translate() + ": " + base.Fullness.ToStringPercent();
				if ((double)this.ticksOverFull > 0.33 * (double)this.Props.ticksUntilDanger)
				{
					text += "\n" + "RD_Overfull".Translate(); //"Overfull!";
				}
				else if ((float)this.ticksOverFull > 0.67f * (float)this.Props.ticksUntilDanger)
				{
					text += "\n" + "RD_DangrouslyOverfull".Translate(); //Dangrously Overfull!
				}
				result = text;
			}
			return result;
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<long>(ref this.ticksOverFull, "ticksOverFull", 0L, false);
		}

		private long ticksOverFull;
	}
}

