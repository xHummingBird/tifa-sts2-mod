using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Tifa.TifaCode.Mechanics.Limit;
using Tifa.TifaCode.Powers;
using Tifa.TifaCode.Relics;

namespace Tifa.TifaCode.Cards.Ancient;

public class LimitBreak() : TifaCard(0, CardType.Skill,
    CardRarity.Ancient, TargetType.AnyEnemy), ILimitCard
{
    protected override bool ShouldGlowGoldInternal => IsPlayable;
    protected override bool IsPlayable => base.Owner.HasPower<LimitBreakPower>();
    protected override IEnumerable<DynamicVar> CanonicalVars => [
    ];
    
    private IEnumerable<CardModel> GetLimitBreakCards()
    {
        var pile = PileType.Hand.GetPile(base.Owner);
        return pile.Cards.OfType<LimitBreak>();
    }
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var rf = CombatState.CreateCard<RiseAndFall>(base.Owner);
        var df = CombatState.CreateCard<DolphinFlurry>(base.Owner);
        //var ms = CombatState.CreateCard<MeteorStrike>(base.Owner);
        //var fh = CombatState.CreateCard<FinalHeaven>(base.Owner);
        
        PremiumHeart? premiumHeart = base.Owner?.GetRelic<PremiumHeart>();

        if (base.IsUpgraded)
        {
            CardCmd.Upgrade(rf);
            CardCmd.Upgrade(df);
            //CardCmd.Upgrade(ms);
            //CardCmd.Upgrade(fh);
            
        }
        
        List<CardModel> cards = [rf, df];

        // if (base.Owner.Creature.GetPowerAmount<ChiPower>() >= 5)
        //     cards.Add(ms);
        // if (premiumHeart != null && base.Owner.Creature.GetPowerAmount<ChiPower>() >= 7)
        //     cards.Add(fh);
        
        foreach (var card in GetLimitBreakCards().ToList())
        {
            await CardCmd.Exhaust(choiceContext, card);
        }
        
        if (base.Owner.Creature.GetPowerAmount<ChiPower>() >= 3)
        {
            CardModel? cardModel = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards.ToList(), base.Owner, canSkip: false);
            LimitManager.SetLimit(base.Owner, 0);
            await PowerCmd.Remove<LimitBreakPower>(base.Owner.Creature);
            await CardCmd.AutoPlay(choiceContext, cardModel, play.Target);
        }

        else if (base.Owner.Creature.GetPowerAmount<ChiPower>() < 3)
        {
            LimitManager.SetLimit(base.Owner, 0);
            await PowerCmd.Remove<LimitBreakPower>(base.Owner.Creature);
            await CardCmd.AutoPlay(choiceContext, rf, play.Target);
        }
        // if (cardModel is MeteorStrike meteorStrike)
        //     await CardCmd.AutoPlay(choiceContext, cardModel, null);
        // else 
        
    }

    protected override void OnUpgrade()
    {
        
    }
}