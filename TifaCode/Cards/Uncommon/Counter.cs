using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Tifa.TifaCode.Extensions;
using Tifa.TifaCode.Relics;

namespace Tifa.TifaCode.Cards.Uncommon;

public class Counter() : TifaCard(1, CardType.Skill,
    CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ThornsPower>(),
        
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ThornsPower>(3m),
        new DynamicVar("Combo", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        
        AudioHelper.PlayRandomDefend();
        await PowerCmd.Apply<ThornsPower>(choiceContext, base.Owner.Creature, DynamicVars["ThornsPower"].BaseValue, base.Owner.Creature, this);
        var comboRelic =
            Owner.GetRelic<ComboRelicBase>();
        comboRelic?.GainCombo(DynamicVars["Combo"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ThornsPower"].UpgradeValueBy(1m);
        DynamicVars["Combo"].UpgradeValueBy(1);
    }
}