using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ReconAndDiscovery.Triggers
{
	public class RectActionTrigger : ActionTrigger
	{
		public CellRect Rect
		{
			get
			{
				return this.rect;
			}
			set
			{
				this.rect = value;
				if (base.Spawned)
				{
					this.rect.ClipInsideMap(base.Map);
				}
			}
		}

		public override ICollection<IntVec3> Cells
		{
			get
			{
				return this.rect.Cells.ToList<IntVec3>();
			}
		}

		private CellRect rect;
	}
}

