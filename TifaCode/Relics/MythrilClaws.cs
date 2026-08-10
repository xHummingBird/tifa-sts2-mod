using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Tifa.TifaCode.Powers;

namespace Tifa.TifaCode.Relics;


public class MythrilClaws() : TifaRelic //provide 3 block everytime you apply Chi
{
    public override RelicRarity Rarity => RelicRarity.Common;
    
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