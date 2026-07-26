using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Tifa.TifaCode.Relics;

public sealed class PremiumHeart : ComboRelicBase
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override int MaxCombo => 40;

    protected override int ModifyComboGain(CardModel card, int amount)
    {
        if (amount <= 0)
            return amount;

        if (card.Type == CardType.Attack)
            return amount + 1;

        return amount;
    }
}