using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Tifa.TifaCode.Extensions;
using Tifa.TifaCode.Powers;

namespace Tifa.TifaCode.Cards.Uncommon;

public class BlazeKick() : TifaCard(1, CardType.Attack,
    CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override bool ShouldGlowGoldInternal => base.Owner.HasPower<ChiPower>();
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(10, ValueProp.Move),
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
        
        await using AttackContext context =
            await AttackCommand.CreateContextAsync(
                base.CombatState,
                choiceContext,
                play);
        
        if (ownerCreature != null && tifa != null)
        {
            AudioHelper.PlayRandomPhrase();
            tifa.PlayAnimation(ownerCreature, "high_jump_kick");
            await Task.Delay((int)(0.133f * 1000f));
            SfxCmd.Play("res://Tifa/sfx/kick_up.wav");
            tifa.PlayVfxOnTarget(
                play.Target,
                "res://Tifa/scenes/vfx/hit_blue.tscn",
                "hit"
            );
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(play.Target));
            SfxCmd.Play("event:/sfx/characters/attack_fire");
            SfxCmd.Play("res://Tifa/sfx/kick_hit_1.wav");
                
        }
        
        List<DamageResult> results =
        (
            await CreatureCmd.Damage(
                choiceContext,
                play.Target,
                base.DynamicVars.Damage.BaseValue,
                ValueProp.Move,
                this,
                play)
        ).ToList();

        context.AddHit(results);

        DamageResult? damageResult = results.FirstOrDefault();

        if (damageResult == null)
            return;

        if (base.Owner.HasPower<ChiPower>())
        {
            List<Creature> otherEnemies = base.CombatState
                .GetTeammatesOf(damageResult.Receiver)
                .Where(e =>
                    e != damageResult.Receiver &&
                    e.IsHittable)
                .ToList();

            if (otherEnemies.Count > 0)
            {
                foreach (var enemy in otherEnemies)
                {
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(
                        NGroundFireVfx.Create(enemy));
                }

                context.AddHit(
                    await CreatureCmd.Damage(
                        choiceContext,
                        otherEnemies,
                        damageResult.TotalDamage,
                        ValueProp.Unpowered | ValueProp.Move,
                        ownerCreature,
                        this,
                        play)
                );
            }
        }
        await Task.Delay(400);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}