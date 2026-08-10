using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Tifa.TifaCode.Cards.Ancient;
using Tifa.TifaCode.Extensions;
using Tifa.TifaCode.Mechanics.Limit;
using Tifa.TifaCode.Powers;
using Tifa.TifaCode.Relics;

namespace Tifa.TifaCode.Cards.Rare;

public class ShiftMomentum() : TifaCard(1, CardType.Skill,
    CardRarity.Rare, TargetType.Self)
{
    protected override bool ShouldGlowGoldInternal => base.Owner.Creature.CurrentHp * 2 < base.Owner.Creature.MaxHp;
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    
    private IEnumerable<CardModel> GetLimitBreakCards()
    {
        var pile = PileType.Hand.GetPile(base.Owner);
        return pile.Cards.OfType<LimitBreak>();
    }
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<LimitBreakPower>(),
        HoverTipFactory.FromPower<ChiPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        AudioHelper.PlayRandomChiBuff();
        int combo = LimitManager.GetLimit(base.Owner.Creature.Player);
        var comboRelic =
            Owner.GetRelic<ComboRelicBase>();
        comboRelic?.GainCombo(combo);
        
        LimitManager.SetLimit(base.Owner.Creature.Player, 0);
        if (base.Owner.Creature.HasPower<LimitBreakPower>())
        {
            await PowerCmd.Remove<LimitBreakPower>(base.Owner.Creature);
            foreach (var card in GetLimitBreakCards().ToList())
            {
                await CardCmd.Exhaust(choiceContext, card);
            }
        }

    }
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}