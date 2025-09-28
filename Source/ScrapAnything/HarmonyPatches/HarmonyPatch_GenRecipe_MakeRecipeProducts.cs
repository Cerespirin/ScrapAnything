using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using System.Linq;

namespace Cerespirin.ScrapAnything
{
	[HarmonyPatch(typeof(GenRecipe), nameof(GenRecipe.MakeRecipeProducts))]
	public static class HarmonyPatch_GenRecipe_MakeRecipeProducts
	{
		public static IEnumerable<Thing> Postfix(IEnumerable<Thing> __result, RecipeDef recipeDef, Pawn worker, List<Thing> ingredients, Precept_ThingStyle precept = null, ThingStyleDef style = null, int? overrideGraphicIndex = null)
		{
			foreach (Thing thing in __result)
			{
				yield return thing;
			}

			if (recipeDef.HasModExtension<ScrapAnythingExt>())
			{
				MethodInfo postProcessProduct = typeof(GenRecipe).GetMethod("PostProcessProduct", BindingFlags.NonPublic | BindingFlags.Static);
				{
					foreach (Thing product in ingredients.SelectMany(ingredient => ingredient.SmeltProducts_Old()))
					{
						yield return (Thing)postProcessProduct.Invoke(null, new object[] { product, recipeDef, worker, precept, style, overrideGraphicIndex });
					}
				}
			}
		}
	}
}
