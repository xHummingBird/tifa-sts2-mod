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

namespace Tifa.TifaCode.Cards.Rare;

public class RollingBlaze() : TifaCard(1, CardType.Attack,
    CardRarity.Ancient, TargetType.AnyEnemy)
{
    protected override bool ShouldGlowGoldInternal => base.Owner.Creature.GetPowerAmount<ChiPower>() >= 3;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(11, ValueProp.Move),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ChiPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        var tifa = Owner?.Character as Character.Tifa;

        decimal blockAmount = DynamicVars.Damage.PreviewValue;
        
        if (ownerCreature != null && tifa != null)
        {
            CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
            await tifa.DashTo(ownerCreature, play.Target, distance: 450f);
            AudioHelper.PlayRandomLastHit();
            tifa.PlayAnimation(ownerCreature, "meteodrive");
            await Task.Delay((int)(0.067f * 1000f));
            SfxCmd.Play("res://Tifa/sfx/kick_hard.wav");
            await Task.Delay((int)(0.367f * 1000f));
            SfxCmd.Play("res://Tifa/sfx/kick_up.wav");
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, "res://Tifa/sfx/meteostrike_2.wav")
                .Execute(choiceContext);
            await Task.Delay(633);
            await tifa.Retreat(ownerCreature);
            CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        }
        else
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, "res://Tifa/sfx/meteostrike_2.wav")
                .Execute(choiceContext);

        if (base.Owner.Creature.GetPowerAmount<ChiPower>() >= 3)
        {
            CreatureCmd.GainBlock(base.Owner.Creature, blockAmount, ValueProp.Unpowered, play, false);
        }
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(10);
    }
}