using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Cerespirin.ScrapAnything.HarmonyPatches
{
	internal static class MyRecipeHelper
	{
		public static IEnumerable<Thing> SmeltProducts_Old(this Thing thisThing)
		{
			// RimWorld version 1.5 added an additional check which totally destroyed the mod's functionality.
			// This check was probably added for a reason, so instead bring back the old version for our use.
			List<ThingDefCountClass> costListAdj = thisThing.def.CostListAdjusted(thisThing.Stuff, true);
			int num2;
			for (int i = 0; i < costListAdj.Count; i = num2 + 1)
			{
				if (!costListAdj[i].thingDef.intricate)// && costListAdj[i].thingDef.smeltable)
				{
					int num = GenMath.RoundRandom((float)costListAdj[i].count * 0.25f);
					if (num > 0)
					{
						Thing thing = ThingMaker.MakeThing(costListAdj[i].thingDef, null);
						thing.stackCount = num;
						yield return thing;
					}
				}
				num2 = i;
			}
			if (thisThing.def.smeltProducts != null)
			{
				for (int i = 0; i < thisThing.def.smeltProducts.Count; i = num2 + 1)
				{
					ThingDefCountClass thingDefCountClass = thisThing.def.smeltProducts[i];
					Thing thing2 = ThingMaker.MakeThing(thingDefCountClass.thingDef, null);
					thing2.stackCount = thingDefCountClass.count;
					yield return thing2;
					num2 = i;
				}
			}
			yield break;
		}
	}
}
