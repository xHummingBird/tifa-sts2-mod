using MegaCrit.Sts2.Core.Entities.Powers;

namespace Tifa.TifaCode.Powers;

public class UnbridledStrengthPower : TifaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
}