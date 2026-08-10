using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Tifa.TifaCode.Extensions;
using Tifa.TifaCode.Powers;

namespace Tifa.TifaCode.Cards.Uncommon;

public class DefensiveStance() : TifaCard(2, CardType.Skill,
    CardRarity.Uncommon, TargetType.Self)
{
    protected override bool ShouldGlowGoldInternal => (Owner.Creature.GetPowerAmount<ChiPower>() >= 2);
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new BlockVar(10m, ValueProp.Move),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ChiPower>(),
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        AudioHelper.PlayRandomDefend();
        decimal amount = await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        if (Owner.Creature.GetPowerAmount<ChiPower>() >= 2)
            await PowerCmd.Apply<BlockNextTurnPower>(choiceContext, base.Owner.Creature, amount, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(3m);
    }
}