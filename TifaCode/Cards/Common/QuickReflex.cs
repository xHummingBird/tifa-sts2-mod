using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Tifa.TifaCode.Extensions;

namespace Tifa.TifaCode.Cards.Common;

public class QuickReflex() : TifaCard(0, CardType.Skill,
    CardRarity.Common, TargetType.Self)
{
    private int PlayCountThisTurn =>
        CombatManager.Instance.History.CardPlaysFinished
            .Count(e =>
                e.HappenedThisTurn(base.CombatState) &&
                e.CardPlay.Player == base.Owner);
    
    protected override bool ShouldGlowGoldInternal => PlayCountThisTurn == 0;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(3, ValueProp.Move),
        new CardsVar(1)
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        AudioHelper.PlayRandomDefend();
        await CommonActions.CardBlock(this, play);
        
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
        DynamicVars.Block.UpgradeValueBy(2);
    }
}