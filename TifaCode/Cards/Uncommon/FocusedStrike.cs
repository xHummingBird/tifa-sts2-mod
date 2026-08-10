using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Tifa.TifaCode.Extensions;
using Tifa.TifaCode.Powers;

namespace Tifa.TifaCode.Cards.Common;

public class FocusedStrike() : TifaCard(2, CardType.Attack,
    CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override bool ShouldGlowGoldInternal => base.Owner.Creature.GetPowerAmount<ChiPower>() >= 2;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(14),
        new ExtraDamageVar(7m),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier((CardModel card, Creature? _) =>
                card.Owner.Creature.GetPowerAmount<ChiPower>() >= 2 ? 1 : 0)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ChiPower>(),
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        var tifa = Owner?.Character as Character.Tifa;
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
        if (ownerCreature != null && tifa != null)
        {
            await tifa.DashTo(ownerCreature, play.Target, distance: 270f);
            AudioHelper.PlayRandomPhrase();
            tifa.PlayAnimation(ownerCreature, "uppercut");
            await Task.Delay((int)(0.267f * 1000f));
            SfxCmd.Play("res://Tifa/sfx/punch_swing_1.wav");
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, "res://Tifa/sfx/punch_critical.wav")
                .Execute(choiceContext);
            await Task.Delay(380);
            await tifa.Retreat(ownerCreature);
            CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        }
        else
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, "res://Tifa/sfx/punch_hit_1.wav")
                .Execute(choiceContext);
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(2);
        DynamicVars.ExtraDamage.UpgradeValueBy(1);
    }
}