using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Tifa.TifaCode.Extensions;
using Tifa.TifaCode.Powers;

namespace Tifa.TifaCode.Cards.Uncommon;

public class ChiBarrier() : TifaCard(1, CardType.Skill,
    CardRarity.Uncommon, TargetType.Self)
{
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new BlockVar(4, ValueProp.Move),
        new DynamicVar("BlockMultiplier", 2m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        AudioHelper.PlayRandomDefend();
        await CommonActions.CardBlock(this, play);
        decimal blockAmount = Owner.Creature.GetPowerAmount<ChiPower>() * DynamicVars["BlockMultiplier"].BaseValue;
        CreatureCmd.GainBlock(base.Owner.Creature, blockAmount, ValueProp.Unpowered, play, false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BlockMultiplier"].UpgradeValueBy(1m);
    }
}