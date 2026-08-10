using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Tifa.TifaCode.Extensions;
using Tifa.TifaCode.Mechanics.Limit;
using Tifa.TifaCode.Powers;

namespace Tifa.TifaCode.Cards.Ancient;

public class MeteorStrike() : TifaCard(0, CardType.Attack,
    CardRarity.Ancient, TargetType.AllEnemies), ILimitCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(42m, ValueProp.Move),
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
        
        int chiLevel = base.Owner.Creature.GetPowerAmount<ChiPower>();

        if (ownerCreature != null && Owner?.Character is Character.Tifa tifa)
        {
            CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
            
            var enemies = base.CombatState.HittableEnemies.ToList();

            var targetEnemy =
                enemies[(enemies.Count - 1) / 2];
            
            SfxCmd.Play("res://Tifa/sounds/limit_break (1).wav");
            SfxCmd.Play("res://Tifa/sfx/limit_break_thunder.wav");
            tifa.PlayAnimation(ownerCreature, "limit_break");
            await Task.Delay(700);
            AudioHelper.PlayRandomAttackHard();
            tifa.DashTo(ownerCreature, targetEnemy, durationSeconds: 0.267f, distance: 360f, overrideAnim: "meteor_strike");
            await Task.Delay(200);
            SfxCmd.Play("res://Tifa/sfx/kick_hard.wav");
            await Task.Delay(67);
            SfxCmd.Play("res://Tifa/sfx/meteostrike_2.wav");
            foreach (var target in enemies)
            {
                TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, target);
                var vfx = NGroundFireVfx.Create(target);
                if (vfx != null)
                {
                    NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(vfx);
                }
            }
           

            await Task.Delay(400);
            SfxCmd.Play("res://Tifa/sfx/kick_hard.wav");
            await Task.Delay(100);
            SfxCmd.Play("res://Tifa/sfx/meteostrike_2.wav");
            foreach (var target in enemies)
            {
                TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, target);
                var vfx = NGroundFireVfx.Create(target);
                if (vfx != null)
                {
                    NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(vfx);
                }
            }
            AudioHelper.PlayRandomAttackHard();
            await Task.Delay(633);
            SfxCmd.Play("res://Tifa/sfx/meteostrike_2.wav");
            foreach (var target in enemies)
            {
                TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, target);
                var vfx = NGroundFireVfx.Create(target);
                if (vfx != null)
                {
                    NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(vfx);
                }
            }
            AudioHelper.PlayRandomAttackHard();
            await Task.Delay(400);
            SfxCmd.Play("res://Tifa/sfx/kick_hard.wav");
            await Task.Delay(67);
            SfxCmd.Play("res://Tifa/sfx/meteostrike_2.wav");
            foreach (var target in enemies)
            {
                TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, target);
                var vfx = NGroundFireVfx.Create(target);
                if (vfx != null)
                {
                    NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(vfx);
                }
            }
            
            await Task.Delay(733);
            AudioHelper.PlayRandomLastHit();
            await Task.Delay(200);
            SfxCmd.Play("res://Tifa/sfx/kick_hard.wav");
            await Task.Delay(100);
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, "res://Tifa/sfx/meteostrike_4.wav")
                .BeforeDamage(async delegate
                {
                    var targets = base.CombatState.HittableEnemies;

                    foreach (var target in targets)
                    {
                        var vfx = NGroundFireVfx.Create(target);
                        if (vfx != null)
                        {
                            NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(vfx);
                        }
                    }
                })
                .Execute(choiceContext);
            
            await Task.Delay(1000);
            await tifa.Retreat(ownerCreature);
            CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        }
        else  await CommonActions.CardAttack(this, play.Target)
            .WithHitFx(null, "res://Tifa/sfx/meteostrike_4.wav")
            .BeforeDamage(async delegate
            {
                var targets = base.CombatState.HittableEnemies;

                foreach (var target in targets)
                {
                    var vfx = NGroundFireVfx.Create(target);
                    if (vfx != null)
                    {
                        NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(vfx);
                    }
                }
            })
            .Execute(choiceContext);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(10);
    }
}