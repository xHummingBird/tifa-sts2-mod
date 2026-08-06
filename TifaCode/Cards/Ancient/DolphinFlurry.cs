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

public class DolphinFlurry() : TifaCard(0, CardType.Attack,
    CardRarity.Ancient, TargetType.AnyEnemy), ILimitCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(38),
        new ExtraDamageVar(15m),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier((CardModel card, Creature? _) =>
                card.Owner.Creature.GetPowerAmount<ChiPower>() >= 5 ? 1 : 0),
        new PowerVar<WeakPower>(2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ChiPower>(),
        HoverTipFactory.FromPower<WeakPower>(),
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
            SfxCmd.Play("res://Tifa/sfx/limit_break_thunder.wav");
            await tifa.DashTo(ownerCreature, play.Target, durationSeconds: 0.267f, distance: 300f, overrideAnim: "dolphin_flurry");
            AudioHelper.PlayRandomAttackHard();
            await Task.Delay(400);
            TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, play.Target,
                TifaExtensions.TifaAttackSfx.KickUp, "res://Tifa/sfx/kick_critical_1.wav",
                TifaExtensions.TifaHitEffects.BlueHit);
            await Task.Delay(300);
            TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, play.Target,
                TifaExtensions.TifaAttackSfx.KickUp, "res://Tifa/sfx/kick_critical_2.wav",
                TifaExtensions.TifaHitEffects.BlueHit);
            await Task.Delay(200);
            TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, play.Target,
                TifaExtensions.TifaAttackSfx.KickUp, "res://Tifa/sfx/kick_critical_3.wav",
                TifaExtensions.TifaHitEffects.BlueHit);
            await Task.Delay(200);
            TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, play.Target,
                TifaExtensions.TifaAttackSfx.KickUp, "res://Tifa/sfx/kick_critical_1.wav",
                TifaExtensions.TifaHitEffects.BlueHit);
            await Task.Delay(100);
            TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, play.Target,
                TifaExtensions.TifaAttackSfx.KickUp, "res://Tifa/sfx/kick_critical_2.wav", 
                TifaExtensions.TifaHitEffects.BlueHit);
            await Task.Delay(100);
            TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, play.Target,
                TifaExtensions.TifaAttackSfx.KickUp, "res://Tifa/sfx/kick_critical_3.wav", 
                TifaExtensions.TifaHitEffects.BlueHit);
            await Task.Delay(100);
            TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, play.Target,
                TifaExtensions.TifaAttackSfx.KickUp, "res://Tifa/sfx/kick_critical_1.wav", 
                TifaExtensions.TifaHitEffects.BlueHit);
            await Task.Delay(100);
            TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, play.Target,
                TifaExtensions.TifaAttackSfx.KickUp, "res://Tifa/sfx/kick_critical_2.wav", 
                TifaExtensions.TifaHitEffects.BlueHit);
            await Task.Delay(100);
            TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, play.Target,
                TifaExtensions.TifaAttackSfx.KickUp, "res://Tifa/sfx/kick_critical_3.wav", 
                TifaExtensions.TifaHitEffects.BlueHit);
            await Task.Delay(133);
            AudioHelper.PlayRandomAttackHard();
            await Task.Delay(67);
            TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, play.Target,
                TifaExtensions.TifaAttackSfx.KickUp, "res://Tifa/sfx/kick_critical_3.wav",
                TifaExtensions.TifaHitEffects.BlueHit);
            await Task.Delay(567);
            SfxCmd.Play("res://Tifa/sfx/dolphin_charge.wav");
            AudioHelper.PlayRandomLastHit();
            await Task.Delay(333);
            SfxCmd.Play("res://Tifa/sfx/dolphin_blow_2.wav");
            await Task.Delay(67);
            SfxCmd.Play("res://Tifa/sfx/punch_swing_1.wav");
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, "res://Tifa/sfx/punch_hit_hard.wav")
                .Execute(choiceContext);
            await Task.Delay(267);
            SfxCmd.Play("res://Tifa/sfx/dolphin_blow_3.wav");
            await Task.Delay(637);
            await tifa.Retreat(ownerCreature);
            CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        }
        else await CommonActions.CardAttack(this, play.Target)
            .WithHitFx(null, "res://Tifa/sfx/punch_hit_hard.wav")
            .Execute(choiceContext);
        await PowerCmd.Apply<WeakPower>(choiceContext, play.Target, DynamicVars.Weak.BaseValue, base.Owner.Creature, this);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(5);
        DynamicVars.ExtraDamage.UpgradeValueBy(5);
    }
}