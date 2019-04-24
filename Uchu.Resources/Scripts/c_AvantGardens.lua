--------------------------------------------------------------
-- Constants/Settings for Avant Gardens Scripts
--------------------------------------------------------------

CONSTANTS = {}
--------------------------------------------------------------
-- Other
--------------------------------------------------------------
CONSTANTS["NO_OBJECT"] = "0"
CONSTANTS["EMPTY_ID_NAME"] = "|" .. CONSTANTS["NO_OBJECT"]
CONSTANTS["LOT_NULL"] = -1

--------------------------------------------------------------
-- Scene 1
--------------------------------------------------------------


--------------------------------------------------------------
-- Scene 2 constants
--------------------------------------------------------------
CONSTANTS["COUNTDOWN_TIME"] = 3.0


--CONSTANTS["S2_EXIT_COURSE_TEXT"] = Localize("UI_OBSTACLE_COURSE_EXIT")
--CONSTANTS["S2_START_COURSE_TEXT"] =  Localize("UI_OBSTACLE_COURSE_START")
--CONSTANTS["S2_OUT_OF_RANGE_COURSE_TEXT"] = Localize("UI_OBSTACLE_COURSE_OUT_OF_RANGE")
--CONSTANTS["S2_FINISH_COURSE_TEXT"] = Localize("UI_OBSTACLE_COURSE_FINISH")


--------------------------------------------------------------
-- Scene 2
-- Kipper commercial constants
--------------------------------------------------------------

-- the LOT's of the NPC's
CONSTANTS["KIPPER_DUEL_SENTINEL_NPC_LOT"] 		= 4776
CONSTANTS["KIPPER_DUEL_PARADOX_NPC_LOT"] 		= 4777
CONSTANTS["KIPPER_SPECTATOR_ASSEMBLY_NPC_LOT"] 	= 4790

-- how long to hold on each model before asking for the next transformation
CONSTANTS["KIPPER_DUEL_TIME_BETWEEN_MODELS"] = 3.5

-- whether or not the sentinel NPC goes first (mouse)
-- if false, then paradox goes first
CONSTANTS["KIPPER_DUEL_SENTINEL_GOES_FIRST"] = true

-- the LOT's for the models produced by the two dueling NPC's
CONSTANTS["KIPPER_DUEL_MOUSE_LOT"] 			= 4779
CONSTANTS["KIPPER_DUEL_CAT_LOT"] 			= 4780
CONSTANTS["KIPPER_DUEL_DOG_LOT"] 			= 4781
CONSTANTS["KIPPER_DUEL_DRAGON_LOT"] 		= 4782
CONSTANTS["KIPPER_DUEL_FIRE_ENGINE_LOT"] 	= 4783
CONSTANTS["KIPPER_DUEL_SUBMARINE_LOT"] 		= 4784
CONSTANTS["KIPPER_DUEL_KIPPER_LOT"] 		= 4786
CONSTANTS["KIPPER_DUEL_ELEPHANT_LOT"] 		= 4789

-- how long to wait after smashing off the old bricks before spawning in any additional ones needed
CONSTANTS["KIPPER_DUEL_ADDITIONAL_BRICKS_DELAY"] 	= 0.5

-- the max x and z offset to spawn each brick at compared to its final location in the completed model
CONSTANTS["KIPPER_DUEL_MAX_BRICK_POS_OFFSET"] 		= 5.0

-- how far away you can click on one of the NPC's to trigger text
CONSTANTS["KIPPER_NPC_INTERACT_DISTANCE"] = 35.0

-- the mission number to talk to the reporter in Zorillo
CONSTANTS["KIPPER_MISSION_ID"] = 214

-- the anim names for the dueling NPC's
CONSTANTS["KIPPER_ANIM_GLOAT"] 		= "good"
CONSTANTS["KIPPER_ANIM_POUT"] 		= "bad"

-- the anim names for the spectator NPC
CONSTANTS["KIPPER_ANIM_LEFT"] 		= "talk-left"
CONSTANTS["KIPPER_ANIM_RIGHT"] 		= "talk-right"

-- whether the spectator considers the sentinel NPC to the left
CONSTANTS["SENTINEL_IS_LEFT_OF_SPECTATOR"] 		= true




--------------------------------------------------------------
-- Scene 2
-- constants for Burno's hotdog cart
--------------------------------------------------------------
CONSTANTS["LOT_BURNO"] = 4045
CONSTANTS["LOT_PATH_UNDER_BURNO"] 	= 4814
CONSTANTS["LOT_BURNO_HOTDOG_CART"] 	= 4046




--------------------------------------------------------------
-- Scene 3
-- constants for the musical instruments quickbuilds
--------------------------------------------------------------

-- the object template for each instrument
CONSTANTS["INSTRUMENT_LOT_GUITAR"] 		= 4039
CONSTANTS["INSTRUMENT_LOT_BASS"]		= 4040
CONSTANTS["INSTRUMENT_LOT_KEYBOARD"] 	= 4041
CONSTANTS["INSTRUMENT_LOT_DRUM"] 		= 4042

-- the anim used to show the player playing each instrument
CONSTANTS["INSTRUMENT_ANIM"] = {}
CONSTANTS["INSTRUMENT_ANIM"][CONSTANTS["INSTRUMENT_LOT_GUITAR"]] 			= "guitar"
CONSTANTS["INSTRUMENT_ANIM"][CONSTANTS["INSTRUMENT_LOT_BASS"]] 				= "bass"
CONSTANTS["INSTRUMENT_ANIM"][CONSTANTS["INSTRUMENT_LOT_KEYBOARD"]] 			= "keyboard"
CONSTANTS["INSTRUMENT_ANIM"][CONSTANTS["INSTRUMENT_LOT_DRUM"]] 				= "drums"

-- the anim used to show the player smashing each instrument
CONSTANTS["INSTRUMENT_SMASH_ANIM"] = {}
CONSTANTS["INSTRUMENT_SMASH_ANIM"][CONSTANTS["INSTRUMENT_LOT_GUITAR"]] 		= "guitar-smash"
CONSTANTS["INSTRUMENT_SMASH_ANIM"][CONSTANTS["INSTRUMENT_LOT_BASS"]] 		= "bass-smash"
CONSTANTS["INSTRUMENT_SMASH_ANIM"][CONSTANTS["INSTRUMENT_LOT_KEYBOARD"]] 	= "keyboard-smash"
CONSTANTS["INSTRUMENT_SMASH_ANIM"][CONSTANTS["INSTRUMENT_LOT_DRUM"]] 		= -1		-- has no smashing anim

-- the music used for each instrument
CONSTANTS["INSTRUMENT_MUSIC"] = {}
CONSTANTS["INSTRUMENT_MUSIC"][CONSTANTS["INSTRUMENT_LOT_GUITAR"]]			= "Concert_Guitar"
CONSTANTS["INSTRUMENT_MUSIC"][CONSTANTS["INSTRUMENT_LOT_BASS"]] 			= "Concert_Bass"
CONSTANTS["INSTRUMENT_MUSIC"][CONSTANTS["INSTRUMENT_LOT_KEYBOARD"]] 		= "Concert_Keys"
CONSTANTS["INSTRUMENT_MUSIC"][CONSTANTS["INSTRUMENT_LOT_DRUM"]] 			= "Concert_Drums"

-- path set up through Happy Flower for the cinematic for each instrument
CONSTANTS["INSTRUMENT_CINEMATIC"] = {}
CONSTANTS["INSTRUMENT_CINEMATIC"][CONSTANTS["INSTRUMENT_LOT_GUITAR"]] 		= "Concert_Cam_G"
CONSTANTS["INSTRUMENT_CINEMATIC"][CONSTANTS["INSTRUMENT_LOT_BASS"]] 		= "Concert_Cam_B"
CONSTANTS["INSTRUMENT_CINEMATIC"][CONSTANTS["INSTRUMENT_LOT_KEYBOARD"]] 	= "Concert_Cam_K"
CONSTANTS["INSTRUMENT_CINEMATIC"][CONSTANTS["INSTRUMENT_LOT_DRUM"]] 		= "Concert_Cam_D"

-- the LOT for the left hand equippable item used for each instrument
CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_LEFT"] = {}
CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_LEFT"][CONSTANTS["INSTRUMENT_LOT_GUITAR"]] 	= 4991
CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_LEFT"][CONSTANTS["INSTRUMENT_LOT_BASS"]] 		= 4992
CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_LEFT"][CONSTANTS["INSTRUMENT_LOT_KEYBOARD"]] 	= -1		-- no equippable used
CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_LEFT"][CONSTANTS["INSTRUMENT_LOT_DRUM"]] 		= 4995

-- the LOT for the right hand equippable item used for each instrument
CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_RIGHT"] = {}
CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_RIGHT"][CONSTANTS["INSTRUMENT_LOT_GUITAR"]] 	= -1
CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_RIGHT"][CONSTANTS["INSTRUMENT_LOT_BASS"]] 		= -1
CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_RIGHT"][CONSTANTS["INSTRUMENT_LOT_KEYBOARD"]] 	= -1
CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_RIGHT"][CONSTANTS["INSTRUMENT_LOT_DRUM"]] 		= 4996

-- whether or not to hide the completed quickbuild while the player is playing the instrument
CONSTANTS["INSTRUMENT_HIDE"] = {}
CONSTANTS["INSTRUMENT_HIDE"][CONSTANTS["INSTRUMENT_LOT_GUITAR"]] 				= true
CONSTANTS["INSTRUMENT_HIDE"][CONSTANTS["INSTRUMENT_LOT_BASS"]] 					= true
CONSTANTS["INSTRUMENT_HIDE"][CONSTANTS["INSTRUMENT_LOT_KEYBOARD"]] 				= false
CONSTANTS["INSTRUMENT_HIDE"][CONSTANTS["INSTRUMENT_LOT_DRUM"]] 					= false

-- once the smash anim starts playing, how long to wait before unequipping the instrument
CONSTANTS["INSTRUMENT_UNEQUIP_TIME"] = {}
CONSTANTS["INSTRUMENT_UNEQUIP_TIME"][CONSTANTS["INSTRUMENT_LOT_GUITAR"]] 		= 1.033
CONSTANTS["INSTRUMENT_UNEQUIP_TIME"][CONSTANTS["INSTRUMENT_LOT_BASS"]] 			= 0.94
CONSTANTS["INSTRUMENT_UNEQUIP_TIME"][CONSTANTS["INSTRUMENT_LOT_KEYBOARD"]] 		= -1		-- has no equippables
CONSTANTS["INSTRUMENT_UNEQUIP_TIME"][CONSTANTS["INSTRUMENT_LOT_DRUM"]] 			= 0			-- has no smash anim, unequip immediately

-- how much imagination is repeatedly drained while the instrument is being played
CONSTANTS["INSTRUMENT_IMAGINATION_COST"] = 2

-- how often imagination is drained while playing the instrument
CONSTANTS["INSTRUMENT_COST_FREQUENCY"] = 4.0				





--------------------------------------------------------------
-- Scene 3
-- constants for the NPC that wants to see a dance emote
--------------------------------------------------------------

-- the mission number for dancing in front of this NPC
CONSTANTS["DANCE_ADMIRER_MISSION"] = 175




--------------------------------------------------------------
-- Scene 3
-- constants for dancing up near the disco ball
--------------------------------------------------------------

-- the group an NPC must be assigned to in Happy Flower in order for it to mirror the player's dancing
CONSTANTS["CONCERT_FAN_GROUP"] = "dance_crowd"

-- indices for the acceptable dances, same as above
CONSTANTS["INDEX_DWARF"] 		= 1
CONSTANTS["INDEX_FIREFIGHTER"] 	= 2
CONSTANTS["INDEX_BREAKDANCE"] 	= 3
CONSTANTS["INDEX_SKILLZ"]	 	= 4

-- the corresponding animations (from Emotes table in database)
CONSTANTS["DISCO_ANIMS"] = {}
CONSTANTS["DISCO_ANIMS"][CONSTANTS["INDEX_DWARF"]] 			= "headbang"		-- emote 207
CONSTANTS["DISCO_ANIMS"][CONSTANTS["INDEX_FIREFIGHTER"]] 	= "hat-dance"		-- emote 208
CONSTANTS["DISCO_ANIMS"][CONSTANTS["INDEX_BREAKDANCE"]] 	= "breakdance"		-- emote 192
CONSTANTS["DISCO_ANIMS"][CONSTANTS["INDEX_SKILLZ"]] 		= "breakdance"		-- emote 209





--------------------------------------------------------------
-- Scene 3
-- constants for the concert prop choicebuilds
--------------------------------------------------------------

-- the LOT's for the props
CONSTANTS["LOT_CHOICEBUILD_ROCKET"] 	= 5023
CONSTANTS["LOT_CHOICEBUILD_SPOTLIGHT"] 	= 4891
CONSTANTS["LOT_CHOICEBUILD_LASER"] 		= 5024
CONSTANTS["LOT_CHOICEBUILD_SPEAKER"] 	= 4858

-- the concert shell particle effect number for each LOT
CONSTANTS["CHOICEBUILD_SHELL_EFFECT_NUM"] = {}
CONSTANTS["CHOICEBUILD_SHELL_EFFECT_NUM"][CONSTANTS["LOT_CHOICEBUILD_ROCKET"]] 			= 537
CONSTANTS["CHOICEBUILD_SHELL_EFFECT_NUM"][CONSTANTS["LOT_CHOICEBUILD_SPOTLIGHT"]] 		= -1	-- no effect
CONSTANTS["CHOICEBUILD_SHELL_EFFECT_NUM"][CONSTANTS["LOT_CHOICEBUILD_LASER"]] 			= -1
CONSTANTS["CHOICEBUILD_SHELL_EFFECT_NUM"][CONSTANTS["LOT_CHOICEBUILD_SPEAKER"]] 		= 538

-- the concert hill particle effect number for each LOT
CONSTANTS["CHOICEBUILD_HILL_EFFECT_NUM"] = {}
CONSTANTS["CHOICEBUILD_HILL_EFFECT_NUM"][CONSTANTS["LOT_CHOICEBUILD_ROCKET"]] 			= -1
CONSTANTS["CHOICEBUILD_HILL_EFFECT_NUM"][CONSTANTS["LOT_CHOICEBUILD_SPOTLIGHT"]] 		= 539
CONSTANTS["CHOICEBUILD_HILL_EFFECT_NUM"][CONSTANTS["LOT_CHOICEBUILD_LASER"]] 			= 267
CONSTANTS["CHOICEBUILD_HILL_EFFECT_NUM"][CONSTANTS["LOT_CHOICEBUILD_SPEAKER"]] 			= -1

 -- the discoball particle effect number for each LOT
CONSTANTS["CHOICEBUILD_DISCOBALL_EFFECT_NUM"] = {}
CONSTANTS["CHOICEBUILD_DISCOBALL_EFFECT_NUM"][CONSTANTS["LOT_CHOICEBUILD_ROCKET"]] 		= -1
CONSTANTS["CHOICEBUILD_DISCOBALL_EFFECT_NUM"][CONSTANTS["LOT_CHOICEBUILD_SPOTLIGHT"]] 	= -1
CONSTANTS["CHOICEBUILD_DISCOBALL_EFFECT_NUM"][CONSTANTS["LOT_CHOICEBUILD_LASER"]] 		= 266
CONSTANTS["CHOICEBUILD_DISCOBALL_EFFECT_NUM"][CONSTANTS["LOT_CHOICEBUILD_SPEAKER"]] 	= -1

-- the name of the particle effects for each LOT
CONSTANTS["CHOICEBUILD_EFFECT_NAME"] = {}
CONSTANTS["CHOICEBUILD_EFFECT_NAME"][CONSTANTS["LOT_CHOICEBUILD_ROCKET"]] 				= "flamethrower"
CONSTANTS["CHOICEBUILD_EFFECT_NAME"][CONSTANTS["LOT_CHOICEBUILD_SPOTLIGHT"]] 			= "spotlight"
CONSTANTS["CHOICEBUILD_EFFECT_NAME"][CONSTANTS["LOT_CHOICEBUILD_LASER"]] 				= "laser"
CONSTANTS["CHOICEBUILD_EFFECT_NAME"][CONSTANTS["LOT_CHOICEBUILD_SPEAKER"]] 				= "speaker"

-- the names of the groups with the objects the particle effects get attached to
-- this is set up through Happy Flower
CONSTANTS["CHOICEBUILD_SHELL_GROUP"] 		= "effectsShell"
CONSTANTS["CHOICEBUILD_HILL_GROUP"] 		= "effectsHill"
CONSTANTS["CHOICEBUILD_DISCOBALL_GROUP"] 	= "effectsDiscoball"


--------------------------------------------------------------
-- Scene 3
-- constants for the stage platforms that move when the choicebuilds are all alike
--------------------------------------------------------------

-- the LOT's for stage and the moving platforms
CONSTANTS["LOT_STAGE"] = 3504
CONSTANTS["LOT_STAGE_PLATFORM_LARGE"] = 5027
CONSTANTS["LOT_STAGE_PLATFORM_SMALL"] = 5028

-- important path waypoints
CONSTANTS["WAYPOINT_LARGE_EXTENSION"] 	= 1		-- the large moving platform looks like an extension of the stage
CONSTANTS["WAYPOINT_LARGE_STEPS"] 		= 2		-- the large moving platform looks like a step up to the golden brick
CONSTANTS["WAYPOINT_SMALL_EXTENSION"] 	= 1		-- the small moving platform looks like an extension of the stage
CONSTANTS["WAYPOINT_SMALL_STEPS"] 		= 3		-- the small moving platform looks like a step up to the golden brick





--------------------------------------------------------------
-- Scene 4
--------------------------------------------------------------



--------------------------------------------------------------
-- Helper Functions
--------------------------------------------------------------

--------------------------------------------------------------
-- Pads a number with zeros on the left, to fill a field of the specified
-- length.
--------------------------------------------------------------
function ZeroPad(number, length)
	return string.rep("0", length - #tostring(number)) .. tostring(number)
end

--------------------------------------------------------------
-- parses time to a string
--------------------------------------------------------------
function ParseTime(numTime)

	local newTime = tonumber(numTime)
	
	local min = math.floor(newTime / 1000 / 60)
	newTime = newTime - (min * 1000 * 60)
	
	local sec = math.floor(newTime / 1000)
	newTime = newTime - (sec * 1000)
	
	local msec = math.floor(newTime)
	
	local strTime = ""
	if (min > 0) then
		strTime = ZeroPad(min,2) .. ":" .. ZeroPad(sec,2) .. "." .. ZeroPad(msec,3)
	else
		strTime = ZeroPad(sec,2) .. "." .. ZeroPad(msec,3)
	end

	return strTime

end