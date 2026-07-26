using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace Tifa.TifaCode.Extensions;

public static class TifaStaticHoverTip
{
    public static readonly IHoverTip Limit = new HoverTip(
        new LocString("static_hover_tips", "TIFA_LIMIT.title"),
        new LocString("static_hover_tips", "TIFA_LIMIT.description")
    );
    
    public static readonly IHoverTip Chi = new HoverTip(
        new LocString("static_hover_tips", "TIFA_CHI.title"),
        new LocString("static_hover_tips", "TIFA_CHI.description")
    );
    
    public static readonly IHoverTip Combo = new HoverTip(
        new LocString("static_hover_tips", "TIFA_COMBO.title"),
        new LocString("static_hover_tips", "TIFA_COMBO.description")
    );
}