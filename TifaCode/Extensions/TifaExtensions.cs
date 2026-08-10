using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using Tifa.TifaCode.Cards.Ancient;
using Tifa.TifaCode.Relics;

namespace Tifa.TifaCode.Extensions;

public static class TifaExtensions
{
    public static class CombatHelpers
    {
        private const string DefaultHitVfx =
            "res://Tifa/scenes/vfx/hit_yellow.tscn";

        public static async Task TifaFakeHit(
            Creature attacker,
            Creature target,
            string? attackSfx = null,
            string? hitSfx = null,
            string? hitVfx = null)
        {
            if (target == null)
                return;

            // attack sound
            if (attackSfx != null) SfxCmd.Play(attackSfx);
            if (hitSfx != null) SfxCmd.Play(hitSfx);
            var tifa = attacker.Player.Character as Character.Tifa;

            // impact effect
            tifa.PlayVfxOnTarget(
                target,
                hitVfx ?? DefaultHitVfx,
                "hit");

            // victim reaction
            await CreatureCmd.TriggerAnim(target, "Hit", 0f);

            if (target.Monster?.HasHurtSfx == true)
            {
                SfxCmd.Play(target.Monster.HurtSfx);
            }
        }
    }
    
    public static class TifaHitEffects
    {
        public const string YellowHit =
            "res://Tifa/scenes/vfx/hit_yellow.tscn";

        public const string BlueHit =
            "res://Tifa/scenes/vfx/hit_blue.tscn";
    }

    public static class TifaAttackSfx
    {
        public const string Punch1 =
            "res://Tifa/sfx/punch_swing_1.wav";

        public const string Punch2 =
            "res://Tifa/sfx/punch_swing_2.wav";

        public const string Punch3 =
            "res://Tifa/sfx/punch_swing_3.wav";

        public const string KickUp =
            "res://Tifa/sfx/kick_up.wav";

        public const string KickDown =
            "res://Tifa/sfx/kick_down.wav";
    }
    
    public static class TifaHitSfx
    {
        public const string Punch1 =
            "res://Tifa/sfx/punch_hit_1.wav";

        public const string Punch2 =
            "res://Tifa/sfx/punch_hit_2.wav";

        public const string Punch3 =
            "res://Tifa/sfx/punch_hit_3.wav";

        public const string Kick1 =
            "res://Tifa/sfx/kick_hit_1.wav";

        public const string Kick2 =
            "res://Tifa/sfx/kick_hit_2.wav";
        
        public const string Kick3 =
            "res://Tifa/sfx/kick_hit_3.wav";
    }
    
    public static ComboRelicBase? GetComboRelic(this Player player)
    {
        return player.Relics
            .OfType<ComboRelicBase>()
            .FirstOrDefault();
    }

    public static int GetCombo(this Player player)
    {
        return player.GetComboRelic()?.Combo ?? 0;
    }

    public static int GetChi(this Player player)
    {
        return player.GetComboRelic()?.ChiLevel ?? 0;
    }

    public static int GetMaxChi(this Player player)
    {
        return player.GetComboRelic()?.MaxChiLevel ?? 0;
    }
    
    public static async Task AddLimitBreakToHand(Player player)
    {
        if (CombatManager.Instance.IsOverOrEnding)
        {
            return;
        }

        var playerState = player.PlayerCombatState;

        // Already in hand, do nothing.
        if (playerState.AllCards
            .OfType<LimitBreak>()
            .Any(c => c.Pile?.Type == PileType.Hand))
        {
            return;
        }

        // Find an existing Limit Break anywhere (draw/discard/exhaust/etc.)
        var limitBreak = playerState.AllCards
            .OfType<LimitBreak>()
            .FirstOrDefault();
        SonicStrikers? sonicStrikers = player.GetRelic<SonicStrikers>();
        
        if (limitBreak != null)
        {
            if (sonicStrikers != null)
            {
                CardCmd.Upgrade(limitBreak);
            }

            await Task.Delay(500);
            await CardPileCmd.Add(limitBreak, PileType.Hand);
        }
        else
        {
            limitBreak = player.Creature.CombatState
                .CreateCard<LimitBreak>(player);
            if (sonicStrikers != null)
            {
                CardCmd.Upgrade(limitBreak);
            }
            await Task.Delay(500);
            await CardPileCmd.AddGeneratedCardToCombat(
                limitBreak,
                PileType.Hand,
                player);
        }
    }
}
