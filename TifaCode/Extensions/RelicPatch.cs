using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using Tifa.TifaCode.Cards.Ancient;
using Tifa.TifaCode.Cards.Basic;
using Tifa.TifaCode.Cards.Rare;
using Tifa.TifaCode.Relics;

namespace Tifa.TifaCode.Extensions;

[HarmonyPatch(typeof(TouchOfOrobas), "GetUpgradedStarterRelic")]
internal static class SquallTouchOfOrobasPatch
{
    private static void Postfix(RelicModel starterRelic, ref RelicModel __result)
    {
        if (starterRelic is LeatherGlove)
        {
            __result = ModelDb.Relic<PremiumHeart>().ToMutable();
        }
    }
}


[HarmonyPatch(typeof(ArchaicTooth), "TranscendenceUpgrades", MethodType.Getter)]
internal static class SquallArchaicToothTranscendencePatch
{
    [HarmonyPostfix]
    private static void Postfix(ref Dictionary<ModelId, CardModel> __result)
    {
        __result[ModelDb.Card<Divekick>().Id] = ModelDb.Card<Meteodrive>();
    }
}


[HarmonyPatch(typeof(DustyTome), nameof(DustyTome.AfterObtained))]
public static class DustyTomePatch
{
    [HarmonyPrefix]
    public static void Prefix(DustyTome __instance)
    {
        if (__instance.Owner?.Character is not Character.Tifa)
            return;
        
        __instance.AncientCard = ModelDb.Card<SynchroCyclone>().Id;
    }
}