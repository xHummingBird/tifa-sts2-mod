using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Tifa.TifaCode.Powers;

namespace Tifa.TifaCode.Relics;

public class MetalKnuckle() : TifaRelic //Provide 3 Vigor everytime you apply Chi
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power,
        decimal amount, Creature? creature, CardModel? cardSource)
    {
        if (power is not ChiPower)
            return;

        if (amount <= 0)
            return;

        Flash();
    }
}