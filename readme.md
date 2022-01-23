# Parry Knight Mod

This mod was created with the following principles:
- Only parrying will damage parryable enemies.
- Only spells will damage non-parryable enemies.

In order to make a full-game playthrough possible, aspids and False Knight are unaffected by the above until a spell is acquired.

Parryable enemies will typically take much longer to kill. **An enemy health bar mod is strongly recommended.**

A full list of changes to gameplay is listed at the bottom of the ReadMe.

# Enemies

The following enemies are parryable, meaning at least one of their attacks can be parried:
- Broken Vessel
- Brooding Mawlek
- Brothers Oro & Mato
- God Tamer
- Great Husk Sentry
- Great Nailsage Sly (except in phase 2)
- Heavy Fool
- Heavy Sentry
- Hive Knight
- The Hollow Knight
- Hornet Sentinel
- Husk Miner
- Husk Sentry
- Husk Warrior
- Kingsmould
- Lost Kin
- Mantis Traitor
- Mantis Warrior
- Moss Knight
- Myla
- Nightmare King Grimm
- Paintmaster Sheo
- Pure Vessel
- Shade
- Shielded Fool
- Soul Warrior
- Stalking Devout
- Sturdy Fool
- Traitor Lord
- Troupe Master Grimm
- Watcher Knights
- Winged Fool
- Winged Sentry

Every enemy not listed above must be killed with a spell.

# Gameplay Changes

Here are all the changes to the game made by the mod:
- All parryable enemies, listed in EnemyList.cs, will receive no damage from any source from the player. Parrying them during one of their parryable actions will damage them proportional to your current nail's damage.
- All other enemies will receive no damage from anything sourcing from the player except spells. 
- Nail Arts will now gather soul, since they are otherwise useless.
- Aspids and False Knight can be damaged with the nail until a spell is obtained. This is so that you are not softlocked from the beginning.
- Some ability and ability pickup texts have been modified to convey how they are affected by this mod. For instance, spells now contain the text "Only non-parryable enemies will be damaged by this spell."
- All charms that damage enemies no longer damage enemies, and in some cases, behave differently so that they do not damage enemies. For instance, Grimmchild cannot shoot fireballs. Yes, this means Grimmchild is even more useless than before.
- All charms that affect nail damage will now affect nail parry damage instead. 
- Dream Nail can no longer instakill the shade.
- A few bosses, which noticeably take way too long to kill with only parries, receive a damage multiplier so that they die faster.
- If a player parries an enemy that the mod hasn't anticipated, the player will be notified to send their ModLog.txt to the mod author.