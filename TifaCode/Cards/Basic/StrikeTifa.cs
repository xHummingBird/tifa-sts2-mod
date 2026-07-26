using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Tifa.TifaCode.Extensions;

namespace Tifa.TifaCode.Cards.Basic;

public class StrikeTifa() : TifaCard(1, CardType.Attack,
    CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3m, ValueProp.Move),
        new RepeatVar(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Tifa tifa)
        {
            AudioHelper.PlayRandomAttack();
            float duration = tifa.PlayAnimation(ownerCreature, "attack").total;
            if (duration > 0f)
                await Task.Delay((int)(0.167f * 1000f));
            SfxCmd.Play("res://Tifa/sfx/punch_swing_1.wav");
            await Task.Delay((int)(0.033f * 1000f));
        }
        await CommonActions.CardAttack(this, play.Target, hitCount: DynamicVars.Repeat.IntValue)
            .WithHitFx("res://Tifa/scenes/vfx/hit_yellow.tscn", "res://Tifa/sfx/punch_hit_1.wav")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
    }
}