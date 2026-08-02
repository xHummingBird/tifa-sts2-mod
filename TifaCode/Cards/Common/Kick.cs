using BaseLib.Extensions;
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
using Tifa.TifaCode.Powers;
using Tifa.TifaCode.Relics;

namespace Tifa.TifaCode.Cards.Common;

public class Kick() : TifaCard(1, CardType.Attack,
    CardRarity.Common, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(4, ValueProp.Move),
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var comboRelic =
            Owner.GetRelic<ComboRelicBase>();
        
        var ownerCreature = Owner?.Creature;
        var tifa = Owner?.Character as Character.Tifa;
        
        var targets = base.CombatState.HittableEnemies;
        int enemiesHit = base.CombatState.HittableEnemies.Count(e => e.IsAlive);
        
        if (ownerCreature != null && tifa != null)
        {
            AudioHelper.PlayRandomAttackHard();
            tifa.PlayAnimation(ownerCreature, "divekick");
            await Task.Delay((int)(0.333f * 1000f));
            SfxCmd.Play("res://Tifa/sfx/kick_down.wav");
            await Task.Delay((int)(0.067f * 1000f));
            foreach (var target in targets)
            {
                tifa.PlayVfxOnTarget(
                    target,
                    "res://Tifa/scenes/vfx/hit_yellow.tscn",
                    "hit"
                );
            }
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, "res://Tifa/sfx/kick_hit_1.wav")
                .Execute(choiceContext);
            await Task.Delay(170);
        }
        else
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, "res://Tifa/sfx/kick_hit_1.wav")
                .Execute(choiceContext);
        comboRelic?.GainCombo(enemiesHit - 1); //minus 1 because playing the card itself already increases it by 1
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}