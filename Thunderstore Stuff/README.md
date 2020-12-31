Basic Description
------------
Two new equipment: Untapped Potential and Tempestuous Rage.
UP is a basic equipment that will give the user an elite buff for the rest of the stage then destroy itself.
TR is a lunar equipment that will give the user the elite buff of the most recent elite that hurt them. This buff will be kept between stages, but also comes with a curse that increases ALL damage taken.
The curses from TR can be temporarily reduced by gaining the elite buff via UP, Wake of Vultures, or the normal Elite Equipment. (The percent damage increase per curse is configurable)

Planned Features
------------
Improve/replace the buff icon for the "Curse", its a placeholder now (reach out if interested)
When buffs are given by UP or TR, give the elite equip display

Note for Mod Devs
------------
If you want to add compatibility for AffixGen to your own elite buff types, I've made the library that holds the affix trackers public to aid in this.
All you should need to do is add a soft dependency to this plugin and after you register your elite types, the buffs, and their equipment, you should be able to add a new AffixTracker to AffixTrackerLib.affixTrackers.
The AffixTracker class and its constructors are self-documented, so it should be fairly easy to make them. Reach out if there's something that doesn't make sense.

Changelog
------------
2.2.0 - Minor refactor to tracking system, added visual buff for the "Curse" mechanic, new notes for compatibility
2.1.3 - Did a bit of code cleanup, no new functionality, just making the code a bit lighter and faster
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