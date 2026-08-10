using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Tifa.TifaCode.Extensions;

namespace Tifa.TifaCode.Cards.Common;

public class Feint() : TifaCard(0, CardType.Attack,
    CardRarity.Common, TargetType.AnyEnemy)
{
    private int PlayCountThisTurn =>
        CombatManager.Instance.History.CardPlaysFinished
            .Count(e =>
                e.HappenedThisTurn(base.CombatState) &&
                e.CardPlay.Player == base.Owner);
    
    protected override bool ShouldGlowGoldInternal => PlayCountThisTurn == 0;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(2m, ValueProp.Move),
        new RepeatVar(2),
        new PowerVar<WeakPower>(1)
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
            DamageCmd.Attack(damage).FromCard(this, play).Targeting(play.Target)
                .WithValueProp(ValueProp.Unpowered)
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
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, "res://Tifa/sfx/punch_hit_2.wav")
                .Execute(choiceContext);
        }
        else
            await CommonActions.CardAttack(this, play.Target, hitCount: DynamicVars.Repeat.IntValue)
                .WithHitFx(null, "res://Tifa/sfx/punch_hit_1.wav")
                .Execute(choiceContext);
        if (PlayCountThisTurn == 0)
        {
            await PowerCmd.Apply<WeakPower>(choiceContext, play.Target, DynamicVars.Weak.BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars.Weak.UpgradeValueBy(1m);
    }
}