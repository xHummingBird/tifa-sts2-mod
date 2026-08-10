using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Tifa.TifaCode.Extensions;
using Tifa.TifaCode.Powers;
using Tifa.TifaCode.Relics;

namespace Tifa.TifaCode.Cards.Uncommon;

public class PressurePoint() : TifaCard(0, CardType.Attack,
    CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<VulnerablePower>(2m),
        new DynamicVar ("Combo", 2)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>(),
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        var tifa = Owner?.Character as Character.Tifa;
        
        if (ownerCreature != null && tifa != null)
        {
            AudioHelper.PlayRandomPhrase();
            var comboRelic =
                Owner.GetRelic<ComboRelicBase>();
            comboRelic?.GainCombo(DynamicVars["Combo"].IntValue);
        }
        
        await PowerCmd.Apply<VulnerablePower>(choiceContext, play.Target, DynamicVars.Vulnerable.BaseValue, base.Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Vulnerable.UpgradeValueBy(1m);
        DynamicVars["Combo"].UpgradeValueBy(1m);
    }
}