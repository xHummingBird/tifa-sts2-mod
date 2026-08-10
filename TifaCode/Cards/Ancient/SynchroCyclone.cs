using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Tifa.TifaCode.Extensions;
using Tifa.TifaCode.Mechanics.Limit;
using Tifa.TifaCode.Powers;

namespace Tifa.TifaCode.Cards.Ancient;

public class SynchroCyclone() : TifaCard(3, CardType.Attack,
    CardRarity.Ancient, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(25m, ValueProp.Move),
        new EnergyVar(1)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay? play)
    {
        var ownerCreature = Owner?.Creature;
        
        int chiLevel = base.Owner.Creature.GetPowerAmount<ChiPower>();

        if (ownerCreature != null && Owner?.Character is Character.Tifa tifa)
        {
            CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
            SfxCmd.Play("res://Tifa/sounds/cloud/battle_start_4.wav");
            await tifa.DashTo(ownerCreature, play.Target, durationSeconds: 0.367f, distance: 360f, overrideAnim: "synchro_cyclone");
            
            AudioHelper.PlayRandomAttack();
            await Task.Delay(133);
            TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, play.Target,
                TifaExtensions.TifaAttackSfx.KickUp, "res://Tifa/sfx/kick_critical_1.wav",
                TifaExtensions.TifaHitEffects.BlueHit);
            SfxCmd.Play("res://Tifa/sfx/cloud/sword_swing.wav");
            SfxCmd.Play("res://Tifa/sfx/cloud/cloud_hit.wav");
            await Task.Delay(267);
            
            AudioHelper.PlayRandomAttackHard();
            await Task.Delay(133);
            TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, play.Target,
                TifaExtensions.TifaAttackSfx.KickDown, "res://Tifa/sfx/kick_critical_2.wav",
                TifaExtensions.TifaHitEffects.BlueHit);
            SfxCmd.Play("res://Tifa/sfx/cloud/sword_swing.wav");
            SfxCmd.Play("res://Tifa/sfx/cloud/cloud_hit.wav");
            
            await Task.Delay(333);
            AudioHelper.PlayRandomAttackHard();
            SfxCmd.Play("res://Tifa/sounds/cloud/generic_attack_4.wav");
            await Task.Delay(200);
            TifaExtensions.CombatHelpers.TifaFakeHit(base.Owner.Creature, play.Target,
                TifaExtensions.TifaAttackSfx.KickUp, "res://Tifa/sfx/kick_critical_3.wav",
                TifaExtensions.TifaHitEffects.BlueHit);
            SfxCmd.Play("res://Tifa/sfx/cloud/sword_swing.wav");
            SfxCmd.Play("res://Tifa/sfx/cloud/cloud_hit.wav");
            
            await Task.Delay(667);
            SfxCmd.Play("res://Tifa/sounds/cloud/owarida.wav");
            AudioHelper.PlayRandomLastHit();
            await Task.Delay(233);
            SfxCmd.Play("res://Tifa/sfx/cloud/sword_swing_heavy.wav");
            SfxCmd.Play("res://Tifa/sfx/cloud/cloud_hit.wav");
            SfxCmd.Play("res://Tifa/sfx/Punch_swing_3.wav");
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, "res://Tifa/sfx/punch_hit_hard.wav")
                .Execute(choiceContext);
            await Task.Delay(880);
            await tifa.Retreat(ownerCreature);
            CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        }
        else  await CommonActions.CardAttack(this, play.Target)
            .WithHitFx(null, "res://Tifa/sfx/punch_hit_hard.wav")
            .Execute(choiceContext);
        foreach (CardModel card in PileType.Hand.GetPile(base.Owner).Cards.ToList())
        {
            if (card.Type == CardType.Skill && !card.EnergyCost.CostsX)
            {
                card.SetToFreeThisTurn();
            }
        }
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(7);
    }
}