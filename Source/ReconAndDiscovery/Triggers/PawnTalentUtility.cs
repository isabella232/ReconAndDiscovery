using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ReconAndDiscovery.Triggers
{
	public static class PawnTalentUtility
	{
		public static Pawn FindBestPsychic(IEnumerable<IntVec3> within, Map map)
		{
			Pawn pawn = null;
			foreach (IntVec3 c in within)
			{
				foreach (Thing thing in c.GetThingList(map))
				{
					if (thing.def.category == ThingCategory.Pawn && thing.def.race.intelligence == Intelligence.Humanlike)
					{
						Pawn pawn2 = thing as Pawn;
						int num = pawn2.story.traits.DegreeOfTrait(TraitDef.Named("PsychicSensitivity"));
						if (num > 0)
						{
							if (pawn == null)
							{
								pawn = pawn2;
							}
							else if (pawn.story.traits.DegreeOfTrait(TraitDef.Named("PsychicSensitivity")) < num)
							{
								pawn = pawn2;
							}
						}
					}
				}
			}
			return pawn;
		}

		public static Pawn FindBest(IEnumerable<IntVec3> within, Map map, SkillDef skill)
		{
			List<Pawn> list = new List<Pawn>();
			foreach (IntVec3 c in within)
			{
				foreach (Thing thing in c.GetThingList(map))
				{
					if (thing.def.category == ThingCategory.Pawn && thing.def.race.intelligence == Intelligence.Humanlike)
					{
						list.Add(thing as Pawn);
					}
				}
			}
			Pawn result;
			if (list.NullOrEmpty<Pawn>())
			{
				result = null;
			}
			else
			{
				list.SortByDescending((Pawn p) => p.skills.GetSkill(skill).Level);
				result = list.FirstOrDefault<Pawn>();
			}
			return result;
		}
	}
}

