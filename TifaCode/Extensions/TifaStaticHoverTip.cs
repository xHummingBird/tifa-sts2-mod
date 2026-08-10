using Godot;
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

public static class TifaHoverTipText
{
    public const string ChiToken = "%CHI%";

    public const string MaxChiToken = "%MAXCHI%";

    private static readonly StringName TextProperty = "text";

    public sealed class TextTarget
    {
        public required GodotObject TextNode { get; init; }

        public required string Template { get; init; }
    }

    /*
     * Snapshots every label under the tip whose text still contains a token,
     * so the values can be re-rendered later without losing the template.
     */
    public static List<TextTarget> CollectTargets(Node root)
    {
        List<TextTarget> targets = [];

        Collect(root, targets);

        return targets;
    }

    public static void Render(
        List<TextTarget> targets,
        int chi,
        int maxChi)
    {
        foreach (var target in targets)
        {
            if (!GodotObject.IsInstanceValid(target.TextNode))
                continue;

            string text =
                target.Template
                    .Replace(ChiToken, chi.ToString())
                    .Replace(MaxChiToken, maxChi.ToString());

            target.TextNode.Set(TextProperty, text);
        }
    }

    private static void Collect(
        Node root,
        List<TextTarget> targets)
    {
        var value = root.Get(TextProperty);

        if (value.VariantType == Variant.Type.String)
        {
            string text = value.AsString();

            if (text.Contains(ChiToken) || text.Contains(MaxChiToken))
            {
                targets.Add(
                    new TextTarget
                    {
                        TextNode = root,
                        Template = text
                    });
            }
        }

        foreach (var child in root.GetChildren())
            Collect(child, targets);
    }
}