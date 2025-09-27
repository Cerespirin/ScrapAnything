using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace Cerespirin.ScrapAnything
{
	[StaticConstructorOnStartup]
	public static class MyStaticConstructor
	{
		static MyStaticConstructor()
		{
			Harmony harmony = new Harmony("rimworld.cerespirin.scrapanything");
			harmony.PatchAll();

			IEnumerable<ThingDef> workTables = DefDatabase<ThingDef>.AllDefs.Where(t => t.IsWorkTable);
			FieldInfo allRecipesCached = typeof(ThingDef).GetField("allRecipesCached", BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (ThingDef workTable in workTables)
			{
				IEnumerable<RecipeDef> tableRecipes = workTable.AllRecipes.Where(r => r.ProducedThingDef?.HasSmeltProducts() ?? false);
				if (!tableRecipes.Any()) { continue; }

				ThingFilter newFilter = new ThingFilter();
				foreach (ThingDef thingDef in tableRecipes.Select(r => r.ProducedThingDef)) { newFilter.SetAllow(thingDef, true); }

				IngredientCount newCount = new IngredientCount();
				newCount.SetBaseCount(1);
				newCount.filter = newFilter;

				RecipeDef generatedRecipe = new RecipeDef
				{
					defName = "ScrapAnything_DisassembleAt" + workTable.defName,
					label = "ScrapAnything_BillLabel".Translate(),
					description = "ScrapAnything_BillDesc".Translate(),
					jobString = "ScrapAnything_BillJob".Translate(workTable.label),
					workAmount = RecipeDefOf.SmeltOrDestroyThing.smeltingWorkAmount,
					workSpeedStat = MyDefOf.SmeltingSpeed,
					effectWorking = tableRecipes.GroupBy(r => r.effectWorking).OrderByDescending(g => g.Count()).Select(o => o.Key).First(),
					soundWorking = tableRecipes.GroupBy(r => r.soundWorking).OrderByDescending(g => g.Count()).Select(o => o.Key).First(),
					//specialProducts = new List<SpecialProductType> { SpecialProductType.Smelted },
					modExtensions = new List<DefModExtension> { new ScrapAnythingExt() },
					recipeUsers = new List<ThingDef> { workTable },
					ingredients = new List<IngredientCount> { newCount },
					fixedIngredientFilter = newFilter,
					forceHiddenSpecialFilters = new List<SpecialThingFilterDef>
					{
						MyDefOf.AllowBurnableApparel,
						MyDefOf.AllowBurnableWeapons,
						MyDefOf.AllowNonBurnableApparel,
						MyDefOf.AllowNonBurnableWeapons,
						MyDefOf.AllowNonSmeltableApparel,
						MyDefOf.AllowNonSmeltableWeapons,
						MyDefOf.AllowSmeltable,
						MyDefOf.AllowSmeltableApparel,
					}
				};
				generatedRecipe.ResolveReferences();
				DefDatabase<RecipeDef>.Add(generatedRecipe);
				// Clear the recipe cache because we've added a new recipe.
				allRecipesCached.SetValue(workTable, null);
			}
		}
	}
}
