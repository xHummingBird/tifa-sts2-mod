using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
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

namespace Tifa.TifaCode.Cards.Common;

public class WhirlingUppercut() : TifaCard(0, CardType.Attack,
    CardRarity.Common, TargetType.AnyEnemy)
{
    private int PlayCountThisTurn =>
        CombatManager.Instance.History.CardPlaysFinished
            .Count(e =>
                e.HappenedThisTurn(base.CombatState) &&
                e.CardPlay.Player == base.Owner);
    
    protected override bool ShouldGlowGoldInternal => PlayCountThisTurn == 0;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(5, ValueProp.Move),
        new CardsVar(1)
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        var tifa = Owner?.Character as Character.Tifa;
        
        if (ownerCreature != null && tifa != null)
        {
            AudioHelper.PlayRandomAttack();
            tifa.PlayAnimation(ownerCreature, "uppercut");
            await Task.Delay((int)(0.267f * 1000f));
            SfxCmd.Play("res://Tifa/sfx/punch_swing_1.wav");
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, "res://Tifa/sfx/punch_hit_1.wav")
                .Execute(choiceContext);
            await Task.Delay(380);
        }
        else
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, "res://Tifa/sfx/punch_hit_1.wav")
                .Execute(choiceContext);
        
        if (PlayCountThisTurn == 0)
        {
            await CardPileCmd.Draw(
                choiceContext,
                base.DynamicVars.Cards.BaseValue,
                base.Owner);
        }
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1);
    }
}