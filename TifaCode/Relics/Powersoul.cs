using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Tifa.TifaCode.Relics;

public class Powersoul() : TifaRelic //Start combat with 20 additional Limit
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    
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