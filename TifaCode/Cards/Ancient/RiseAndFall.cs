using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Tifa.TifaCode.Extensions;
using Tifa.TifaCode.Mechanics.Limit;
using Tifa.TifaCode.Powers;

namespace Tifa.TifaCode.Cards.Ancient;

public class RiseAndFall() : TifaCard(0, CardType.Attack,
    CardRarity.Ancient, TargetType.AnyEnemy), ILimitCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(30m, ValueProp.Move),
        new PowerVar<VulnerablePower>(2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ChiPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
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
        
        int chiLevel = base.Owner.Creature.GetPowerAmount<ChiPower>();

        if (ownerCreature != null && Owner?.Character is Character.Tifa tifa)
        {
            CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
            SfxCmd.Play("res://Tifa/sounds/limit_break (1).wav");
            SfxCmd.Play("res://Tifa/sfx/limit_break_thunder.wav");
            await tifa.DashTo(ownerCreature, play.Target, durationSeconds: 0.367f, distance: 360f, overrideAnim: "rise_and_fall");
            AudioHelper.PlayRandomAttack();
            await Task.Delay(133);
            TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, play.Target,
                TifaExtensions.TifaAttackSfx.KickUp, "res://Tifa/sfx/kick_critical_1.wav",
                TifaExtensions.TifaHitEffects.BlueHit);
            await Task.Delay(400);
            AudioHelper.PlayRandomAttackHard();
            await Task.Delay(133);
            TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, play.Target,
                TifaExtensions.TifaAttackSfx.KickDown, "res://Tifa/sfx/kick_critical_2.wav",
                TifaExtensions.TifaHitEffects.BlueHit);
            await Task.Delay(467);
            AudioHelper.PlayRandomAttackHard();
            await Task.Delay(100);
            TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, play.Target,
                TifaExtensions.TifaAttackSfx.KickUp, "res://Tifa/sfx/kick_critical_3.wav",
                TifaExtensions.TifaHitEffects.BlueHit);
            await Task.Delay(667);
            AudioHelper.PlayRandomAttackHard();
            await Task.Delay(233);
            tifa.PlayVfxOnTarget(
                play.Target,
                "res://Tifa/scenes/vfx/hit_blue.tscn",
                "hit"
            );
            SfxCmd.Play("res://Tifa/sfx/Punch_swing_3.wav");
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, "res://Tifa/sfx/punch_hit_hard.wav")
                .Execute(choiceContext);
            await Task.Delay(670);
            await tifa.Retreat(ownerCreature);
            CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        }
        else  await CommonActions.CardAttack(this, play.Target)
            .WithHitFx(null, "res://Tifa/sfx/punch_hit_hard.wav")
            .Execute(choiceContext);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, play.Target, DynamicVars.Vulnerable.BaseValue, base.Owner.Creature, this);
        
        if (chiLevel < 5 && chiLevel >= 3)
            LimitManager.SetLimit(base.Owner, 20);
        
        else if (chiLevel >= 5)
            LimitManager.SetLimit(base.Owner, 50);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(6);
        DynamicVars.Vulnerable.UpgradeValueBy(1);
    }
}