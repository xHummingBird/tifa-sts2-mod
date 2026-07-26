using MegaCrit.Sts2.Core.Commands;

namespace Tifa.TifaCode.Extensions;

public static class AudioHelper
{
    private static readonly Random rng = new Random();
    
    private static readonly string[] attackSfx =
    {
        "res://Tifa/sounds/attack (1).wav",
        "res://Tifa/sounds/attack (2).wav",
        "res://Tifa/sounds/attack (3).wav",
        "res://Tifa/sounds/attack (4).wav",
        "res://Tifa/sounds/attack (5).wav",
        "res://Tifa/sounds/attack (6).wav",
    };
    
    private static readonly string[] damagedSfx =
    {
        "res://Tifa/sounds/hit_low (1).wav",
        "res://Tifa/sounds/hit_low (2).wav",
        "res://Tifa/sounds/hit_low (3).wav",
    };
    
    private static readonly string[] highDamagedSfx =
    {
        "res://Tifa/sounds/hit_high (1).wav",
        "res://Tifa/sounds/hit_high (2).wav",
        "res://Tifa/sounds/hit_high (3).wav",
    };
    
    private static readonly string[] criticalDamagedSfx =
    {
        "res://Tifa/sounds/hit_critical (1).wav",
        "res://Tifa/sounds/hit_critical (2).wav",
        "res://Tifa/sounds/hit_critical (3).wav",
        "res://Tifa/sounds/hit_critical (4).wav",
        "res://Tifa/sounds/hit_critical (5).wav",
    };
    
    private static readonly string[] defendSfx =
    {
        "res://Tifa/sounds/defend (1).wav",
        "res://Tifa/sounds/defend (2).wav",
        "res://Tifa/sounds/defend (3).wav",
        "res://Tifa/sounds/defend (4).wav",
    };
    
    private static readonly string[] victorySfx =
    {
        "res://Tifa/sounds/victory_1.wav",
        "res://Tifa/sounds/victory_2.wav",
        "res://Tifa/sounds/victory_3.wav",
        "res://Tifa/sounds/victory_4.wav",
        "res://Tifa/sounds/victory_5.wav",
        "res://Tifa/sounds/victory_6.wav",
    };

    private static readonly string[] gameoverSfx =
    {
        "res://Tifa/sounds/gameover (1).wav",
        "res://Tifa/sounds/gameover (2).wav",
        "res://Tifa/sounds/gameover (3).wav",
    };
    
    private static readonly string[] phraseSfx =
    {
        "res://Tifa/sounds/random_phrase (1).wav",
        "res://Tifa/sounds/random_phrase (2).wav",
        "res://Tifa/sounds/random_phrase (3).wav",
        "res://Tifa/sounds/random_phrase (4).wav",
        "res://Tifa/sounds/random_phrase (5).wav",
        "res://Tifa/sounds/random_phrase (6).wav",
        "res://Tifa/sounds/random_phrase (7).wav",
        "res://Tifa/sounds/random_phrase (8).wav",
        "res://Tifa/sounds/random_phrase (9).wav",
        "res://Tifa/sounds/random_phrase (10).wav",
    };

    private static readonly string[] buffSfx =
    {
        "res://Tifa/sounds/chi_buff (1).wav",
        "res://Tifa/sounds/chi_buff (2).wav",
        "res://Tifa/sounds/chi_buff (3).wav",
    };

    private static readonly string[] attackHighSfx =
    {
        "res://Tifa/sounds/attack_hard (1).wav",
        "res://Tifa/sounds/attack_hard (2).wav",
        "res://Tifa/sounds/attack_hard (3).wav",
        "res://Tifa/sounds/attack_hard (4).wav",
        
    };
    
    private static readonly string[] lastHitSfx =
    {
        "res://Tifa/sounds/last_hit (1).wav",
        "res://Tifa/sounds/last_hit (2).wav",
        "res://Tifa/sounds/last_hit (3).wav",
        "res://Tifa/sounds/last_hit (4).wav",
        "res://Tifa/sounds/last_hit (5).wav",
    };
    
    private static readonly string[] limitBreakSfx =
    {
        "res://Tifa/sounds/limit_break (1).wav",
        "res://Tifa/sounds/limit_break (2).wav",
    };
    
    public static void PlayRandomAttack()
    {
        PlayRandom(attackSfx);
    }
    
    public static void PlayRandomDefend()
    {
        PlayRandom(defendSfx);
    }

    public static void PlayRandomPhrase()
    {
        PlayRandom(phraseSfx);
    }
    
    public static void PlayRandomDamaged()
    {
        PlayRandom(damagedSfx);
    }

    public static void PlayRandomDamagedHigh()
    {
        PlayRandom(highDamagedSfx);
    }

    public static void PlayRandomGameover()
    {
        PlayRandom(gameoverSfx);
    }

    public static void PlayRandomDamagedCritical()
    {
        PlayRandom(criticalDamagedSfx);
    }
    
    public static void PlayRandomVictory()
    {
        PlayRandom(victorySfx);
    }

    public static void PlayRandomAttackHard()
    {
        PlayRandom(attackHighSfx);
    }

    public static void PlayRandomLastHit()
    {
        PlayRandom(lastHitSfx);
    }

    public static void PlayRandomLimitBreak()
    {
        PlayRandom(limitBreakSfx);
    }

    public static void PlayRandomChiBuff()
    {
        PlayRandom(buffSfx);
    }

    public static void PlayRandom(string[] pool)
    {
        int index = rng.Next(pool.Length);
        SfxCmd.Play(pool[index]);
    }
}