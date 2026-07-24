using BaseLib.Abstracts;
using Tifa.TifaCode.Extensions;
using Godot;

namespace Tifa.TifaCode.Character;

public class TifaRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Tifa.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}