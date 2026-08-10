using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Tifa.TifaCode.Powers;

namespace Tifa.TifaCode.Cards.Uncommon;

public class FightersInstinct() : TifaCard(
    2,
    CardType.Power,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<FightersInstinctPower>(1m),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await PowerCmd.Apply<FightersInstinctPower>(
            choiceContext,
            base.Owner.Creature,
            base.DynamicVars["FightersInstinctPower"].BaseValue,
            base.Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["FightersInstinctPower"].UpgradeValueBy(1);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}