using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Tifa.TifaCode.Powers;

public class PerfectBalancePower : TifaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        if (player != base.Owner.Player)
        {
            return amount;
        }
        return amount + (decimal)base.Amount;
    }
}