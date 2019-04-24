--------------------------------------------------------------
-- PROTOTYPE: Generic Survival Instance Client Zone Script: Including this 
-- file gives the custom functions for the Survival game.
--
-- updated mrb... 1/25/11 - updated celebrations
--------------------------------------------------------------

require('02_client/Minigame/Waves/L_BASE_WAVES_CLIENT')

--------------------------------------------------------------
-- Global variables
--------------------------------------------------------------

scoreboard = {	title = Localize("UI_WAVES_TITLE"),
				columnOne = Localize("UI_SG_WAVE"),
				columnTwo = Localize("TIME"),}
				
gameUI = { 	game = "BattleWaves",
			scoreboard = "BattleWavesScoreboard",
			summary = "SurvivalSummary",
			leaderboard = "SurvivalLeaderboard", }

--table of celebrations to play throughout the waves minigame, these are only needed if you want to play celebrations
celebrations = {intro = 11,	ending = 11}

--------------------------------------------------------------
