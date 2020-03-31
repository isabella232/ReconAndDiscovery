using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace ReconAndDiscovery
{
	public class GameCondition_Radiation : GameCondition
	{
		public GameCondition_Radiation()
		{
			Color sky = new Color(0.8f, 0.8f, 0.3f);
			Color shadow = new Color(0.9f, 0.9f, 1f);
			Color overlay = new Color(0.7f, 0.7f, 0.5f);
			this.SkyColours = new SkyColorSet(sky, shadow, overlay, 9f);
		}

		public override void End()
		{
			base.End();
		}

		public override void ExposeData()
		{
			base.ExposeData();
		}

		private void AssignRadiationSickness(Pawn p)
		{
			if (p.Faction == Faction.OfPlayer)
			{
				Messages.Message("RD_DevelopedRadiationSickness".Translate(p.Named("PAWN"))//"{0} has developed radiation sickness" 
, MessageTypeDefOf.NegativeEvent, false);
				p.health.AddHediff(HediffDef.Named("RD_RadiationSickness"), null, null);
			}
		}

		private void GiveCarcinoma(Pawn p)
		{
			if (p.RaceProps.IsFlesh)
			{
				List<BodyPartRecord> allParts = p.RaceProps.body.AllParts;
				BodyPartDef partDef = allParts.RandomElement<BodyPartRecord>().def;
				if (p.RaceProps.body == BodyDefOf.Human)
				{
					float value = Rand.Value;
					if (value < 0.1f)
					{
						partDef = DefDatabase<BodyPartDef>.GetNamed("Lung", true);
					}
					else if (value < 0.2f)
					{
						partDef = DefDatabase<BodyPartDef>.GetNamed("Lung", true);
					}
					else if (value < 0.4f)
					{
						partDef = BodyPartDefOf.Stomach;
					}
					else if (value < 0.6f)
					{
						partDef = BodyPartDefOf.Liver;
					}
					else if (value < 0.8f)
					{
						partDef = BodyPartDefOf.Brain;
					}
				}
				IEnumerable<BodyPartRecord> source = from part in allParts
				where part.def == partDef
				select part;
				if (source.Count<BodyPartRecord>() != 0)
				{
					BodyPartRecord bodyPartRecord = source.RandomElement<BodyPartRecord>();
					if (allParts.Contains(bodyPartRecord))
					{
						if (!p.health.hediffSet.PartIsMissing(bodyPartRecord))
						{
							p.health.AddHediff(HediffDef.Named("Carcinoma"), bodyPartRecord, null);
							Log.Message(string.Format("Added carcinoma to {0}, part {1}",
                                p.Label, bodyPartRecord.def.label));
						}
					}
				}
			}
		}

		private void Miscarry(Pawn pawn)
		{
			if (pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Pregnant, true) != null)
			{
				if (pawn.Faction == Faction.OfPlayer)
				{
					
					Messages.Message("RD_MiscarriedRadiation".Translate(pawn.Named("PAWN")) //has miscarried due to radiation poisoning.
, pawn, MessageTypeDefOf.NegativeEvent);
				}
			}
		}

		private bool IsProtectedAt(Map map, IntVec3 c)
		{
            List<Map> affectedMaps = base.AffectedMaps;
            bool result;
            Room room = GridsUtility.GetRoom(c, map, RegionType.Set_Passable);
            if (room == null)
            {
                result = false;
            }
            else if (room.PsychologicallyOutdoors)
            {
                result = false;
            }
            else
            {
                foreach (IntVec3 c2 in room.Cells)
                {
                    if (!c2.Roofed(map))
                    {
                        return false;
                    }
                    if (c2.GetRoof(map) != RoofDefOf.RoofRockThick)
                    {
                        return false;
                    }
                }
                result = true;
            }
            return result;
		}

		public override void GameConditionTick()
		{
            List<Map> affectedMaps = base.AffectedMaps;
            foreach (Map map in affectedMaps)
            {
                if (Rand.Chance(0.006666667f))
                {
                    List<Thing> list = map.listerThings.ThingsInGroup(ThingRequestGroup.Plant);
                    if (list.Count != 0)
                    {
                        Plant plant = list.RandomElement<Thing>() as Plant;
                        if (plant != null)
                        {
                            if (!this.IsProtectedAt(map, plant.Position))
                            {
                                if (plant.def != ThingDef.Named("Plant_Psychoid"))
                                {
                                    plant.CropBlighted();
                                    if (plant.sown)
                                    {
	
                                        Messages.Message("RD_PlantDiedRatiation".Translate() //A plant has died due to radiation damage"
                                            , MessageTypeDefOf.NegativeEvent);
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (Pawn pawn in map.mapPawns.AllPawnsSpawned)
                {
                    if (!this.IsProtectedAt(map, pawn.Position))
                    {
                        float chance = 0.14f * pawn.GetStatValue(StatDefOf.ToxicSensitivity, true) / 60000f;
                        float chance2 = 0.04f * pawn.GetStatValue(StatDefOf.ToxicSensitivity, true) / 60000f;
                        if (Rand.Chance(chance))
                        {
                            this.AssignRadiationSickness(pawn);
                        }
                        if (Rand.Chance(chance2))
                        {
                            this.GiveCarcinoma(pawn);
                        }
                        if (Rand.Chance(chance) && pawn.health.hediffSet.HasHediff(HediffDefOf.Pregnant))
                        {
                            this.Miscarry(pawn);
                        }
                    }
                }
            }

		}

		public override void Init()
		{
			base.Init();
		}

		public override SkyTarget? SkyTarget(Map map)
		{
			return new SkyTarget?(new SkyTarget(0.1f, this.SkyColours, 1f, 1f));
		}

		public override float SkyTargetLerpFactor(Map map)
		{
			return GameConditionUtility.LerpInOutValue((float)base.TicksPassed, (float)base.TicksLeft, 2500f, 0.25f);
		}

		private SkyColorSet SkyColours;
	}
}

