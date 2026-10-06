using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Tifa.TifaCode.Powers;

public class FightingSpiritPower : TifaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(base.Owner))
        {
            return;
        }
        CardPile pile = PileType.Discard.GetPile(base.Owner.Player);
        IEnumerable<CardModel> source = pile.Cards.Where((CardModel c) => c.Type == CardType.Attack);
        IEnumerable<CardModel> enumerable = source.ToList().UnstableShuffle(base.Owner.Player.RunState.Rng.CombatCardSelection).Take(base.Amount);
        foreach (CardModel card in enumerable)
        {
            await CardPileCmd.Add(card, PileType.Hand);
            if (card.IsUpgradable)
            {
                CardCmd.Upgrade(card);
            }
        }
    }
}