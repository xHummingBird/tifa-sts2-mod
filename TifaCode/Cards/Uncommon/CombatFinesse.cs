using BaseLib.Extensions;
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

public class CombatFinesse() : TifaCard(
    1,
    CardType.Power,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override bool ShouldGlowGoldInternal => IsPlayable;
    protected override bool IsPlayable => base.Owner.Creature.GetPowerAmount<ChiPower>() >= 2;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ChiPower>(),
        HoverTipFactory.FromPower<DexterityPower>(),
        HoverTipFactory.FromPower<StrengthPower>()
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        AudioHelper.PlayRandomChiBuff();
        await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner.Creature, 2, base.Owner.Creature, this);
        await PowerCmd.Apply<DexterityPower>(choiceContext, base.Owner.Creature, 2, base.Owner.Creature, this);
        
    }
    
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}