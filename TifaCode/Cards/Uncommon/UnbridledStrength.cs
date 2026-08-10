using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Tifa.TifaCode.Extensions;
using Tifa.TifaCode.Powers;
using Tifa.TifaCode.Relics;

namespace Tifa.TifaCode.Cards.Uncommon;

public class UnbridledStrength() : TifaCard(1, CardType.Power, //Gain 1 Chi level. Exhaust. 0 cost. Innate for upgrade
    CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Combo", 5)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ChiPower>(),
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var comboRelic =
            Owner.GetRelic<ComboRelicBase>();
        AudioHelper.PlayRandomAttackHard();
        comboRelic?.GainCombo(DynamicVars["Combo"].IntValue);
        await PowerCmd.Apply<UnbridledStrengthPower>(
            choiceContext,
            base.Owner.Creature,
            1,
            base.Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
} 