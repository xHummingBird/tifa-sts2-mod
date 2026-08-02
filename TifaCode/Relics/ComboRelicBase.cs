using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using BaseLib.Extensions;
using Cloud.CloudCode.Mechanics.Limit;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using Tifa.TifaCode.Mechanics.Limit;
using Tifa.TifaCode.Powers;

namespace Tifa.TifaCode.Relics;

public abstract class ComboRelicBase : TifaRelic
{
    private int _combo;
    private bool _comboIncreasedThisTurn;
    public int StoredLimit { get; set; }

    public override RelicRarity Rarity => RelicRarity.Starter;

    public override bool ShowCounter => CombatManager.Instance.IsInProgress;

    public override int DisplayAmount => Combo;

    public int Combo => _combo;

    public int ChiLevel => Math.Clamp(
        Combo / ComboPerChiLevel,
        0,
        MaxChiLevel);

    public virtual int MaxCombo => 100;

    protected virtual int BaseMaxChiLevel => 6;

    protected virtual int StartingCombo => 0;

    protected virtual int ComboPerChiLevel => 5;

    protected virtual int ComboLossFromUnblockedDamage => 1;

    protected virtual int ComboDecayDivisor => 5; // 20%

    public int MaxChiLevel => Math.Clamp(
        BaseMaxChiLevel + GetBonusMaxChiLevel(),
        0,
        MaxCombo / ComboPerChiLevel);

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("MaxCombo", MaxCombo),
        new DynamicVar("BaseMaxChiLevel", BaseMaxChiLevel),
        new DynamicVar("MaxChiLevel", MaxChiLevel),
        new DynamicVar("ComboPerChiLevel", ComboPerChiLevel),
        new DynamicVar("ComboLossFromUnblockedDamage", ComboLossFromUnblockedDamage)
    ];
    
    protected virtual int GetBonusMaxChiLevel()
    {
        int bonus = 0;

        /*
         * Add future max Chi bonuses here.
         *
         * Examples:
         *
         * if (base.Owner.GetRelic<SomeMaxChiRelic>() != null)
         *     bonus += 1;
         *
         * bonus += base.Owner.Creature.GetPowerAmount<SomeMaxChiPower>();
         */

        return bonus;
    }

    private int ComboInternal
    {
        get => _combo;
        set
        {
            AssertMutable();

            _combo = Math.Clamp(
                value,
                0,
                MaxCombo);

            UpdateDisplay();
        }
    }

    public override async Task BeforeCombatStart()
    {
        ComboInternal = 0;
        _comboIncreasedThisTurn = false;
        base.Status = RelicStatus.Normal;

        int startingCombo = GetStartingComboFromRelics();

        if (startingCombo > 0)
        {
            GainCombo(
                startingCombo,
                countsForTurnCheck: false);
        }

        await SyncChiPower(
            null,
            base.Owner.Creature,
            null);
    }
    
    private int GetStartingComboFromRelics()
    {
        int startingCombo = 0;

        if (base.Owner.GetRelic<SonicStrikers>() != null)
        {
            startingCombo += ComboPerChiLevel; // +5 Combo = Chi 1
        }

        return startingCombo;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        LimitManager.SetLimit(
            base.Owner,
            Combo);

        ComboInternal = 0;
        _comboIncreasedThisTurn = false;
        base.Status = RelicStatus.Normal;

        return Task.CompletedTask;
    }
    
    private bool HasLivingEnemies()
    
    {
        var state = CombatManager.Instance?.DebugOnlyGetState();
        
        if (state == null)
            return false;
        
        return state.Creatures.Any(c =>
        c.Side != base.Owner.Creature.Side && !c.IsDead);
    }

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (!CombatManager.Instance.IsInProgress)
            return;

        CardModel card = cardPlay.Card;

        if (card.Owner != base.Owner)
            return;

        if (card.Type == CardType.Attack)
        {
            GainLimitFromAttackHits(card);
        }

        int comboGain = GetComboGainFromCard(card);

        if (comboGain > 0)
        {
            comboGain = ModifyComboGain(card, comboGain);
        }

        if (comboGain > 0)
        {
            GainCombo(comboGain);
        }

        if (HasLivingEnemies())
        {
            await SyncChiPower(
                choiceContext,
                base.Owner.Creature,
                card);
        }
    }
    
    private void GainLimitFromAttackHits(CardModel card)
    {
        if (card.Type != CardType.Attack)
            return;

        int hits = TryGetRepeatVarFromCard(card) ?? 1;
        hits = Math.Max(1, hits);

        int limitPerHit = 3;

        int wayOfTheFistAmount =
            base.Owner.Creature.GetPowerAmount<WayOfTheFistPower>();

        if (wayOfTheFistAmount > 0)
        {
            limitPerHit += wayOfTheFistAmount;
        }

        LimitManager.GainLimit(
            base.Owner,
            hits * limitPerHit);
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side != base.Owner.Creature.Side)
            return;

        int limitGain = 5;

        if (ChiLevel >= 4)
        {
            limitGain += 2;
        }
            
        _comboIncreasedThisTurn = false;
        
        LimitManager.GainLimit(base.Owner, limitGain);
        
        await SyncChiPower(
            null,
            base.Owner.Creature,
            null);
        
        
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != base.Owner.Creature.Side)
            return;

        if (!_comboIncreasedThisTurn)
        {
            DecayCombo();
        }

        await SyncChiPower(
            null,
            base.Owner.Creature,
            null);
    }

    public override Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != base.Owner.Creature)
            return Task.CompletedTask;

        if (result.UnblockedDamage <= 0)
            return Task.CompletedTask;

        LoseCombo(ComboLossFromUnblockedDamage);

        /*
         * Do not sync ChiPower here.
         * Damage loss should affect Chi on the next player turn check.
         */

        return Task.CompletedTask;
    }

    private int GetComboGainFromCard(CardModel card)
    {
        if (card.Type != CardType.Attack)
            return 0;

        int repeat = TryGetRepeatVarFromCard(card) ?? 1;

        return Math.Max(1, repeat);
    }

    /*
     * This lets Leather Glove / Premium Heart / future relics modify combo gain.
     *
     * Important:
     * This is only called when the base combo gain is greater than 0.
     * So Skills and Powers will not accidentally gain combo here.
     */
    protected virtual int ModifyComboGain(CardModel card, int amount)
    {
        if (card.Type == CardType.Attack &&
            ChiLevel >= 2 && card is not ILimitCard)
        {
            amount += 1;
        }

        return amount;
    }

    private int? TryGetRepeatVarFromCard(CardModel card)
    {
        object? dynamicVars = GetPropertyValue(card, "DynamicVars");

        if (dynamicVars == null)
            return null;

        object? repeatVar = GetPropertyValue(dynamicVars, "Repeat");

        if (repeatVar == null)
            return null;

        object? intValue = GetPropertyValue(repeatVar, "IntValue");

        if (intValue is int value)
            return value;

        return null;
    }

    private static object? GetPropertyValue(
        object instance,
        string propertyName)
    {
        try
        {
            PropertyInfo? property = instance
                .GetType()
                .GetProperty(
                    propertyName,
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic);

            return property?.GetValue(instance);
        }
        catch
        {
            return null;
        }
    }

    private async Task SyncChiPower(
        PlayerChoiceContext? choiceContext,
        Creature source,
        CardModel? card)
    {
        Creature creature = base.Owner.Creature;

        int currentChi = creature.GetPowerAmount<ChiPower>();
        int targetChi = ChiLevel;

        if (targetChi > currentChi)
        {
            if (Owner.Character is Character.Tifa tifa)
            {
                float duration = tifa.PlayAnimation(creature, "chi").total;
                await Task.Delay(500);
            }
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(base.Owner.Creature));
            await PowerCmd.Apply<ChiPower>(
                choiceContext,
                creature,
                targetChi - currentChi,
                source,
                card);
        }
        else if (targetChi < currentChi)
        {
            var chiPower = creature.GetPower<ChiPower>();

            if (chiPower == null)
                return;

            int amountToRemove = currentChi - targetChi;

            for (int i = 0; i < amountToRemove; i++)
            {
                await PowerCmd.Decrement(chiPower);

                if (!creature.HasPower<ChiPower>())
                    break;

                chiPower = creature.GetPower<ChiPower>();

                if (chiPower == null)
                    break;
            }
        }
    }

    public void GainCombo(int amount)
    {
        GainCombo(
            amount,
            countsForTurnCheck: true);
    }

    public void GainCombo(
        int amount,
        bool countsForTurnCheck)
    {
        if (amount <= 0)
            return;

        ComboInternal += amount;

        if (countsForTurnCheck)
            _comboIncreasedThisTurn = true;
    }

    public void LoseCombo(int amount)
    {
        if (amount <= 0)
            return;

        ComboInternal -= amount;
    }

    public void SetCombo(int amount)
    {
        ComboInternal = amount;
    }

    public void ConsumeCombo(int amount)
    {
        LoseCombo(amount);
    }

    public void ConsumeAllCombo()
    {
        ComboInternal = 0;
    }

    public int SpendAllCombo()
    {
        int spent = Combo;

        ConsumeAllCombo();

        return spent;
    }

    public bool HasCombo(int amount)
    {
        return Combo >= amount;
    }

    public bool HasChi(int amount)
    {
        return ChiLevel >= amount;
    }

    public bool HasMaxChi()
    {
        return ChiLevel >= MaxChiLevel;
    }

    public int GetComboForUI()
    {
        return Combo;
    }

    public int GetMaxComboForUI()
    {
        return MaxCombo;
    }

    public int GetChiLevelForUI()
    {
        return ChiLevel;
    }

    public int GetMaxChiLevelForUI()
    {
        return MaxChiLevel;
    }

    public int GetComboProgressForUI()
    {
        return Combo % ComboPerChiLevel;
    }

    public int GetComboProgressMaxForUI()
    {
        return ComboPerChiLevel;
    }

    private void DecayCombo()
    {
        int amountToLose = Combo / ComboDecayDivisor;

        if (amountToLose <= 0)
            return;

        LoseCombo(amountToLose);
    }

    private void UpdateDisplay()
    {
        base.Status = Combo > 0
            ? RelicStatus.Active
            : RelicStatus.Normal;

        InvokeDisplayAmountChanged();
    }
}