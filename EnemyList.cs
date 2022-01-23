using Modding;
using System.Collections.Generic;

namespace ParryKnight
{
    /// <summary>
    /// Example Mod's global settings.
    /// </summary>
    /// <remarks>These will be saved In the Save Folder as ModName.GlobalSettings.json</remarks>
    public class EnemyList : ModSettings
    {
        private static string[] parryEnemies = new string[] {
            "Infected Knight",          // Broken Vessel
            "Mawlek Body",              // Brooding Mawlek
            "Mawlek Col",               // Brooding Mawlek, but Colosseum
            "Oro",                      // Brothers Oro & Mato
            "Mato",                     // Brothers Oro & Mato
            "Lancer",                   // God Tamer
            "Great Shield Zombie",      // Great Husk Sentry
            //"Sly Boss",                 // Great Nailsage Sly - Not in list because of Phase 2. There is a hard-coded fix in ParryKnight.cs
            "Grimm Boss",               // Grimm
            "Colosseum_Worm",           // Heavy Fool
            "Ruins Sentry Fat",         // Heavy Sentry
            "Ruins Sentry Fat B",       // Heavy Sentry, but B (whatever that means)
            "Ruins Sentry FatB",
            "Hive Knight",              // Hive Knight
            "Hollow Knight Boss",       // The Hollow Knight
            "Hornet Boss 2",            // Hornet Sentinel
            "Zombie Miner 1",           // Husk Miner
            "Ruins Sentry 1",           // Husk Sentry
            "Ruins SentryB 1",
            "Zombie Shield",            // Husk Warrior
            "Zombie Shield 1",          // Husk Warrior, but different?
            "Royal Gaurd",              // Kingsmould
            "Lost Kin",                 // Lost Kin
            "Mantis Heavy",             // Mantis Traitor
            "Mantis Heavy Spawn",
            "Mantis",                   // Mantis Warrior
            "Gate Mantis",              // Gate Mantis (Mantis Warrior in Claw room)
            "Moss Knight",              // Moss Knight
            "Moss Knight B",
            "Moss Knight C",
            "Zombie Myla",              // Myla
            "Nightmare Grimm Boss",     // Nightmare King Grimm
            "Sheo Boss",                // Paintmaster Sheo
            "HK Prime",                 // Pure Vessel
            "Hollow Shade",             // Shade
            "Colosseum_Shield_Zombie",  // Shielded Fool
            "Mage Knight",              // Soul Warrior
            "Slash Spider",             // Stalking Devout
            "Colosseum_Miner",          // Sturdy Fool
            "Mantis Traitor Lord",      // Traitor Lord
            "Black Knight 1",           // Watcher Knights
            "Black Knight 2",
            "Black Knight 3",
            "Black Knight 4",
            "Black Knight 5",
            "Black Knight 6",
            "Colosseum_Flying_Sentry",  // Winged Fool
            "Ruins Flying Sentry",      // Winged Sentry
            "Ruins Flying SentryB"
        };
        public List<string> parriableEnemies = new List<string>(parryEnemies);
    }
}
