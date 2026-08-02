using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Tifa.TifaCode.Extensions;

namespace Tifa.TifaCode.Cards.Uncommon;

public class SonicCombo() : TifaCard(2, CardType.Attack,
    CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new RepeatVar(3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
        
        if (ownerCreature != null && Owner?.Character is Character.Tifa tifa)
        {
            await tifa.DashTo(ownerCreature, play.Target, distance: 250f);
            decimal damage = DynamicVars.Damage.PreviewValue;
            AudioHelper.PlayRandomAttack();
            float duration = tifa.PlayAnimation(ownerCreature, "sonic_combo").total;
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
            
            await Task.Delay((int)(0.133f * 1000f));
            SfxCmd.Play("res://Tifa/sfx/punch_swing_2.wav");
            await Task.Delay((int)(0.033f * 1000f));
            tifa.PlayVfxOnTarget(
                play.Target,
                "res://Tifa/scenes/vfx/hit_yellow.tscn",
                "hit"
            );
            DamageCmd.Attack(damage).FromCard(this, play).Targeting(play.Target)
                .WithValueProp(ValueProp.Unpowered)
                .WithHitFx(null, "res://Tifa/sfx/punch_hit_2.wav")
                .Execute(choiceContext);
            
            await Task.Delay((int)(0.234f * 1000f));
            SfxCmd.Play("res://Tifa/sfx/kick_up.wav");
            AudioHelper.PlayRandomAttackHard();
            tifa.PlayVfxOnTarget(
                play.Target,
                "res://Tifa/scenes/vfx/hit_yellow.tscn",
                "hit"
            );
            await DamageCmd.Attack(damage).FromCard(this, play).Targeting(play.Target)
                .WithValueProp(ValueProp.Unpowered)
                .WithHitFx(null, "res://Tifa/sfx/kick_critical_3.wav")
                .Execute(choiceContext);
            await Task.Delay((int)(0.267f * 1000f));
            await tifa.Retreat(ownerCreature);
        }
        else
            await CommonActions.CardAttack(this, play.Target, hitCount: DynamicVars.Repeat.IntValue)
                .WithHitFx("null", "res://Tifa/sfx/punch_hit_1.wav")
                .Execute(choiceContext);
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
    }
}