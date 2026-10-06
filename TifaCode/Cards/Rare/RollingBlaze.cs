using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Tifa.TifaCode.Extensions;
using Tifa.TifaCode.Powers;

namespace Tifa.TifaCode.Cards.Rare;

public class RollingBlaze() : TifaCard(1, CardType.Attack,
    CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override bool ShouldGlowGoldInternal => base.Owner.Creature.GetPowerAmount<ChiPower>() >= 1;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(12, ValueProp.Move),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ChiPower>(),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Dictionary<PowerModel, int> debuffAmounts = (from p in play.Target.Powers
            where p.TypeForCurrentAmount == PowerType.Debuff
            select ((PowerModel)p.ClonePreservingMutability(), Amount: p.Amount)).ToDictionary();
        foreach (KeyValuePair<PowerModel, int> item in debuffAmounts)
        {
            PowerModel key = item.Key;
            ITemporaryPower temporaryPower = key as ITemporaryPower;
            if (temporaryPower != null)
            {
                KeyValuePair<PowerModel, int> keyValuePair = debuffAmounts.FirstOrDefault<KeyValuePair<PowerModel, int>>((KeyValuePair<PowerModel, int> p) => p.Key.Id == temporaryPower.InternallyAppliedPower.Id);
                if (keyValuePair.Key != null)
                {
                    debuffAmounts[keyValuePair.Key] += item.Value;
                }
            }
        }
        
        bool haveChi = false;
            
        if (base.Owner.Creature.GetPowerAmount<ChiPower>() >= 1)
            haveChi = true;
        
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

        if (haveChi)
        {
            foreach (Creature enemy in base.CombatState.HittableEnemies)
            {
                if (enemy == play.Target)
                {
                    continue;
                }
                foreach (KeyValuePair<PowerModel, int> item2 in debuffAmounts)
                {
                    if (item2.Value != 0)
                    {
                        PowerModel powerModel = PowerCmd.FindExistingInstanceForStacking(item2.Key, enemy, item2.Key.Applier);
                        if (powerModel != null)
                        {
                            await PowerCmd.ModifyAmount(choiceContext, powerModel, item2.Value, item2.Key.Applier, this);
                            continue;
                        }
                        PowerModel power = (PowerModel)item2.Key.ClonePreservingMutability();
                        await PowerCmd.Apply(choiceContext, power, enemy, item2.Value, item2.Key.Applier, this);
                    }
                }
            }
        }
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}