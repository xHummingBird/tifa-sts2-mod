using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Tifa.TifaCode.Powers;

public class UnfetteredFuryPower : TifaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(base.Owner))
        {
            await PowerCmd.Apply<DoubleDamagePower>(new ThrowingPlayerChoiceContext(), base.Owner, base.Amount, base.Owner, null);
            await PowerCmd.Remove(this);
        }
    }
}