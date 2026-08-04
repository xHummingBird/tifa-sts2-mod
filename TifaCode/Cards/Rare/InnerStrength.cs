using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using Tifa.TifaCode.Powers;

namespace Tifa.TifaCode.Cards.Rare;

public class InnerStrength() : TifaCard(1, CardType.Power,
    CardRarity.Rare, TargetType.Self)
{
    protected override bool ShouldGlowGoldInternal =>
        Owner.Creature.HasPower<ChiPower>();
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ChiPower>(),
        HoverTipFactory.FromPower<StrengthPower>(),
        
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)

    {
       decimal amount = Owner.Creature.GetPowerAmount<ChiPower>();
       
       await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner.Creature, amount, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}