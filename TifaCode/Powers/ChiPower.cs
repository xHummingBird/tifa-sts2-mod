using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Tifa.TifaCode.Powers;

public sealed class ChiPower : TifaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    private int StrengthBonus
    {
        get
        {
            int bonus = 0;

            // Chi 1: +1 Strength
            if (base.Amount >= 3)
                bonus += 1;

            // Chi 6: +1 Strength
            if (base.Amount >= 6)
                bonus += 1;

            // Chi 8: +1 Strength
            if (base.Amount >= 8)
                bonus += 1;

            // Chi 9 onwards: +1 Strength per extra Chi level
            if (base.Amount >= 9)
                bonus += base.Amount - 8;

            return bonus;
        }
    }

    private int DexterityBonus
    {
        get
        {
            int bonus = 0;

            // Chi 3: +1 Dexterity
            if (base.Amount >= 4)
                bonus += 1;

            // Chi 6: +1 Dexterity
            if (base.Amount >= 6)
                bonus += 1;

            // Chi 8: +1 Dexterity
            if (base.Amount >= 8)
                bonus += 1;

            // Chi 9 onwards: +1 Dexterity per extra Chi level
            if (base.Amount >= 9)
                bonus += base.Amount - 8;

            return bonus;
        }
    }

    private int DrawBonus => base.Amount >= 5 ? 1 : 0;

    private int EnergyBonus => base.Amount >= 7 ? 1 : 0;

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (base.Owner != dealer)
            return 0m;

        if (!props.IsPoweredAttack())
            return 0m;

        return StrengthBonus;
    }

    public override decimal ModifyBlockAdditive(
        Creature target,
        decimal block,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (cardSource != null)
        {
            if (cardSource.Owner.Creature != base.Owner)
                return 0m;
        }
        else if (base.Owner != target)
        {
            return 0m;
        }

        if (!props.IsPoweredCardOrMonsterMoveBlock())
            return 0m;

        return DexterityBonus;
    }

    public override decimal ModifyHandDraw(
        Player player,
        decimal count)
    {
        if (player != base.Owner.Player)
            return count;

        return count + DrawBonus;
    }

    public override decimal ModifyMaxEnergy(
        Player player,
        decimal amount)
    {
        if (player != base.Owner.Player)
            return amount;

        return amount + EnergyBonus;
    }
}