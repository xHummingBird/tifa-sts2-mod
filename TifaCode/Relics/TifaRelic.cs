using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Tifa.TifaCode.Character;
using Tifa.TifaCode.Extensions;
using Godot;

namespace Tifa.TifaCode.Relics;

[Pool(typeof(TifaRelicPool))]
public abstract class TifaRelic : CustomRelicModel
{
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();

    protected override string PackedIconOutlinePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".BigRelicImagePath();

    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
}