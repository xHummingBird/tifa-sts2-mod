using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Tifa.TifaCode.Extensions;
using Tifa.TifaCode.Powers;

namespace Tifa.TifaCode.Cards.Rare;

public class Deathblow() : TifaCard(2, CardType.Attack,
    CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override bool ShouldGlowGoldInternal => base.Owner.Creature.GetPowerAmount<ChiPower>() >= 3;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(18, ValueProp.Move),
        new DynamicVar("hpPercent", 15)
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
        decimal damageAmount = DynamicVars.Damage.BaseValue;
        decimal threshold = DynamicVars["hpPercent"].BaseValue;
        
        if (ownerCreature != null && tifa != null)
        {
            CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
            await tifa.DashTo(ownerCreature, play.Target, distance: 400f);
            AudioHelper.PlayRandomPhrase();
            tifa.PlayAnimation(ownerCreature, "true_strike");
            await Task.Delay((int)(0.333f * 1000f));
            SfxCmd.Play("res://Tifa/sfx/punch_swing_1.wav");
            await DamageCmd.Attack(damageAmount).FromCard(this, play).Targeting(play.Target)
                .WithValueProp(ValueProp.Move)
                .WithHitFx(null, "res://Tifa/sfx/punch_hit_hard.wav")
                .Execute(choiceContext);
            await Task.Delay(700);
            await tifa.Retreat(ownerCreature);
            CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        }
        else
            await DamageCmd.Attack(damageAmount).FromCard(this, play).Targeting(play.Target)
                .WithValueProp(ValueProp.Move)
                .WithHitFx(null, "res://Tifa/sfx/kick_hit_1.wav")
                .Execute(choiceContext);
        if (play.Target.CurrentHp * 100 <= play.Target.MaxHp * threshold && play.Target.CurrentHp > 0)
        {
            await DoomPower.DoomKill(new List<Creature> { play.Target });
            return;
        }
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars["hpPercent"].UpgradeValueBy(5m);
    }
}