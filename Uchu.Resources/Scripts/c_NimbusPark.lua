--------------------------------------------------------------
-- Constants/Settings for Nimbus Park Scripts
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------


CONSTANTS = {}
--------------------------------------------------------------
-- Other
--------------------------------------------------------------
CONSTANTS["NO_OBJECT"] = "0"
CONSTANTS["LOT_NULL"] = -1
CONSTANTS["PLAYER_FELIX_1_FLAG_BIT"] = 4
CONSTANTS["PLAYER_CHAT_HELP_FLAG_BIT"] = 12
CONSTANTS["SCENE_2_EVENT_FLAG_BIT"] = 13
CONSTANTS["SCENE_3_EVENT_FLAG_BIT"] = 14
CONSTANTS["HELP_TRIGGER_IDLE_TIME"] = 3.0
CONSTANTS["NUM_SCENES"] = 7

-- [LOT / Help Text] for Help Triggers
HELP_DATA = {}
HELP_DATA[3239] = Localize("HELP_TRIGGER_TEXT_MOVEMENT")
HELP_DATA[3240] = Localize("HELP_TRIGGER_TEXT_JUMP")
HELP_DATA[3241] = Localize("HELP_TRIGGER_TEXT_DOUBLE_JUMP")
HELP_DATA[3242] = Localize("HELP_TRIGGER_TEXT_BUILD_BOUNCER")
HELP_DATA[3420] = Localize("HELP_TRIGGER_TEXT_WALL_SMASH")
CONSTANTS["HELP_TRIGGER_DATA"] = HELP_DATA
    
--------------------------------------------------------------
-- Scene 1
--------------------------------------------------------------
CONSTANTS["FELIX_1_LOT"] = 3203
CONSTANTS["FELIX_1_SPAWN_POS"] = { x = 307.49, y = 332.9, z = -547.02 }
CONSTANTS["FELIX_1_SPAWN_ROT"] = { x = 0.0, y = 1.0, z = 0.0, w = 0.0 }
CONSTANTS["FELIX_1_PROX_RADIUS"] = 80
CONSTANTS["FELIX_1_INTERACT_RADIUS"] = 15
CONSTANTS["FELIX_1_FLEE_PATHNAME"] = "felix1path"
CONSTANTS["FELIX_1_FLEE_TEXT"] = Localize("FELIX_1_FLEE_TEXT")

STATUE_LOTS = { 2469, 2465, 2949 } 
CONSTANTS["SCENE_1_STATUE_LOTS"] = STATUE_LOTS
CONSTANTS["SCENE_1_STATUE_COOLDOWN"] = 10.0
CONSTANTS["SCENE_1_STATUE_PROX_RADIUS"] = 53.0
CONSTANTS["SCENE_1_STATUE_OFFSET"] = {x = 0.0, y = 8.3, z = 0.0 }
CONSTANTS["SCENE_1_STATUE_EFFECT_ID"] = 229
CONSTANTS["SCENE_1_STATUE_EFFECT_TYPE"] = "change"

CONSTANTS["SCENE_1_WIZARD_1_LOT"] = 3727
CONSTANTS["SCENE_1_WIZARD_2_LOT"] = 3728
CONSTANTS["SCENE_1_WIZARD_BATTLE_LOTS"] = {3762, 3763, 3764, 3765, 3766, 3767, 3768, 3770}
CONSTANTS["SCENE_1_VALID_ACTORS"] = {3727,3728}
WIZARD_TEXT = {}
WIZARD_TEXT[1] = "Shazam!"
WIZARD_TEXT[2] = "I choose you..."
WIZARD_TEXT[3] = "How about this!"
WIZARD_TEXT[4] = "Oh yeah? Try this!"
WIZARD_TEXT[5] = "Hocus Pocus!"
WIZARD_TEXT[6] = "Take that!"
WIZARD_TEXT[7] = "This is better!"
WIZARD_TEXT[8] = "My turn!"
WIZARD_TEXT[9] ="Form of..."
CONSTANTS["WIZARD_CAST_TEXT"] = WIZARD_TEXT

--------------------------------------------------------------
-- Scene 2
--------------------------------------------------------------
CONSTANTS["SCENE_2_VALID_ACTORS"] = {3696, 2562, 3694}  -- statue, npc, meteorscene
CONSTANTS["FELIX_2_PROX_RADIUS"] = 20
CONSTANTS["SCENE_2_MISSION_1_ID"] = 115
CONSTANTS["SCENE_2_CHAT_HELP_TEXT"] = Localize("NP_SCENE_2_CHAT_HELP_TEXT")
--CONSTANTS["SCENE_2_METEOR_LOT"] = 3637
--CONSTANTS["SCENE_2_METEOR_SHARD_LOT"] = 3638
--CONSTANTS["SCENE_2_CAMERA_LOT"] = 3604
--CONSTANTS["SCENE_2_METEOR_SPEED"] = 5.0
--CONSTANTS["SCENE_2_METEOR_PATH"] = "meteor_path"
--CONSTANTS["SCENE_2_METEOR_SHARD_PATH"] = "meteor_shard_path"
--CONSTANTS["SCENE_2_CAMERA_PATH"] = "scene2_camera_path"
CONSTANTS["SCENE_2_MONUMENT_BUILD_TIME"] = 5.0
CONSTANTS["SCENE_2_MONUMENT_BREAK_TIME"] = 5.0
CONSTANTS["SCENE_2_PLAYER_CINE_POS"] = { x = 621.89, y = 252.72, z = 87.17 }
CONSTANTS["SCENE_2_PLAYER_CINE_ROT"] = { x = 0.0, y = 0.0, z = 0.0, w = 1.0 }
CONSTANTS["SCENE_2_CINE_TIMING_STATUE_HIT"] = 18.133
CONSTANTS["SCENE_2_CINE_LENGTH"] = 23.0




--------------------------------------------------------------
-- Scene 3
--------------------------------------------------------------
CONSTANTS["SCENE_3_VALID_ACTORS"] = {3235, 3607, 2666, 2652, 2653, 3608, 3405, 3605, 3606, 3214, 3614}
CONSTANTS["SCENE_3_MISSION_1_ID"] = 174
CONSTANTS["SCENE_3_MISSION_2_ID"] = 175
CONSTANTS["SCENE_3_MISSION_3_ID"] = 176
CONSTANTS["SCENE_3_MISSION_4_ID"] = 178

CONSTANTS["SCENE_3_MISSION_4_READY_COMPLETE_TEXT"] = Localize("NP_SCENE_3_MISSION_4_READY_COMPLETE_TEXT")

CONSTANTS["SCENE_3_BBOY_DANCE_DELAY"] = 5.0

CROWD_TEXT = {}
CROWD_TEXT[1] = Localize("NP_CROWD_BORED_TEXT_1")
CROWD_TEXT[2] = Localize("NP_CROWD_BORED_TEXT_2")
CROWD_TEXT[3] = Localize("NP_CROWD_BORED_TEXT_3")
CONSTANTS["CROWD_NO_MISSION_TEXT"] = CROWD_TEXT

CROWD_TEXT = {}
CROWD_TEXT[1] = Localize("NP_CROWD_HAPPY_TEXT_1")
CROWD_TEXT[2] = Localize("NP_CROWD_HAPPY_TEXT_2")
CROWD_TEXT[3] = Localize("NP_CROWD_HAPPY_TEXT_3")
CONSTANTS["CROWD_YES_MISSION_TEXT"] = CROWD_TEXT

CONSTANTS["CROWD_PROX_CHANCE"] = 45		-- whole number %
CONSTANTS["SCENE_3_CROWD_EFFECT_ID"] = 274
CONSTANTS["SCENE_3_CROWD_EFFECT_TYPE"] = "bored"



--------------------------------------------------------------
-- Scene 4
--------------------------------------------------------------
CONSTANTS["COURSE_STARTER_LOT"] = 3004
CONSTANTS["COUNTDOWN_TIME"] = 3.0
CONSTANTS["COURSE_STARTER_OFFER_TEXT"] = Localize("NP_COURSE_STARTER_OFFER_TEXT")
CONSTANTS["COURSE_STARTER_ACTIVE_TEXT"] = Localize("NP_COURSE_STARTER_ACTIVE_TEXT")
CONSTANTS["COURSE_OUT_OF_RANGE_TEXT"] = Localize("NP_COURSE_OUT_OF_RANGE_TEXT")
CONSTANTS["COURSE_OUT_OF_RANGE_MSG_SHOW_TIME"] = 4000
CONSTANTS["COURSE_FINISH_TEXT"] = Localize("NP_COURSE_FINISH_TEXT")
CONSTANTS["COURSE_FINISH_TEXT2"] = Localize("NP_COURSE_FINISH_TEXT2")
CONSTANTS["COURSE_FINISH_MSG_SHOW_TIME"] = 4000
CONSTANTS["CollectableTemplate"] = 3609
CONSTANTS["BigCollectableTemplate"] = 3610
CONSTANTS["CollectablePath"] = "CourseCollectables"
CONSTANTS["BigCollectablePath"] = "BigCourseCollectables"
CONSTANTS["CollectableTimeAdded"] = 100
CONSTANTS["BigCollectableTimeAdded"] = 1000

--------------------------------------------------------------
-- Scene 5
--------------------------------------------------------------
CONSTANTS["SCENE_5_MISSION_1_ID"] = 130
CONSTANTS["SCENE_5_MISSION_2_ID"] = 129

--------------------------------------------------------------
-- Scene 6
--------------------------------------------------------------
-- Server only constants, see ai/NP/L_NP_SERVER_CONSTANTS.lua

--------------------------------------------------------------
-- Scene 7
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