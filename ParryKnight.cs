using System;
using System.Collections.Generic;
using System.Reflection;
using GlobalEnums;
using Modding;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ModCommon.Util;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Language;

namespace ParryKnight
{
    /// <summary>
    /// The main mod class
    /// </summary>
    /// <remarks>This configuration has settings that are save specific and global (profile) too.</remarks>
    public class ParryKnight : Mod
    {
        /// <summary>
        /// Fetches the Mod Version From AssemblyInfo.AssemblyVersion
        /// </summary>
        /// <returns>Mod's Version</returns>
        public override string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        private List<GameObject> enemies = new List<GameObject>();
        private bool slyPhase2 = false;

        private ActionList _actionList = new ActionList();
        private EnemyList _enemyList = new EnemyList();

        internal static ParryKnight Instance;

        private bool badParry = false;
        private bool textParry = false;

        public override void Initialize()
        {
            Instance = this;
            Log("Initializing");

            On.HutongGames.PlayMaker.Actions.IntOperator.OnEnter += (orig, self) =>
            {
                if (self.Fsm.GameObject.name == "Enemy Damager" && self.Fsm.Name == "Attack" && self.State.Name == "Hit")
                {
                    self.integer2 = 0; // No Weaverling Damage
                }

                if (self.Fsm.GameObject.name == "Shield" && self.Fsm.Name == "Shield Hit" && self.State.Name == "Hit")
                {
                    self.integer2 = 0; // No Dreamshield Damage
                }

                orig(self);
            };

            On.HealthManager.TakeDamage += (orig, self, hitInstance) =>
            {
                string name = self.name;
                if (name.Contains("("))
                    name = name.Substring(0, name.IndexOf("(")).Trim();
                PlayerData pd = PlayerData.instance;
                if (hitInstance.AttackType.Equals(AttackTypes.Nail) && ( // Nail Arts
                        hitInstance.Source.name.Equals("Hit L") ||
                        hitInstance.Source.name.Equals("Hit R") ||
                        hitInstance.Source.name.Equals("Great Slash") ||
                        hitInstance.Source.name.Equals("Dash Slash")
                    ))
                {
                    int tmpInt = PlayerData.instance.GetInt("MPReserve");
                    int additionalCharge = 6; // Gain additional soul on Nail Art
                    if (tmpInt == 0)
                    {
                        additionalCharge += 5;
                    }
                    if (hitInstance.Source.name.Equals("Hit L") || hitInstance.Source.name.Equals("Hit R"))
                    {
                        additionalCharge = 5;
                    }
                    if (PlayerData.instance.GetBool("equippedCharm_26")) // If Nailmaster's Glory is equipped, Gain even more additional soul on Nail Art.
                    {
                        additionalCharge *= 2;
                    }
                    PlayerData.instance.AddMPCharge(additionalCharge);
                    GameCameras.instance.soulOrbFSM.SendEvent("MP GAIN"); // Update visuals
                    if (PlayerData.instance.GetInt("MPReserve") != tmpInt)
                    {
                        GameManager.instance.soulVessel_fsm.SendEvent("MP RESERVE UP"); // Update visuals
                    }
                }
                if ((hitInstance.AttackType.Equals(AttackTypes.Generic) && hitInstance.Source.name.Equals("Damager" /*??? I 'ardly even know 'er!! */)) || // Glowing Womb
                    (hitInstance.AttackType.Equals(AttackTypes.Generic) && ( // Thorns of Agony
                        hitInstance.Source.name.Equals("Hit U") ||
                        hitInstance.Source.name.Equals("Hit D") ||
                        hitInstance.Source.name.Equals("Hit L") ||
                        hitInstance.Source.name.Equals("Hit R")
                    )) 
                ) 
                {
                    hitInstance.DamageDealt = 0;
                }
                if (hitInstance.AttackType.Equals(AttackTypes.Generic) && (
                    hitInstance.Source.name.Equals("SuperDash Damage") || hitInstance.Source.name.Equals("SD Burst"))) // Crystal Dash
                {
                    hitInstance.DamageDealt = 0;
                }
                if (_enemyList.parriableEnemies.Contains(name) || // If an enemy is fully parriable,
                    (name.Equals("Sly Boss") && !slyPhase2)) // or Sly isn't in Phase 2
                {
                    if (hitInstance.AttackType.Equals(AttackTypes.Nail) ||
                            hitInstance.AttackType.Equals(AttackTypes.NailBeam) ||
                            hitInstance.AttackType.Equals(AttackTypes.SharpShadow) ||
                            hitInstance.AttackType.Equals(AttackTypes.Spell))
                    {
                        hitInstance.DamageDealt = 0;
                    }
                }
                else if (pd.fireballLevel == 0 && (
                    name.Equals("Spitter") || // Aspids (to escape the Aspid Arena)
                    name.Equals("False Knight New") || // False Knight, and his Head
                    name.Equals("Head")
                    ))
                {
                    orig(self, hitInstance);
                    return;
                }
                else // If an enemy is NOT parriable
                {
                    if (hitInstance.AttackType.Equals(AttackTypes.Nail) ||
                            hitInstance.AttackType.Equals(AttackTypes.NailBeam) ||
                            hitInstance.AttackType.Equals(AttackTypes.SharpShadow))
                    {
                        hitInstance.DamageDealt = 0;
                    }
                }
                if (name.Equals("Parasite Balloon Spawner") && self.hp <= 0 && !hitInstance.AttackType.Equals(AttackTypes.Spell))
                { // Parasite Balloons in the Broken Vessel and Lost Kin fights spawn with negative health if previously damaged. 
                    self.hp = 1;
                }

                if (hitInstance.DamageDealt != 0)
                {
                    Log("HIT LOGGED: " + hitInstance.AttackType + "    " + name + "    " + hitInstance.Source);
                    Log("ATTACK TYPE " + hitInstance.AttackType + "    " + hitInstance.Source.name);
                }
                    
                orig(self, hitInstance);
            };

            On.HeroController.NailParry += (orig, self) => {
                bool parryOccurred = false;
                List<string> logActions = new List<string>();
                string nameList = "";
                foreach (GameObject enemy in enemies)
                {
                    if (!enemy)
                        continue;
                    PlayMakerFSM fsm = enemy.GetComponent<PlayMakerFSM>();
                    if (!fsm)
                        continue;
                    string state = fsm.ActiveStateName;
                    string name = enemy.name;
                    if (name.Contains("("))
                        name = name.Substring(0, name.IndexOf("(")).Trim();
                    nameList += name + ", ";
                    string log = "PARRY     " + name + "     " + name + "-" + state;
                    logActions.Add(log);
                    // If an enemy is performing a FSM action that is parriable,
                    // OR one of these weird cases:
                    // - Brooding Mawlek is in the room! Mawlek has no FSM that's parriable, but its body parts have separate actions that are parriable. Weird case.
                    if (_actionList.parriableActions.Contains(name + "-" + state) ||
                            name == "Mawlek Body" ||
                            name == "Mawlek Col")
                    {
                        parryOccurred = true;
                        HealthManager enemyHealth = enemy.GetComponent<HealthManager>();
                        if (enemyHealth)
                        {
                            double bossMultiplier = 1; // Some bosses are just stupidly boring because parries hardly ever occur. Let's make them killable faster.
                            switch (name)
                            {
                                case "Hornet Boss 2":
                                case "Grimm Boss":
                                case "Nightmare Grimm Boss":
                                case "Hive Knight": 
                                    bossMultiplier = 2; 
                                    break;
                                case "Sheo Boss":
                                    bossMultiplier = 1.5;
                                    break;
                                default:
                                    bossMultiplier = 1;
                                    break;
                            }
                            int damage = 5 + PlayerData.instance.nailSmithUpgrades * 4;
                            if (PlayerData.instance.GetBool("equippedCharm_25")) // If Strength Charm is equipped
                                damage = (int)(damage * 1.5);
                            if (PlayerData.instance.GetBool("equippedCharm_6") && PlayerData.instance.health == 1) // If Fury of the Fallen is equipped and hp = 1
                                damage = (int)(damage * 1.75);
                            enemyHealth.ApplyExtraDamage((int) (damage * bossMultiplier));
                            if (name.Equals("Sly Boss") && enemyHealth.hp <= 0)
                            { // If Sly has been parried and drops to zero health, detect phase 2 (to make him damageable by spells)
                                slyPhase2 = true;
                            }
                        }
                    }
                }
                //Log("ENEMIES    " + nameList);
                if (!parryOccurred)
                {
                    // THERE WAS A HUGE OVERSIGHT! You just parried an enemy in a way that wasn't accounted for! 
                    // Add some sort of on-screen indicator for the player to see that damage should have occurred
                    // and that the player should send the log file to me!!!
                    foreach (string log in logActions)
                    {
                        Log("NOT LOGGED!!! " + log);
                        badParry = true;
                    }
                }
                if (badParry && !textParry)
                {
                    CanvasUtil.CreateFonts();
                    GameObject c = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920, 1080));
                    Text ohNo = CanvasUtil.CreateTextPanel(c, "OH NO! You found a parry the mod dev didn't know about!",
                        35, TextAnchor.MiddleCenter, new CanvasUtil.RectData(
                        new Vector2(1500, 1500), new Vector2(0f, 90), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0.5f)
                    ), true).GetComponent<Text>();
                    Text sendLog = CanvasUtil.CreateTextPanel(c, "After closing the game, send your ModLog to AngelSassin in the HK Discord.",
                        25, TextAnchor.MiddleCenter, new CanvasUtil.RectData(
                        new Vector2(1500, 1500), new Vector2(0f, 50), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0.5f)
                    ), true).GetComponent<Text>(); 
                    Text logLocation = CanvasUtil.CreateTextPanel(c, "(...\\LocalLow\\Team Cherry\\Hollow Knight\\ModLog.txt)",
                        18, TextAnchor.MiddleCenter, new CanvasUtil.RectData(
                        new Vector2(1500, 1500), new Vector2(0f, 20), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0.5f)
                    ), true).GetComponent<Text>();
                    textParry = true;
                }
                orig(self);
            };

            ModHooks.Instance.ApplicationQuitHook += ApplicationQuitHook;
            ModHooks.Instance.OnEnableEnemyHook += EnemyEnabled;
            ModHooks.Instance.BeforeSceneLoadHook += BeforeSceneLoad;
            On.PlayMakerFSM.OnEnable += OnFsmEnable;
            On.SpellFluke.DoDamage += OnFlukeDamage;
            ModHooks.Instance.LanguageGetHook += LanguageGet;

            Log("Initialized");
        }


        private void OnFlukeDamage(On.SpellFluke.orig_DoDamage orig, SpellFluke self, GameObject obj, int recursion, bool burst)
        {
            string name = obj.name;
            if (name.Contains("("))
                name = name.Substring(0, name.IndexOf("(")).Trim();
            if (_enemyList.parriableEnemies.Contains(name) || // If an enemy is fully parriable,
                    (name.Equals("Sly Boss") && !slyPhase2)) // or Sly isn't in Phase 2
            {
                self.SetAttr("damage", 0);
            }
            orig(self, obj, recursion, burst);
        }

        private void ApplicationQuitHook()
        {
            SaveGlobalSettings();
        }

        public bool EnemyEnabled(GameObject enemy, bool isDead)
        {
            //Log(">>> ENEMY " + enemy.name + " LOADED");
            if (isDead)
                return isDead;
            enemies.Add(enemy);
            return false;
        }

        public string BeforeSceneLoad(string sceneName)
        {
            enemies.Clear();
            slyPhase2 = false;
            return sceneName;
        }

        private void OnFsmEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);

            string name = self.gameObject.name;
            if (name.Contains("("))
                name = name.Substring(0, name.IndexOf("(")).Trim();

            if (name == "Hollow Shade" && self.FsmName == "Dreamnail Kill") // Stop Shade from dying to dream nail. It MUST be parried!
            {
                self.RemoveAction("Idle", 0);
                self.RemoveTransition("Idle", "Die");
            }

            if (name == "Knight" && self.FsmName == "Spell Control") // Stop Spore and Dung Clouds from spawning when using Focus Spell
            {
                self.RemoveAction("Spore Cloud", 3);
                self.RemoveAction("Dung Cloud", 0);
                self.RemoveAction("Spore Cloud 2", 3);
                self.RemoveAction("Dung Cloud 2", 0);
            }

            if (name == "Dung" && self.FsmName == "Control") // Stop Dung Trail from being spawned
            {
                self.RemoveTransition("Emit Pause", "Equipped");
            }

            if (name == "Grimmchild" && self.FsmName == "Control") // Stop Grimmchild from firing projectiles
            {
                self.GetAction<GameObjectIsNull>("Check For Target", 2).isNotNull = self.GetAction<GameObjectIsNull>("Check For Target", 2).isNull;
            }

            if (name == "Spell Fluke Dung Lv2" && self.FsmName == "Control") // Stop Dung Fluke from creating cloud
            {
                try
                {
                    self.RemoveAction("Blow", 14);
                    self.GetAction<Wait>("Blow", 11).time = 1;
                    self.RemoveAction("Blow", 10);
                    self.RemoveAction("Blow", 9);
                    self.RemoveAction("Blow", 8);
                    self.RemoveAction("Blow", 7);
                    self.RemoveAction("Blow", 6);
                    self.RemoveAction("Blow", 5);
                    self.RemoveAction("Blow", 2);
                    self.RemoveAction("Blow", 1);
                    self.RemoveAction("Blow", 0);
                }
                catch (IndexOutOfRangeException e)
                {

                }
            }
            if (name == "Spell Fluke Dung Lv1" && self.FsmName == "Control") // Stop Dung Fluke from creating cloud
            {
                try
                {
                    self.RemoveAction("Blow", 14);
                    self.RemoveAction("Blow", 13);
                    self.RemoveAction("Blow", 12);
                    self.GetAction<Wait>("Blow", 9).time = 1;
                    self.RemoveAction("Blow", 8);
                    self.RemoveAction("Blow", 7);
                    self.RemoveAction("Blow", 6);
                    self.RemoveAction("Blow", 5);
                    self.RemoveAction("Blow", 4);
                    self.RemoveAction("Blow", 1);
                    self.RemoveAction("Blow", 0);
                }
                catch (IndexOutOfRangeException e)
                {

                }
            }
            if (name == "Spell Fluke Dung" && self.FsmName == "Control") // Stop Dung Fluke from creating cloud
            {
                try
                {
                    self.RemoveAction("Blow", 12);
                    self.GetAction<Wait>("Blow", 9).time = 1;
                    self.RemoveAction("Blow", 8);
                    self.RemoveAction("Blow", 7);
                    self.RemoveAction("Blow", 6);
                    self.RemoveAction("Blow", 5);
                    self.RemoveAction("Blow", 3);
                    self.RemoveAction("Blow", 2);
                }
                catch (IndexOutOfRangeException e)
                {

                }
            }
        }

        public string LanguageGet(string key, string sheet)
        {
            Log("KEY: `" + key + "` displays this text: " + Language.Language.GetInternal(key, sheet));

            // OTHER DESCRIPTIONS
            if (key == "DIRTMOUTH_MAIN")
                return "Parry Knight";
            if (key == "DIRTMOUTH_SUB")
                return "Only Parrying Deals Damage";
            if (key == "FALSE_KNIGHT_DREAM_MAIN")
                return "Failed Chump";
            if (key == "FALSE_KNIGHT_DREAM_SUB")
                return "Nail Arts are your friend";

            // NAIL DESCRIPTIONS
            if (key == "INV_DESC_NAIL1")
                return "A traditional weapon of Hallownest. Its blade is blunt with age and wear.<br><br>Damage is only dealt by parrying. Nail strikes will gather soul.";
            if (key == "INV_DESC_NAIL2")
                return "A traditional weapon of Hallownest, restored to lethal form.<br><br>Damage is only dealt by parrying. Nail strikes will gather soul.";
            if (key == "INV_DESC_NAIL3")
                return "A cleft weapon of Hallownest. The blade is exquisitly balanced.<br><br>Damage is only dealt by parrying. Nail strikes will gather soul.";
            if (key == "INV_DESC_NAIL4")
                return "A powerful weapon of Hallownest, refined beyond all others.<br><br>Damage is only dealt by parrying. Nail strikes will gather soul.";
            if (key == "INV_DESC_NAIL5")
                return "The ultimate weapon of Hallownest. Crafted to perfection, this ancient nail reveals its true form.<br><br>Damage is only dealt by parrying. Nail strikes will gather soul.";

            // GET SPELL DESCRIPTIONS
            if (key == "GET_FIREBALL_2")
                return "Spells will deplete SOUL. Replenish SOUL by striking enemies.<br>Only non-parryable enemies will be damaged by this spell.";
            if (key == "GET_FIREBALL2_2")
                return "Spells will deplete SOUL. Replenish SOUL by striking enemies.<br>Only non-parryable enemies will be damaged by this spell.";
            if (key == "GET_QUAKE_2")
                return "Spells will deplete SOUL. Replenish SOUL by striking enemies.<br>Only non-parryable enemies will be damaged by this spell.";
            if (key == "GET_QUAKE2_2")
                return "Spells will deplete SOUL. Replenish SOUL by striking enemies.<br>Only non-parryable enemies will be damaged by this spell.";
            if (key == "GET_SCREAM_2")
                return "Spells will deplete SOUL. Replenish SOUL by striking enemies.<br>Only non-parryable enemies will be damaged by this spell.";
            if (key == "GET_SCREAM2_2")
                return "Spells will deplete SOUL. Replenish SOUL by striking enemies.<br>Only non-parryable enemies will be damaged by this spell.";

            // GET NAIL ART DESCRIPTIONS
            if (key == "GET_CYCLONE_2")
                return "Release while holding UP or DOWN to perform the Cyclone Slash.<br>Nail Arts deal no damage, but they will gather a lot of soul.";
            if (key == "GET_DSLASH_2")
                return "Release the button while dashing to perform the Dash Slash.<br>Nail Arts deal no damage, but they will gather a lot of soul.";
            if (key == "GET_GSLASH_2")
                return "Release the button without holding UP or DOWN to perform the Great Slash.<br>Nail Arts deal no damage, but they will gather a lot of soul.";

            // SPELL DESCRIPTIONS
            if (key == "INV_DESC_SPELL_FIREBALL1")
                return "Conjure a spirit that will fly forward and burn foes in its path.<br><br>The spirit requires SOUL to be conjured. Strike enemies to gather SOUL.<br><br>Only non-parryable enemies will be damaged by this spell.";
            if (key == "INV_DESC_SPELL_FIREBALL2")
                return "Conjure a shadow that will fly forward and burn foes in its path.<br><br>The shadow requires SOUL to be conjured. Strike enemies to gather SOUL.<br><br>Only non-parryable enemies will be damaged by this spell.";
            if (key == "INV_DESC_SPELL_QUAKE1")
                return "Strike the ground with a concentrated force of SOUL. This force can destroy foes or break through fragile structures.<br><br>The force requires SOUL to be conjured. Strike enemies to gather SOUL.<br><br>Only non-parryable enemies will be damaged by this spell.";
            if (key == "INV_DESC_SPELL_QUAKE2")
                return "Strike the ground with a concentrated force of SOUL and Shadow. This force can destroy foes or break through fragile structures.<br><br>The force requires SOUL to be conjured. Strike enemies to gather SOUL.<br><br>Only non-parryable enemies will be damaged by this spell.";
            if (key == "INV_DESC_SPELL_SCREAM1")
                return "Blast foes with screaming SOUL.<br><br>The Wraiths requires SOUL to be conjured. Strike enemies to gather SOUL.<br><br>Only non-parryable enemies will be damaged by this spell.";
            if (key == "INV_DESC_SPELL_SCREAM2")
                return "Blast foes with screaming SOUL and Shadow.<br><br>The Wraiths requires SOUL to be conjured. Strike enemies to gather SOUL.<br><br>Only non-parryable enemies will be damaged by this spell.";

            // NAIL ARTS DESCRIPTIONS
            if (key == "INV_DESC_ART_CYCLONE")
                return "The signature Nail Art of Nailmaster Mato. A spinning attack that rapidly strikes foes on all sides.<br><br>Nail Arts deal no damage, but they will gather more soul than a normal nail strike.";
            if (key == "INV_DESC_ART_UPPER")
                return "The signature Nail Art of Nailmaster Oro. Strike ahead quickly after dashing forward.<br><br>Nail Arts deal no damage, but they will gather more soul than a normal nail strike.";
            if (key == "INV_DESC_ART_DASH")
                return "The signature Nail Art of Nailmaster Sheo. Unleashes a huge slash directly in front of you which deals extra damage to foes.<br><br>Nail Arts deal no damage, but they will gather more soul than a normal nail strike.";

            // CHARM DESCRIPTIONS
            if (key == "CHARM_DESC_35")
                return "Contains the gratitude of grubs who will move to the next stage of their lives. Imbues weapons with a holy strength.<br><br>When the bearer is at full health, they will fire beams of white-hot energy from their nail.<br><br>This beam deals no damage.";
            if (key == "CHARM_DESC_6")
                return "Embodies the fury and heroism that comes upon those who are about to die.<br><br>When close to death, parrying will deal more damage.";
            if (key == "CHARM_DESC_25")
                return "Strengthens the bearer, increasing the damage they deal to enemies.<br><br>This charm is fragile, and will break if its bearer is killed.<br><br>When equipped, parrying will deal more damage.";
            if (key == "CHARM_DESC_25_G")
                return "Strengthens the bearer, increasing the damage they deal to enemies.<br><br>This charm is unbreakable.<br><br>When equipped, parrying will deal more damage.";
            if (key == "CHARM_DESC_12")
                return "Senses the pain of its bearer and lashes out at the world around them.<br><br>When taking damage, sprout thorny vines.<br><br>Vines deal no damage.";
            if (key == "CHARM_DESC_11")
                return "Living charm born in the gut of a Flukemarm.<br><br>Transforms the Vengeful Spirit spell into a horde of volatile baby flukes.<br><br>Only non-parryable enemies will be damaged by flukes.";
            if (key == "CHARM_DESC_10")
                return "Unique charm bestowed by the King of Hallownest to his most loyal knight. Scratched and dirty, but still cared for.<br><br>Causes the bearer to emit a heroic odour.<br><br>The odor deals no damage.";
            if (key == "CHARM_DESC_22")
                return "Drains the SOUL of its bearer and uses it to birth hatchlings.<br><br>The hatchlings have no desire to eat or live, and will sacrifice themselves to protect their parent.<br><br>Hatchlings deal no damage.";
            if (key == "CHARM_DESC_38")
                return "Defensive charm once wielded by a tribe that could shape dreams.<br><br>Conjures a shield that follows the bearer and attempts to protect them.<br><br>The shield deals no damage.";
            if (key == "CHARM_DESC_39")
                return "Silken charm containing a song of farewell, left by the Weavers who departed Hallownest for their old home.<br><br>Summons weaverlings to give the lonely bearer some companionship and protection.<br><br>Weaverlings deal no damage.";
            if (key == "CHARM_DESC_26")
                return "Contains the passion, skill and regrets of a Nailmaster.<br><br>Increases the bearer's mastery of Nail Arts, allowing them to focus their power faster and unleash arts sooner.<br><br>Nail Arts will gather soul more effectively.";
            if (key == "CHARM_DESC_16")
                return "Contains a forbidden spell that transforms shadows into deadly weapons.<br><br>When using Shadow Dash, the bearer's body will sharpen.<br><br>Shadow Dashing deals no damage.";
            if (key == "CHARM_DESC_17")
                return "Composed of living fungal matter.<br><br>Spore Shroom deals no damage.";
            if (key == "CHARM_DESC_40")
                return "Worn by those who take part in the Grimm Troupe's Ritual.<br><br>The bearer must seek the Grimmkin and collect their flames. Uncollected flames will appear on the bearer's map.<br><br>Grimmchild will never deal damage.";
            if (key == "CHARM_DESC_40_F")
                return "Symbol of a completed ritual. Contains a living, scarlet flame.<br><br>Grimmchild deals no damage.";

            return Language.Language.GetInternal(key, sheet);
        }
    }
}
