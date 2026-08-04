using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Tifa.TifaCode.Extensions;
using Tifa.TifaCode.Powers;

namespace Tifa.TifaCode.Cards.Common;

public class Overpower() : TifaCard(2, CardType.Attack,
    CardRarity.Common, TargetType.AnyEnemy)
{
    protected override bool ShouldGlowGoldInternal => base.Owner.Creature.GetPowerAmount<ChiPower>() >= 2;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new RepeatVar(2),
        new EnergyVar(1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ChiPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Tifa tifa)
        {
            decimal damage = DynamicVars.Damage.PreviewValue;
            AudioHelper.PlayRandomAttack();
            float duration = tifa.PlayAnimation(ownerCreature, "attack").total;
            if (duration > 0f)
                await Task.Delay((int)(0.134f * 1000f));
            SfxCmd.Play("res://Tifa/sfx/punch_swing_1.wav");
            await Task.Delay((int)(0.033f * 1000f));
            tifa.PlayVfxOnTarget(
                play.Target,
                "res://Tifa/scenes/vfx/hit_yellow.tscn",
                "hit"
            );
            CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, "res://Tifa/sfx/punch_hit_1.wav")
                .Execute(choiceContext);
            
            await Task.Delay((int)(0.134f * 1000f));
            SfxCmd.Play("res://Tifa/sfx/punch_swing_2.wav");
            await Task.Delay((int)(0.033f * 1000f));
            tifa.PlayVfxOnTarget(
                play.Target,
                "res://Tifa/scenes/vfx/hit_yellow.tscn",
                "hit"
            );
            await DamageCmd.Attack(damage).FromCard(this, play).Targeting(play.Target)
                .WithValueProp(ValueProp.Unpowered)
                .WithHitFx(null, "res://Tifa/sfx/punch_hit_2.wav")
                .Execute(choiceContext);
        }
        else
            await CommonActions.CardAttack(this, play.Target, hitCount: DynamicVars.Repeat.IntValue)
                .WithHitFx("null", "res://Tifa/sfx/punch_hit_1.wav")
                .Execute(choiceContext);
        if (base.Owner.Creature.GetPowerAmount<ChiPower>() >= 2)
            await PowerCmd.Apply<FreeAttackPower>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
    }
}