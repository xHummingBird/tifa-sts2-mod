using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Tifa.TifaCode.Extensions;
using Tifa.TifaCode.Powers;
using Tifa.TifaCode.Relics;

namespace Tifa.TifaCode.Cards.Common;

public class EnergyWave() : TifaCard(1, CardType.Attack,
    CardRarity.Common, TargetType.AnyEnemy)
{
    protected override bool ShouldGlowGoldInternal => IsPlayable;
    protected override bool IsPlayable => base.Owner.HasPower<ChiPower>();
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ChiPower>(),
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(11m, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var comboRelic = Owner.GetRelic<ComboRelicBase>();
        comboRelic?.LoseCombo(5);
        var ownerCreature = Owner?.Creature;
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
        if (ownerCreature != null && Owner?.Character is Character.Tifa tifa)
        {
            AudioHelper.PlayRandomPhrase();
            tifa.PlayAnimation(ownerCreature, "cast");
            await Task.Delay(300);
            tifa.PlayVfxOnTarget(
                cardPlay.Target,
                "res://Tifa/scenes/vfx.tscn/",
                "chi_trap");
            SfxCmd.Play("res://Tifa/sfx/explode.wav");
            await Task.Delay(100);
        }
        await CommonActions.CardAttack(this, cardPlay.Target)
            .WithHitVfxSpawnedAtBase()
            .BeforeDamage(async delegate
            {
                    var vfx = NGroundFireVfx.Create(cardPlay.Target, VfxColor.Blue);
                    if (vfx != null)
                    {
                        NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(vfx);
                        SfxCmd.Play("event:/sfx/characters/attack_fire");
                    }
                
            })
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(4m);
    }
}