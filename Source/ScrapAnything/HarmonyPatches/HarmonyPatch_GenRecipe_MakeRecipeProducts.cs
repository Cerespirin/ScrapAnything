using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Cerespirin.ScrapAnything.HarmonyPatches
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

				//int num;
				//for (int i = 0; i < recipeDef.specialProducts.Count; i = num + 1)
				foreach (SpecialProductType specialProductType in recipeDef.specialProducts)
				{
					//for (int j = 0; j < ingredients.Count; j = num + 1)
					foreach (Thing thing3 in ingredients)
					{
						//Thing thing3 = ingredients[j];
						//SpecialProductType specialProductType = recipeDef.specialProducts[i];
						//if (specialProductType != SpecialProductType.Butchery)
						{
							//if (specialProductType == SpecialProductType.Smelted)
							{
								foreach (Thing product in thing3.SmeltProducts_Old())
								{
									//yield return GenRecipe.PostProcessProduct(product, recipeDef, worker, precept, style, overrideGraphicIndex);
									yield return (Thing)postProcessProduct.Invoke(null, new object[] { product, recipeDef, worker, precept, style, overrideGraphicIndex });
								}
							}
						}
						//num = j;
					}
					//num = i;
				}
			}
		}
	}
}
