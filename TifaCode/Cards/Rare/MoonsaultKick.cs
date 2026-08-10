using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Tifa.TifaCode.Extensions;
using Tifa.TifaCode.Mechanics.Limit;
using Tifa.TifaCode.Powers;
using Tifa.TifaCode.Relics;

namespace Tifa.TifaCode.Cards.Rare;

public class MoonsaultKick() : TifaCard(1, CardType.Attack,
    CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override bool ShouldGlowGoldInternal => base.Owner.HasPower<ChiPower>();
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(10, ValueProp.Move),
        new DynamicVar("Limit", 10)
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
        var comboRelic = Owner.GetRelic<ComboRelicBase>();
        if (ownerCreature != null && tifa != null)
        {
            CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
            await tifa.DashTo(ownerCreature, play.Target, distance: 290f);
            AudioHelper.PlayRandomPhrase();
            tifa.PlayAnimation(ownerCreature, "somersault");
            await Task.Delay((int)(0.267f * 1000f));
            SfxCmd.Play("res://Tifa/sfx/kick_up.wav");
            tifa.PlayVfxOnTarget(
                play.Target,
                "res://Tifa/scenes/vfx/hit_blue.tscn",
                "hit"
            );
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, "res://Tifa/sfx/kick_critical_3.wav")
                .Execute(choiceContext);
            await Task.Delay(460);
            await tifa.Retreat(ownerCreature);
            CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        }
        else
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, "res://Tifa/sfx/kick_hit_hard.wav")
                .Execute(choiceContext);
        if (base.Owner.HasPower<ChiPower>())
            LimitManager.GainLimit(Owner, DynamicVars["Limit"].IntValue);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars["Limit"].UpgradeValueBy(3);
    }
}