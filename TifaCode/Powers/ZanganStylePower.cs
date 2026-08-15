using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace Tifa.TifaCode.Powers;

public class ZanganStylePower : TifaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? creature, CardModel? cardSource)
    {
        if (power.Owner != base.Owner)
            return;
        
        if (power is not ChiPower)
            return;

        if (amount <= 0)
            return;
        
        Flash();

        //Snapshot the list; enemies may die mid-loop.
        var enemies = base.CombatState.HittableEnemies.ToList();

        foreach (var enemy in enemies)
        {
            if (!enemy.IsAlive)
                continue;
            var vfx = NGroundFireVfx.Create(enemy);
            if (vfx != null)
            {
                NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(vfx);
            }
            SfxCmd.Play("event:/sfx/characters/attack_fire");
            await CreatureCmd.Damage(
                choiceContext,
                enemy,
                base.Amount,
                ValueProp.Unpowered,
                null,
                null);
        }
    }
}