using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using Tifa.TifaCode.Mechanics.Limit;

namespace Tifa.TifaCode.Relics;

public class PurplePain() : TifaRelic //Start with 1 Chi
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    
    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side != base.Owner.Creature.Side)
            return;
        
        if (combatState.RoundNumber <= 1)
            Flash();
    }
}