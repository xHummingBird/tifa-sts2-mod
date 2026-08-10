using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Tifa.TifaCode.Extensions;
using Tifa.TifaCode.Powers;

namespace Tifa.TifaCode.Cards.Uncommon;

public class PunishingStrike() : TifaCard(1, CardType.Attack,
    CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(9, ValueProp.Move),
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        var tifa = Owner?.Character as Character.Tifa;

        decimal damage = DynamicVars.Damage.BaseValue;
        if (play.Target.HasPower<VulnerablePower>())
        {
            damage *= 2;
        }
        
        if (ownerCreature != null && tifa != null)
        {
            AudioHelper.PlayRandomAttack();
            tifa.PlayAnimation(ownerCreature, "uppercut");
            await Task.Delay((int)(0.267f * 1000f));
            SfxCmd.Play("res://Tifa/sfx/punch_swing_1.wav");
            await DamageCmd.Attack(damage).FromCard(this, play).Targeting(play.Target)
                .WithValueProp(ValueProp.Move)
                .WithHitFx(null, "res://Tifa/sfx/punch_hit_1.wav")
                .Execute(choiceContext);
            await Task.Delay(380);
        }
        else
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, "res://Tifa/sfx/punch_hit_1.wav")
                .Execute(choiceContext);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1);
    }
}