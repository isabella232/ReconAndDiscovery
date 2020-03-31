using System;
using RimWorld;
using Verse;

namespace ReconAndDiscovery
{
	public class OsirisCasket : Building_CryptosleepCasket
	{
		public Corpse Corpse
		{
			get
			{
				for (int i = 0; i < this.innerContainer.Count; i++)
				{
					Corpse corpse = this.innerContainer[i] as Corpse;
					if (corpse != null)
					{
						return corpse;
					}
				}
				return null;
			}
		}
	}
}

