using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Tifa.TifaCode.Extensions;
using Tifa.TifaCode.Powers;

namespace Tifa.TifaCode.Cards.Rare;

public class DrainPunch() : TifaCard(1, CardType.Attack,
    CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override bool ShouldGlowGoldInternal => base.Owner.Creature.GetPowerAmount<ChiPower>() >= 2;
    
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
        decimal damageAmount = DynamicVars.Damage.PreviewValue;

        decimal healAmount = damageAmount * 0.2m;
        if (ownerCreature != null && tifa != null)
        {
            CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
            await tifa.DashTo(ownerCreature, play.Target, distance: 400f);
            AudioHelper.PlayRandomPhrase();
            tifa.PlayAnimation(ownerCreature, "true_strike");
            await Task.Delay((int)(0.333f * 1000f));
            SfxCmd.Play("res://Tifa/sfx/punch_swing_1.wav");
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target)
                .WithValueProp(ValueProp.Move)
                .WithHitFx(null, "res://Tifa/sfx/punch_hit_hard.wav")
                .Execute(choiceContext);
            await Task.Delay(700);
            await tifa.Retreat(ownerCreature);
            CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        }
        else
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target)
                .WithValueProp(ValueProp.Move)
                .WithHitFx(null, "res://Tifa/sfx/kick_hit_1.wav")
                .Execute(choiceContext);

        if (base.Owner.Creature.GetPowerAmount<ChiPower>() >= 2)
            await CreatureCmd.Heal(base.Owner.Creature, healAmount);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5);
    }
}