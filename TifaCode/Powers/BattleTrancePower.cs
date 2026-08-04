using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Tifa.TifaCode.Relics;

namespace Tifa.TifaCode.Powers;

public class BattleTrancePower : TifaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        var comboRelic = Owner.Player.GetRelic<ComboRelicBase>();

        if (comboRelic != null)
        {
            comboRelic.LoseCombo(Amount * 5);
        }

        await PowerCmd.Remove(this);
    }
}