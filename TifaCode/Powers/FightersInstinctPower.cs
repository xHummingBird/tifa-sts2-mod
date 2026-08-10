using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Tifa.TifaCode.Powers;

public class FightersInstinctPower : TifaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power,
        decimal amount, Creature? creature, CardModel? cardSource)
    {
        if (power is not ChiPower)
            return;

        if (amount <= 0)
            return;

        Flash();

        var ownerPlayer = base.Owner.Player;
        if (ownerPlayer != null)
            await CardPileCmd.Draw(
                choiceContext,
                Amount,
                ownerPlayer);
    }
}