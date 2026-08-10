using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Tifa.TifaCode.Extensions;
using Tifa.TifaCode.Mechanics.Limit;
using Tifa.TifaCode.Powers;

namespace Tifa.TifaCode.Cards.Ancient;

public class FinalHeaven() : TifaCard(0, CardType.Attack,
    CardRarity.Ancient, TargetType.AnyEnemy), ILimitCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(90m, ValueProp.Move),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ChiPower>(),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay? play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Tifa tifa)
        {
            CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
            SfxCmd.Play("res://Tifa/sounds/limit_break (1).wav");
            tifa.PlayAnimation(ownerCreature, "final_heaven");
            await Task.Delay(400);
            await tifa.DashTo(ownerCreature, play.Target, durationSeconds: 0.067f, distance: -350f, overrideAnim: "final_heaven");
            
            AudioHelper.PlayRandomAttackHard();
            await Task.Delay(200);
            
            tifa.DashTo(ownerCreature, play.Target, durationSeconds: 0.200f, distance: -250f, overrideAnim: "final_heaven");
            await Task.Delay(100);
            TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, play.Target,
                "res://Tifa/sfx/punch_swing_1.wav", "res://Tifa/sfx/punch_critical.wav",
                TifaExtensions.TifaHitEffects.BlueHit);
            tifa.PlayVfxOnTarget(
                play.Target,
                "res://Tifa/scenes/vfx.tscn",
                "final_heaven_hits_2"
            );
            await Task.Delay(100);
            
            AudioHelper.PlayRandomAttack();
            await Task.Delay(67);
            tifa.DashTo(ownerCreature, play.Target, durationSeconds: 0.200f, distance: -350f, overrideAnim: "final_heaven");
            await Task.Delay(67);
            TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, play.Target,
                "res://Tifa/sfx/punch_swing_2.wav", "res://Tifa/sfx/punch_critical.wav",
                TifaExtensions.TifaHitEffects.BlueHit);
            tifa.PlayVfxOnTarget(
                play.Target,
                "res://Tifa/scenes/vfx.tscn",
                "final_heaven_hits_1"
            );
            await Task.Delay(133);
            
            AudioHelper.PlayRandomAttack();
            await Task.Delay(67);
            tifa.DashTo(ownerCreature, play.Target, durationSeconds: 0.200f, distance: -250f, overrideAnim: "final_heaven");
            await Task.Delay(67);
            TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, play.Target,
                "res://Tifa/sfx/punch_swing_1.wav", "res://Tifa/sfx/punch_critical.wav",
                TifaExtensions.TifaHitEffects.BlueHit);
            tifa.PlayVfxOnTarget(
                play.Target,
                "res://Tifa/scenes/vfx.tscn",
                "final_heaven_hits_2"
            );
            await Task.Delay(133);
            
            AudioHelper.PlayRandomAttack();
            await Task.Delay(67);
            tifa.DashTo(ownerCreature, play.Target, durationSeconds: 0.200f, distance: -350f, overrideAnim: "final_heaven");
            await Task.Delay(67);
            TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, play.Target,
                "res://Tifa/sfx/punch_swing_2.wav", "res://Tifa/sfx/punch_critical.wav",
                TifaExtensions.TifaHitEffects.BlueHit);
            tifa.PlayVfxOnTarget(
                play.Target,
                "res://Tifa/scenes/vfx.tscn",
                "final_heaven_hits_1"
            );
            await Task.Delay(133);
            
            AudioHelper.PlayRandomAttack();
            await Task.Delay(67);
            tifa.DashTo(ownerCreature, play.Target, durationSeconds: 0.200f, distance: -250f, overrideAnim: "final_heaven");
            await Task.Delay(67);
            TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, play.Target,
                "res://Tifa/sfx/punch_swing_1.wav", "res://Tifa/sfx/punch_critical.wav",
                TifaExtensions.TifaHitEffects.BlueHit);
            tifa.PlayVfxOnTarget(
                play.Target,
                "res://Tifa/scenes/vfx.tscn",
                "final_heaven_hits_2"
            );
            await Task.Delay(133);
            
            AudioHelper.PlayRandomAttack();
            await Task.Delay(67);
            tifa.DashTo(ownerCreature, play.Target, durationSeconds: 0.200f, distance: -350f, overrideAnim: "final_heaven");
            await Task.Delay(67);
            TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, play.Target,
                "res://Tifa/sfx/punch_swing_2.wav", "res://Tifa/sfx/punch_critical.wav",
                TifaExtensions.TifaHitEffects.BlueHit);
            tifa.PlayVfxOnTarget(
                play.Target,
                "res://Tifa/scenes/vfx.tscn",
                "final_heaven_hits_1"
            );
            await Task.Delay(133);
            
            AudioHelper.PlayRandomAttack();
            await Task.Delay(67);
            tifa.DashTo(ownerCreature, play.Target, durationSeconds: 0.200f, distance: -250f, overrideAnim: "final_heaven");
            await Task.Delay(67);
            TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, play.Target,
                "res://Tifa/sfx/punch_swing_1.wav", "res://Tifa/sfx/punch_critical.wav",
                TifaExtensions.TifaHitEffects.BlueHit);
            tifa.PlayVfxOnTarget(
                play.Target,
                "res://Tifa/scenes/vfx.tscn",
                "final_heaven_hits_2"
            );
            await Task.Delay(133);
            
            SfxCmd.Play("res://Tifa/sounds/random_phrase (1).wav");
            SfxCmd.Play("res://Tifa/sfx/final_heaven_1.wav");
            await Task.Delay(1000);
            tifa.DashTo(ownerCreature, play.Target, durationSeconds: 0.300f, distance: -350f, overrideAnim: "final_heaven");
            await Task.Delay(33);
            SfxCmd.Play("res://Tifa/sfx/final_heaven_swing_2.wav");
            await Task.Delay(67);
            tifa.PlayVfxOnTarget(
                play.Target,
                "res://Tifa/scenes/vfx.tscn",
                "final_heaven_lasthit"
            );
            SfxCmd.Play("res://Tifa/sfx/final_heaven_hit.wav");
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, "res://Tifa/sfx/explode.wav")
                .Execute(choiceContext);
            await Task.Delay(550);
            await tifa.Retreat(ownerCreature);
            CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        }
        else  await CommonActions.CardAttack(this, play.Target)
            .WithHitFx(null, "res://Tifa/sfx/explode.wav")
            .Execute(choiceContext);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(15);
    }
}