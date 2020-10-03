Basic Description
------------
Two new equipment: Untapped Potential and Tempestuous Rage.
UP is a basic equipment that will give the user an elite buff for the rest of the stage then destroy itself.
TR is a lunar equipment that will give the user the elite buff of the most recent elite that hurt them. This buff will be kept between stages, but also comes with a curse that increases ALL damage taken.
The curses from TR can be temporarily reduced by gaining the elite buff via UP, Wake of Vultures, or the normal Elite Equipment. (The percent damage increase per curse is configurable)

Planned Features
------------
Some UI element to track curses and elite effects (beside the buff icons)
When buffs are given by UP or TR, give the elite equip display

Note for Mod Devs
------------
If you want to add compatibility for AffixGen to your own elite buff types, I've made the library that holds the affix trackers public to aid in this.
All you should need to do is add a soft dependency to this plugin and after you register your elite types, the buffs, and their equipment, you should be able to add a new AffixTracker to AffixTrackerLib.affixTrackers.
This AffixTracker class is relatively self-explanatory, but the details are as follows: (More details can be found in the repo)
- eliteIndex, buffIndex, and equipmentIndex should all match your EliteIndex, BuffIndex, and EquipmentIndex
- The bools and floats can be ignored as they're used for the internal maths
- loopsRequired is the number of loops the the player must complete to unlock the affix (in base game the Poison and Ghost types have a loop requirement of 1)
- affixNameTag is a string that is currently unused, but may eventually be used in an UI element to let the player know which affix they recieved

Changelog
------------
2.1.2 - Added some more error-proofing since the last assumption wasn't the only one that was wrong
2.1.1 - Fixed an error that occured due to my assumption that every HealthComponent has a CharacterBody
2.1.0 - Major refactor to hook registration, turrets and drones are re-enabled and should be lag-free
2.0.5 - Changed hook to check if player controlled, should prevent odd behaviour with drones/turrets/clones
2.0.4 - Added a check to prevent Drones and Turrets from registering hooks
2.0.3 - Accidentally disabled all other ItemBehaviours in 2.0.2, fixed that
2.0.2 - Fixed a networking bug that gave clients faulty buff icons
2.0.1 - Fixed a bug that prevented multiple runs without relaunching the game
2.0.0 - Complete redesign, expect day-one bugs
1.1.3 - Fixed a bug that was causing a bunch of issues
1.1.2 - Fixed a bug with Rex and DoT type damage
1.1.1 - Fixed bug that may cause global invincibility
1.1.0 - Added Configuration
1.0.1 - Refactored to prevent infinte loop with LightningOrb
1.0.0 - Initial Upload

Installation
------------
Place the .dll in Risk of Rain 2\BepInEx\plugins or use a mod manager.

Contact
------------
If you have issues/suggestions leave them on the github as an issue/suggestion or reach out to @Rico#6416 on the modding Discord.