using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Tifa.TifaCode.Relics;

public sealed class LeatherGlove : ComboRelicBase
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override int MaxCombo => 30;
}