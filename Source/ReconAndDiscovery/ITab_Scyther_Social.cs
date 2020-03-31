using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace ReconAndDiscovery
{
	public class ITab_Scyther_Social : ITab
	{
		public ITab_Scyther_Social()
		{
			this.size = new Vector2(540f, 510f);
			this.labelKey = "TabSocial".Translate();
			this.tutorTag = "RD_Social".Translate();
		}

		public override bool IsVisible
		{
			get
			{
				return this.SelPawnForSocialInfo.RaceProps.IsMechanoid && this.SelPawnForSocialInfo.RaceProps.intelligence == Intelligence.Humanlike;
			}
		}

		private Pawn SelPawnForSocialInfo
		{
			get
			{
				Pawn result;
				if (base.SelPawn != null)
				{
					result = base.SelPawn;
				}
				else
				{
					Corpse corpse = base.SelThing as Corpse;
					if (corpse == null)
					{
						throw new InvalidOperationException("Social tab on non-pawn non-corpse " + base.SelThing);
					}
					result = corpse.InnerPawn;
				}
				return result;
			}
		}

		protected override void FillTab()
		{
			SocialCardUtility.DrawSocialCard(new Rect(0f, 0f, this.size.x, this.size.y), this.SelPawnForSocialInfo);
		}

		public const float Width = 540f;
	}
}

