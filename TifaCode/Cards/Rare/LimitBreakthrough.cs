using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Tifa.TifaCode.Extensions;
using Tifa.TifaCode.Mechanics.Limit;
using Tifa.TifaCode.Powers;

namespace Tifa.TifaCode.Cards.Rare;

public class LimitBreakthrough() : TifaCard(1, CardType.Skill,
    CardRarity.Rare, TargetType.Self)
{
    protected override bool ShouldGlowGoldInternal => base.Owner.Creature.CurrentHp * 2 < base.Owner.Creature.MaxHp;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new PowerVar<LimitBreakPower>(20m),
        new DynamicVar("ComboMultiplier", 50m)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<LimitBreakPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        AudioHelper.PlayRandomChiBuff();
        int combo = Owner.GetCombo();
        int finalAmount = DynamicVars["LimitBreakPower"].IntValue;
        int additionalAmount = (int)(combo * (DynamicVars["ComboMultiplier"].BaseValue/100m));

        finalAmount += additionalAmount;
        
        LimitManager.GainLimit(Owner, finalAmount);
    }
    protected override void OnUpgrade()
    {
        DynamicVars["ComboMultiplier"].UpgradeValueBy(16);
    }
}