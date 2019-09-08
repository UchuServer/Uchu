--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')

CONSTANTS = {}
CONSTANTS["TOOLTIP_ACTIVITY_SHOOTING_GALLERY_HELP_BIT"] = 3
CONSTANTS["HELP_SCREEN_TEXT"] = "fdsa" -- was "SG1" but ths was causing the game to start onStartup

--[[
UISendMessage switchGameState("pushGameState");

switchGameState.args["state"] = "shootinggallery";

LWOSENDMESSAGE(LWO_MAIN_INTERFACE_UI, &switchGameState);

 
This turns it off:

UISendMessage switchGameState("pushGameState");

switchGameState.args["state"] = "gameplay"; // Or previous state

LWOSENDMESSAGE(LWO_MAIN_INTERFACE_UI, &switchGameState);
--]]
--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function onStartup(self)


	self:SetVar("TotalScore", 0 )
	self:SetVar("rewardModels", {} )

	UI:SendMessage( "pushGameState", {{"state", "shootinggallery" }} )
end


function onPlayerReady(self) 
	UI:SendMessage( "pushGameState", {{"state", "shootinggallery" }} )
	
    self:SetVar("PlayerReady", true)
	-- get local player
	local player = GAMEOBJ:GetObjectByID( GAMEOBJ:GetLocalCharID() )
	
	
	player:ActivateNDAudioMusicCue{m_NDAudioMusicCueName = "GF_SG_Intro" } 
	
	
	-- check the tooltip flag	
	local tooltipMsg = player:GetTooltipFlag{iToolTip = CONSTANTS["TOOLTIP_ACTIVITY_SHOOTING_GALLERY_HELP_BIT"]}
	if (tooltipMsg.bFlag == false) then
	
		-- set the tooltip?
	else
	
		-- fake the msgbox close to start the activity
		GAMEOBJ:GetZoneControlID():MessageBoxRespond{ identifier = CONSTANTS["HELP_SCREEN_TEXT"], sender = player }
		
	end
	
	checkEverythingReady(self)
end

function checkEverythingReady(self)
    local cannonclient = getObjectByName(self, "Cannon_ClientOBJ")
    local ready = self:GetVar("PlayerReady")
    
    if (cannonclient ~= nil and ready ~= nil and ready == true) then
        self:ActivityStateChangeRequest{wsStringValue='clientready'}
        --Notify the server we're ready to enter the activity
    end						
end

function onNotifyClientObject(self,msg)	
    if msg.name == "storeCannonClient" then
        storeObjectByName(self,"Cannon_ClientOBJ", msg.paramObj )
        checkEverythingReady(self)        
    end 
end


function onNotifyClientZoneObject(self,msg) 

    if msg.name == "game_timelimit" then
		self:SetVar("timelimit", msg.param1 ) 
    elseif msg.name == "ClientZone_SetNextBest" then
		self:SetVar("NextBest", msg.paramStr)
	elseif msg.name == "beatHighScore" then
 		if self:GetVar("NextBest") ~= "nil" then
			--print("next Best ****************  ".. self:GetVar("NextBest").."   My Score: ====  "..tonumber(msg.paramStr))
			--[[
			if tonumber(msg.paramStr) > tonumber(self:GetVar("NextBest")) then
					----print("Sending msg to Cannon Client")
					local cannonclient = getObjectByName(self, "Cannon_ClientOBJ")
					--print("Name of cannonclient"..tostring(cannonclient))
					cannonclient:NotifyObject{ name = "beatHighScore" ,ObjIDSender = self}	
			end
			--]]
		end
	elseif msg.name == "UI_Score" then	-- Update the end score board
		local var =  split(msg.paramStr, ",")
 		UI:SendMessage( "ScoreUI", {{"totalScore", var[1] } , {"waveScore1", var[2] } ,{"waveScore2", var[3] }, {"waveScore3", var[4] },{"numShots", var[5] } ,{"numKills", var[6] } ,{"longestStreak", var[7] }} )
	elseif msg.name =="UI_Rewards" then
		local var =  split(msg.paramStr, ",")

		----print("Money>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> "..var[1])
		UI:SendMessage("SetRewards", {{"money", var[1]},{"item1Name", var[2]} ,{"item1Image", var[3]} ,{"item1StackSize", ""} ,{"item2Name", var[5]} ,{"item2Image", var[6]} ,{"item2StackSize", ""} } )
		UI:SendMessage( "ToggleScore", {{"toggleMenu", true }} )
		self:SetVar("TotalScore", 0 )
	elseif msg.name == "Clear" then
		UI:SendMessage("ResetUI", { {"reseting", true } })
		UI:SendMessage("StreakUI", { {"sgHideStreak",  true } ,{"sgStreak",  "0" }})
	elseif msg.name == "wave" then
  		UI:SendMessage("ChageUI", { {"sgWave",  tostring(msg.param1) } })  
	elseif msg.name =="updatescore" then
	   --print("Z Client  ".. tostring(msg.param1))
		local cannonclient = getObjectByName(self, "Cannon_ClientOBJ")
		cannonclient:NotifyObject{ name = "currentScore" , param2 = msg.param1}	
		self:SetVar("TotalScore", tostring(msg.param1))

 		UI:SendMessage("ChageUI", { {"sgScore", tostring(msg.param1) } })
	elseif msg.name == "exit" then
 		UI:SendMessage( "popGameState", {{"state", "shootinggallery" }} )
 		UI:SendMessage("ToggleScore", { {"ToggleScore", true } })
 		UI:SendMessage("HideScoreBoard", { {"sgHide", true } })
 			
	 elseif msg.name == "showloadingUI" then
		UI:SendMessage("ToggleInstanceStart", { {"visible", true } })
	 elseif msg.name == "ShowStreak" then
			UI:SendMessage("StreakUI", { {"sgStreak",  msg.paramStr } ,{"sgHideStreak",  false }})
	 elseif msg.name == "HideStreak" then
			UI:SendMessage("StreakUI", {{"sgHideStreak",  true }} )
	 elseif msg.name == "Audio_Start_Intro" then
		GAMEOBJ:GetObjectByID( GAMEOBJ:GetLocalCharID()):DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = "GF_SG_Game-Over" }
		GAMEOBJ:GetObjectByID( GAMEOBJ:GetLocalCharID()):DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = "GF_SG_Intro" } 
		GAMEOBJ:GetObjectByID( GAMEOBJ:GetLocalCharID()):ActivateNDAudioMusicCue{m_NDAudioMusicCueName = "GF_SG_Core" }
		UI:SendMessage("HideUI", { {"sgHide",  false }})
        self:FireEventServerSide{args = "CannonStored"}
	 elseif msg.name == "Audio_Final_Wave_Done" then	 
	 	UI:SendMessage("HideUI", { {"sgHide",  true }})

		GAMEOBJ:GetObjectByID( GAMEOBJ:GetLocalCharID()):DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = "GF_SG_Core" } 
		GAMEOBJ:GetObjectByID( GAMEOBJ:GetLocalCharID()):ActivateNDAudioMusicCue{m_NDAudioMusicCueName = "GF_SG_Game-Over" }
	 elseif msg.name == "Audio_Exit" then
		GAMEOBJ:GetObjectByID( GAMEOBJ:GetLocalCharID()):DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = "GF_SG_Game-Over" }
		GAMEOBJ:GetObjectByID( GAMEOBJ:GetLocalCharID()):DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = "GF_SG_Intro" } 
		GAMEOBJ:GetObjectByID( GAMEOBJ:GetLocalCharID()):DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = "GF_SG_Core" }
	 elseif msg.name == "SuperChargeBar" then
		UI:SendMessage("SuperChargUI", { {"scBar",  msg.paramStr } } )
	 elseif msg.name == "charge_counting" then
		UI:SendMessage("SuperChargUI", { {"scDeCharge",  tostring(msg.param1) } } )
	
 	elseif msg.name == "mHit" then 	-- mulit hIt
 	 	-- params.multiHit Boolean
 		UI:SendMessage("UpdateSG", { {"multiHit",  true } } )
 	elseif msg.name == "hitFriend" then 	-- Hit Friend  params.friendlyHit � Boolean 
 		UI:SendMessage("UpdateSG", { {"friendlyHit",  true } } )	
 	elseif msg.name == "targetEscaped" then 	-- target escaped play area, used for Duck SG: params.targetEscaped � Boolean; mrb...
 		UI:SendMessage("UpdateSG", { {"targetEscaped",  true } } )	
 	elseif msg.name == "cStreak" then 	-- current Streak      params.streakCoun
 		UI:SendMessage("UpdateSG", { {"streakCount",  msg.paramStr } } )	
 	elseif msg.name == "Mark1" then
		UI:SendMessage("ChageUI", { {"sgFeedBack1",  true } } )
	elseif msg.name == "Mark2" then
		UI:SendMessage("ChageUI", { {"sgFeedBack2",  true } } )
	elseif msg.name == "Mark3" then
		UI:SendMessage("ChageUI", { {"sgFeedBack3",  true } } )
	elseif msg.name == "UnMarkAll" then
		 UI:SendMessage("ChageUI", { {"sgFeedBackUnMark",  true } } )	
	elseif msg.name == "HideScoreBoard" then	
		--UI:SendMessage("HideUI", { {"sgHide", true } })
		UI:SendMessage("HideUI", { {"sgHide", true } })
		UI:SendMessage("ChageUI", { {"sgTimer", " " } })
	elseif msg.name == "modelPercent" then
		UI:SendMessage("UpdateSG", { {"modelPercent",  msg.paramStr } } )
	elseif msg.name == "rewardAdded" then
		local rewards = self:GetVar("rewardModels")
		if rewards then
			table.insert(rewards, msg.param1)
		else
			rewards = {msg.param1}
		end
		self:SetVar("rewardModels", rewards)


		
	end 
	
end


function onShutdown(self, msg)
	self:SetVar("TotalScore", 0 )

	UI:SendMessage( "popGameState", {{"state", "shootinggallery" }} )
	UI:SendMessage("ToggleScore", { {"ToggleScore", true } })
	UI:SendMessage("HideUI", { {"sgHide", true } })
	UI:SendMessage("EmbedUI", { {"sgembed",  true } } )

end

function onStartModelVisualization(self, msg)	
	self:SetVar("modelVisIndex", 1)
	self:SetVar("modelPosX", msg.x1)
	self:SetVar("bagPosX", msg.x2)
	self:SetVar("modelPosY", msg.y1)
	self:SetVar("bagPosY", msg.y2)
	self:SetVar("modelWidth", msg.width1)
	self:SetVar("bagWidth", msg.width2)

	local rewards = self:GetVar("rewardModels")
	

	if not createNextVisObject(self) then
		UI:SendMessage( "EndModelVisualization", {{"done", true}} )
	
	end
end

function onChildRenderComponentReady(self, msg)
	if self:GetVar("currentDisplayModelLOT") and msg.childLOT == self:GetVar("currentDisplayModelLOT") then 
		msg.childID:AttachFlytoScreenPos{screenDestination = {x = self:GetVar("modelPosX"), y = self:GetVar("modelPosY")}, effectType = "flytoscreen", effectID = 595, boxExtents = self:GetVar("modelWidth")}
		GAMEOBJ:GetTimer():AddTimerWithCancel(  4 , "flytimer", self )
		playChestAnimation(self)
		storeObjectByName(self, "currentDisplayModel", msg.childID)
	end
end

function onTimerDone(self, msg)
	if msg.name == "flytimer" then
		local visObj = getObjectByName(self, "currentDisplayModel")
		visObj:SetPosition{pos = visObj:GetSubNodePosition{}.pos}
		visObj:AttachFlytoScreenPos{screenDestination = {x = self:GetVar("bagPosX"), y = self:GetVar("bagPosY")}, effectType = "flytobag", effectID = 595, boxExtents = self:GetVar("bagWidth"), bUseInitialScale = true}
		GAMEOBJ:GetTimer():AddTimerWithCancel(  0.25 , "endTimer", self )
	elseif msg.name == "endTimer" then
		removeFXObject(self)
		if not createNextVisObject(self) then
			UI:SendMessage( "EndModelVisualization", {{"done", true}} )
			self:SetVar("rewardModels", nil)
			local test = self:GetVar("rewardModels")
			for i =1, #test do
				self:SetVar("rewardModels."..i.."", nil)
			end
			
		end
	end
end

function removeFXObject(self)
	if self:GetVar("currentDisplayModel") ~= "0" then
		GAMEOBJ:DeleteObject(getObjectByName(self, "currentDisplayModel"))
		self:SetVar("currentDisplayModel", "0")
	end
end

function createNextVisObject(self)
	local rewards = self:GetVar("rewardModels")
	local index = self:GetVar("modelVisIndex")
	
	if rewards and index <= #rewards then
		local grp = self:GetObjectsInGroup{ignoreSpawners=true,group = "ChestGroup" }.objects
		for i, obj in pairs(grp) do
			local pos = obj:GetPosition().pos
			self:SetVar("currentDisplayModelLOT", rewards[index])
			RESMGR:LoadObject{	objectTemplate = rewards[index],
								x = pos.x,
								y = pos.y,
								z = pos.z,
								owner = self}
			self:SetVar("modelVisIndex", self:GetVar("modelVisIndex") + 1)
		end
	else
		return false
	end
	return true
end

function playChestAnimation(self)
	local chestObjects = self:GetObjectsInGroup{group = "ChestGroup", ignoreSpawners = true}.objects
	if #chestObjects > 0 then
		for index, chest in pairs(chestObjects) do
			chest:PlayAnimation{animationID = "open"}
		end
	else
		print("ERROR: Failed to find chest object.")
	end
end
