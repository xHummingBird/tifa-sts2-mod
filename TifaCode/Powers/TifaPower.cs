using BaseLib.Abstracts;
using BaseLib.Extensions;
using Tifa.TifaCode.Extensions;
using Godot;

namespace Tifa.TifaCode.Powers;

public abstract class TifaPower : CustomPowerModel
{
    //Loads from Tifa/images/powers/your_power.png
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}