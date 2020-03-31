using System;
using System.Collections.Generic;
using Verse;

namespace ReconAndDiscovery.Triggers
{
	public class ActivatedActionDef : Def
	{
		public ActivatedAction ActivatedAction
		{
			get
			{
				if (this.actionObject == null)
				{
					this.actionObject = (ActivatedAction)Activator.CreateInstance(this.activatedActionClass);
					this.actionObject.def = this;
				}
				return this.actionObject;
			}
		}

		public IntRange numThingsToSpawn;

		public IntRange spawnedThingStackSize;

		public List<ThingDef> spawnedThings = new List<ThingDef>();

		public IntRange numPawnsToSpawn;

		public List<PawnKindDef> pawnKinds;

		public bool hasDialog = false;

		public string dialogText = "";

		public Type activatedActionClass;

		public int effectRadius = 0;

		private ActivatedAction actionObject;
	}
}

