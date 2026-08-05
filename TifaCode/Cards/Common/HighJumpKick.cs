using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Tifa.TifaCode.Extensions;
using Tifa.TifaCode.Powers;

namespace Tifa.TifaCode.Cards.Common;

public class HighJumpKick() : TifaCard(0, CardType.Attack,
    CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(6, ValueProp.Move),
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        var tifa = Owner?.Character as Character.Tifa;
        
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
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx( null,"res://Tifa/sfx/kick_hit_1.wav")
                .Execute(choiceContext);
        }
        else
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, "res://Tifa/sfx/kick_hit_1.wav")
                .Execute(choiceContext);
        CardModel card = CreateClone();
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Discard, base.Owner), 1.0f);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}