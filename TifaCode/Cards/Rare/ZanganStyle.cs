using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Tifa.TifaCode.Powers;

namespace Tifa.TifaCode.Cards.Rare;

public class ZanganStyle() : TifaCard(
    1,
    CardType.Power,
    CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ZanganStylePower>(1m),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await PowerCmd.Apply<ZanganStylePower>(
            choiceContext,
            base.Owner.Creature,
            base.DynamicVars["ZanganStylePower"].BaseValue,
            base.Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ZanganStylePower"].UpgradeValueBy(1);
    }
}