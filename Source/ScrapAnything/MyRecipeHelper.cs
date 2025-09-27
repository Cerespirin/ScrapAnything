using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Cerespirin.ScrapAnything
{
	internal static class MyRecipeHelper
	{
		// RimWorld version 1.5 added an additional check which totally destroyed the mod's functionality.
		// This check was probably added for a reason, so instead bring back the old version for our use.
		public static IEnumerable<Thing> SmeltProducts_Old(this Thing thisThing)
		{
			List<ThingDefCountClass> costListAdj = thisThing.def.CostListAdjusted(thisThing.Stuff);
			for (int j = 0; j < costListAdj.Count; j++)
			{
				if (!costListAdj[j].thingDef.intricate)
				{
					int num = GenMath.RoundRandom((float)costListAdj[j].count * 0.25f);
					if (num > 0)
					{
						Thing thing = ThingMaker.MakeThing(costListAdj[j].thingDef);
						thing.stackCount = num;
						yield return thing;
					}
				}
			}

			if (thisThing.def.smeltProducts != null)
			{
				for (int j = 0; j < thisThing.def.smeltProducts.Count; j++)
				{
					ThingDefCountClass thingDefCountClass = thisThing.def.smeltProducts[j];
					Thing thing2 = ThingMaker.MakeThing(thingDefCountClass.thingDef);
					thing2.stackCount = thingDefCountClass.count;
					yield return thing2;
				}
			}
			yield break;
		}

		public static bool HasSmeltProducts(this ThingDef def)
		{
			if (def.HasComp(typeof(CompRottable)) || (def.costList?.Any(c => c.thingDef.HasComp(typeof(CompRottable))) ?? false))
			{
				return false;
			}
			else if (def.MadeFromStuff)
			{
				return true;
			}
			else if (def.smeltProducts?.Any() ?? false)
			{
				return true;
			}
			else if (def.costList?.Where(c => c.thingDef.intricate).Any() ?? false)
			{
				return true;
			}
			return false;
		}
	}
}
