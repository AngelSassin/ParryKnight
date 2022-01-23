using Modding;
using System.Collections.Generic;

namespace ParryKnight
{
    /// <summary>
    /// Example Mod's global settings.
    /// </summary>
    /// <remarks>These will be saved In the Save Folder as ModName.GlobalSettings.json</remarks>
    public class ActionList : ModSettings
    {
        private static string[] parryActions = new string[] {
            "Infected Knight-Dash Attack 1",            // Broken Vessel
            "Infected Knight-Dash Attack 2",
            "Infected Knight-Ohead Slashing",
            "Mawlek Body-THERE IS NONE",                // Brooding Mawlek
            "Mawlek Col-ALSO STILL NONE",               // Brooding Mawlek, but Colosseum
            "Oro-D Slash",                              // Brothers Oro & Mato
            "Oro-Dstab Land",
            "Oro-Combo Slash1",
            "Oro-Combo Slash2",
            "Oro-Combo Slash3",
            "Oro-Combo Slash4",
            "Mato-D Slash",                             // Brothers Oro & Mato
            "Mato-Dstab Land",
            "Mato-Combo Slash1",
            "Mato-Combo Slash2",
            "Mato-Combo Slash3",
            "Mato-Combo Slash4",
            "Lancer-Slash 1",                           // God Tamer
            "Lancer-Slash 2",
            "Great Shield Zombie-Attack1 Slash",        // Great Husk Sentry
            "Great Shield Zombie-Attack2 Slash1",
            "Great Shield Zombie-Attack2 Slash2",
            "Great Shield Zombie-AttackU Slash",
            "Sly Boss-GSlash 1",                        // Great Nailsage Sly
            "Sly Boss-GSlash 2",
            "Sly Boss-BSlash S1",
            "Sly Boss-BSlash S1 2",
            "Sly Boss-BSlash S2",
            "Sly Boss-BSlash S2 2",
            "Sly Boss-Slash S1",
            "Sly Boss-Slash S1 2",
            "Sly Boss-Slash S2",
            "Sly Boss-Slash S2 2",
            "Sly Boss-D Slash S1",
            "Sly Boss-D Slash S1 2",
            "Sly Boss-DSlash S2",
            "Sly Boss-DSlash S2 2",
            "Sly Boss-Dstab Land",
            "Grimm Boss-Slash 1",                       // Grimm
            "Grimm Boss-Slash 2",
            "Grimm Boss-Slash 3",
            "Grimm Boss-Stun",
            "Colosseum_Worm-Single Swipe",              // Heavy Fool
            "Colosseum_Worm-Land",
            "Ruins Sentry Fat-Swipe",                   // Heavy Sentry
            "Ruins Sentry Fat-Single Swipe",
            "Ruins Sentry Fat B-Swipe",                 // Heavy Sentry, but B (whatever that means)
            "Ruins Sentry Fat B-Single Swipe",
            "Ruins Sentry FatB-Swipe",                 
            "Ruins Sentry FatB-Single Swipe",
            "Hive Knight-Slash 1",                      // Hive Knight
            "Hive Knight-Slash 2",
            "Hollow Knight Boss-Slash1",                // The Hollow Knight
            "Hollow Knight Boss-Slash 2",
            "Hollow Knight Boss-Slash 3",
            "Hollow Knight Boss-CSlash",
            "Hollow Knight Boss-Stun Start",
            "Hornet Boss 2-CA 1",                       // Hornet Sentinel
            "Hornet Boss 2-CA 2",
            "Zombie Miner 1-Slash",                     // Husk Miner
            "Ruins Sentry 1-Single Swipe",              // Husk Sentry
            "Ruins Sentry 1-Trip Swipe 1",
            "Ruins Sentry 1-Trip Swipe 2",
            "Ruins Sentry 1-Trip Swipe 3",
            "Ruins SentryB 1-Single Swipe",
            "Ruins SentryB 1-Trip Swipe 1",
            "Ruins SentryB 1-Trip Swipe 2",
            "Ruins SentryB 1-Trip Swipe 3",
            "Zombie Shield-Attack1 Slash",              // Husk Warrior
            "Zombie Shield-A3 Slash 1",
            "Zombie Shield-A3 Lunge 2",
            "Zombie Shield-A3 Slash3",
            "Zombie Shield 1-Attack1 Slash",            // Husk Warrior, but 1 (whatever that means)
            "Zombie Shield 1-A3 Slash 1",
            "Zombie Shield 1-A3 Lunge 2",
            "Zombie Shield 1-A3 Slash3",
            "Royal Gaurd-Slash1 Attack",                // Kingsmould
            "Lost Kin-Dash Attack 1",                   // Lost Kin
            "Lost Kin-Dash Attack 2",
            "Lost Kin-Ohead Slashing",
            "Mantis Heavy-Attack Swipe",                // Mantis Traitor
            "Mantis Heavy Spawn-Attack Swipe",
            "Mantis-Attack Swipe",                      // Mantis Warrior
            "Mantis-Up Slash",
            "Gate Mantis-Attack Swipe",                 // Gate Mantis
            "Gate Mantis-Up Slash",
            "Moss Knight-Attack1 Hitbox On",            // Moss Knight
            "Moss Knight-Attack 2 Slash",
            "Moss Knight B-Attack1 Hitbox On",          
            "Moss Knight B-Attack 2 Slash",
            "Moss Knight C-Attack1 Hitbox On",           
            "Moss Knight C-Attack 2 Slash",
            "Zombie Myla-Slash",                        // Myla
            "Nightmare Grimm Boss-Slash 1",             // Nightmare King Grimm
            "Nightmare Grimm Boss-Slash 2",
            "Nightmare Grimm Boss-Slash 3",
            "Sheo Boss-S 1",                            // Paintmaster Sheo
            "Sheo Boss-S 2",                            
            "Sheo Boss-GSlash 1",
            "Sheo Boss-GSlash 2",
            "Sheo Boss-GSlash 3",
            "HK Prime-Slash1",                          // Pure Vessel
            "HK Prime-Slash 2",
            "HK Prime-Slash 3",
            "HK Prime-CSlash",
            "HK Prime-Stun Start",
            "Hollow Shade-Slash",                       // Shade
            "Colosseum_Shield_Zombie-Attack1 Slash",    // Shielded Fool
            "Colosseum_Shield_Zombie-A3 Slash 1",
            "Colosseum_Shield_Zombie-A3 Lunge 2",
            "Colosseum_Shield_Zombie-A3 Slash3",
            "Mage Knight-Slash",                        // Soul Warrior
            "Mage Knight-Stomp Slash",
            "Slash Spider-A 1",                         // Stalking Devout
            "Slash Spider-A 3",
            "Slash Spider-A 5",
            "Colosseum_Miner-Slash",                    // Sturdy Fool
            "Mantis Traitor Lord-Attack Swipe",         // Traitor Lord
            "Black Knight 1-Slash1",                    // Watcher Knights
            "Black Knight 1-Slash2",
            "Black Knight 2-Slash1",
            "Black Knight 2-Slash2",
            "Black Knight 3-Slash1",
            "Black Knight 3-Slash2",
            "Black Knight 4-Slash1",
            "Black Knight 4-Slash2",
            "Black Knight 5-Slash1",
            "Black Knight 5-Slash2",
            "Black Knight 6-Slash1",
            "Black Knight 6-Slash2",
            "Colosseum_Flying_Sentry-Slash",            // Winged Fool
            "Ruins Flying Sentry-Slash",                // Winged Sentry
            "Ruins Flying SentryB-Slash",              
        };
        public List<string> parriableActions = new List<string>(parryActions);
    }
}
