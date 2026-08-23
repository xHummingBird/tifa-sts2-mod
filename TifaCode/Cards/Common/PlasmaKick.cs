using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Tifa.TifaCode.Extensions;
using Tifa.TifaCode.Powers;

namespace Tifa.TifaCode.Cards.Common;

public class PlasmaKick() : TifaCard(1, CardType.Attack,
    CardRarity.Common, TargetType.AnyEnemy)
{
    protected override bool ShouldGlowGoldInternal => base.Owner.HasPower<ChiPower>();
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(9, ValueProp.Move),
        new CardsVar(1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ChiPower>(),
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        bool haveChi = false;
            
        if (base.Owner.Creature.HasPower<ChiPower>())
            haveChi = true;
        var ownerCreature = Owner?.Creature;
        var tifa = Owner?.Character as Character.Tifa;
        
        if (ownerCreature != null && tifa != null)
        {
            AudioHelper.PlayRandomPhrase();
            tifa.PlayAnimation(ownerCreature, "kick");
            await Task.Delay((int)(0.067f * 1000f));
            SfxCmd.Play("res://Tifa/sfx/kick_up.wav");
            SfxCmd.Play("res://Tifa/sfx/kick_hit_1.wav");
            tifa.PlayVfxOnTarget(
                play.Target,
                "res://Tifa/scenes/vfx/hit_blue.tscn",
                "hit"
            );
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx("vfx/vfx_attack_lightning", "event:/sfx/characters/defect/defect_lightning_passive")
                .Execute(choiceContext);
            await Task.Delay(300);
        }
        else
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, "vfx/vfx_attack_lightning", "event:/sfx/characters/defect/defect_lightning_passive")
                .Execute(choiceContext);
        if (haveChi)
            await CardPileCmd.Draw(
                choiceContext,
                base.DynamicVars.Cards.BaseValue,
                base.Owner);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}