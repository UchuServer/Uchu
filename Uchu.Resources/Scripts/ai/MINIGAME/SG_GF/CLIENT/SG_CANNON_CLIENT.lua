-- ================================================
-- L_ACT_CANNON.lua
-- Client Side
-- updated 9/30/10 mrb... clearing UI on shutdown
-- ================================================
require('o_mis')
-- global vars
-------------------------------
local tMedals = {   {name = Localize("Missions_102_name"), score = 50000},         -- 102; High Scoring Pirate 1
                    {name = Localize("Missions_338_name"), score = 125000},        -- 339; High Scoring Pirate 2
                    {name = Localize("Missions_339_name"), score = 250000},        -- 339; High Scoring Pirate 3
                    {name = Localize("Missions_340_name"), score = 500000},   }    -- 340; High Scoring Pirate

local audioVars = { Intro = "GF_SG_Intro",
                    GameOver = "GF_SG_Game-Over",
                    Core = "GF_SG_Core",
                    SuperCharge = "GF_SG_Super-Cannon",
                    ChestOpening = "{7e264cc1-5524-4f79-8c70-00df13756f1b}", }                    

function StartLuaNotify(self)
    local player = GAMEOBJ:GetZoneControlID()
    
    if player:Exists() then
        self:SendLuaNotificationRequest{requestTarget=player, messageName="StartModelVisualization"}
    end
end

function StopLuaNotify(self)
    local player = GAMEOBJ:GetZoneControlID()
    
    if player:Exists() then
        self:SendLuaNotificationCancel{requestTarget=player, messageName="StartModelVisualization"}
    end
end

function onStartup(self)		
	-- Load client side parameters Here
    self:SetColor{ iLEGOColorID = 0 }
    self:SetVar("OverRideScore", true)
    self:SetVar("FoundFriendGuild", false)
    myTotalScore = 0
    self:SetVar("mySCORE", 0)
    self:SetVar("Started", false)
    self:SetVar("Time", 1)	

    GAMEOBJ:GetZoneControlID():NotifyClientObject{ name ="storeCannonClient" , paramObj = self}    
    StartLuaNotify(self) 	    
end

function onScriptNetworkVarUpdate(self,msg)
    for k,v in pairs(msg.tableOfVars) do
        --print ("k: " .. tostring(k))
        --print ("v: " ..tostring(v))
        if k == "initialskill" then
            self:SetVar("CBSkill", v)
        elseif k == "currentScore" then  
            self:SetVar("mySCORE",  v)
        elseif k == "count" then
            self:SetVar("Time", v)
            self:SetVar("Started", true)
            self:SetVar("TotalTime", v)
            UI:SendMessage("UpdateSG", { {"sgTimer", tostring(v) } })
            GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "Count", self )
        elseif k == "Stop" then
            GAMEOBJ:GetTimer():CancelAllTimers( self )
            self:SetVar("Started", false)
            UI:SendMessage("UpdateSG", { {"sgTimer", " " } })
            self:SetVar("Time", GAMEOBJ:GetZoneControlID():GetVar("timelimit"))      
        elseif k == "cbskill" then
            self:SetVar("CBSkill", v)
        elseif k == "updateScore" then        
            if self:GetVar("HoldingTopScore") then
                local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
                local playerNom = player:GetName().name
                UI:SendMessage( "UpdateSG", {{"opponentScore",  tostring(v) }, 
                                            {"opponentName", playerNom } } )   
            else        
                self:SetVar("TotalScore", v)
                UI:SendMessage( "UpdateSG", {{"opponentScore", self:GetVar("NextBest") }, 
                                            {"opponentName", self:GetVar("NextBestName") },
                                            {"sgScore", v} } )   
                checkNextHighScore(self, v)
            end
        elseif k == "ShowStreak" then
            UI:SendMessage("UpdateSG", { {"sgStreak",  v } ,{"sgHideStreak",  false }})
        elseif k == "HideStreak" then
            UI:SendMessage("UpdateSG", {{"sgHideStreak",  true }} )
        elseif k == "charge_counting" then
            UI:SendMessage("UpdateSG", { {"scDeCharge",  v } } )
        elseif k == "mHit" then 	-- mulit hIt
            UI:SendMessage("UpdateSG", { {"multiHit",  true } } )
        elseif k == "cStreak" then 	-- current Streak
            UI:SendMessage("UpdateSG", { {"streakCount",  v } } )	
        elseif k == "Mark1" then
            UI:SendMessage("UpdateSG", { {"sgFeedBack1",  true } } )
        elseif k == "Mark2" then
            UI:SendMessage("UpdateSG", { {"sgFeedBack2",  true } } )
        elseif k == "Mark3" then
            UI:SendMessage("UpdateSG", { {"sgFeedBack3",  true } } )
        elseif k == "UnMarkAll" then
             UI:SendMessage("UpdateSG", { {"sgFeedBackUnMark",  true } } )	
        elseif k == "game_timelimit" then
            self:SetVar("timelimit", v ) 
        elseif k == "ClientZone_SetNextBest" then
            self:SetVar("NextBest", v)
        elseif k == "Clear" then
            UI:SendMessage("UpdateSG", { {"sgHideStreak",  true } ,{"sgStreak",  "0" }, {"reseting", true } })
            UI:SendMessage("ToggleScoreboardinfo", {{"visible", false }} )
        elseif k == "wave.waveStr" then
            UI:SendMessage("ToggleFlashingText", {{"visible", true}, {"text", v}})
        elseif k == "wave.waveNum" then
            UI:SendMessage("UpdateSG", { {"sgWave",  tostring(v) } })  
        elseif k == "showLoadingUI" then
            UI:SendMessage("ToggleInstanceStart", { {"visible", true} })
            GAMEOBJ:GetObjectByID( GAMEOBJ:GetLocalCharID()):ActivateNDAudioMusicCue{m_NDAudioMusicCueName = audioVars.Intro }
            GAMEOBJ:GetObjectByID( GAMEOBJ:GetLocalCharID()):SetEmotesEnabled{ bEnableEmotes = false }
        elseif k == "Audio_Start_Intro" then
            local player = GAMEOBJ:GetObjectByID( GAMEOBJ:GetLocalCharID())
            
            UI:SendMessage("Togglesg_scoreboard", { {"visible",  true }})
            
            -- audio 
            player:DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = audioVars.GameOver }
            player:DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = audioVars.Intro }
            player:ActivateNDAudioMusicCue{m_NDAudioMusicCueName = audioVars.Core }
        elseif k == "Audio_Final_Wave_Done" then	 
            local player = GAMEOBJ:GetObjectByID( GAMEOBJ:GetLocalCharID())
            
            UI:SendMessage("Togglesg_scoreboard", { {"visible",  false }})
            
            -- audio 
            player:DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = audioVars.Core }
            player:DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = audioVars.SuperCharge }
            player:ActivateNDAudioMusicCue{m_NDAudioMusicCueName = audioVars.GameOver }
        elseif k == "SuperChargeBar" then
            UI:SendMessage("UpdateSG", { {"scBar",  v } } )
            local player = GAMEOBJ:GetObjectByID( GAMEOBJ:GetLocalCharID())
            if v == 100 then
                player:ActivateNDAudioMusicCue{m_NDAudioMusicCueName = audioVars.SuperCharge }
            else
                player:DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = audioVars.SuperCharge }
            end                
        elseif k == "hitFriend" then 	-- Hit Friend  params.friendlyHit - Boolean 
            UI:SendMessage("UpdateSG", { {"friendlyHit",  true } } )	 	
        elseif k == "HideScoreBoard" then	
            UI:SendMessage("Togglesg_scoreboard", { {"visible", false } })
            UI:SendMessage("UpdateSG", { {"sgTimer", " " } })
        elseif k == "modelPercent" then
            UI:SendMessage("UpdateSG", { {"modelPercent",  v } } )
        elseif k == "rewardAdded" then
            local rewards = self:GetVar("rewardModels")
            
            if rewards then
                table.insert(rewards, v)
            else
                rewards = {v}
            end
            
            self:SetVar("rewardModels", rewards)
        elseif k == "UI_Rewards" then
            local var =  split(v, "_")
            
            UI:SendMessage("ToggleScoreboardinfo", {{"visible", true }, {"totalScore", var[1] }, {"numShots", var[5] }, {"numKills", var[6] }, {"longestStreak", var[7]}} )            
            UI:SendMessage("Togglesg_scoreboard", { {"visible",  false }})
            UI:SendMessage("ToggleInstanceRewards", {{"visible", true},{"bHasModelDisplay", true} })
            UI:SendMessage("ToggleLeaderboard", { {"id",  self:GetActivityID().activityID} } )
            self:SetVar("TotalScore", 0 )
            --print("totalScore: " .. var[1] .. "numShots: " .. var[5] .. "numKills: " .. var[6] .. "longestStreak: " .. var[7])
        end 	
    end
end

function addTimer(self)
 	if self:GetVar("Started") and self:GetVar("Time") then
		local cTimer = self:GetVar("Time") -1
				
		if cTimer < 0 then
			  GAMEOBJ:GetTimer():CancelAllTimers(self)
			  self:SetVar("Started", false)
			  UI:SendMessage("UpdateSG", { {"sgTimer", " " } })
	  		  self:SetVar("Time", GAMEOBJ:GetZoneControlID():GetVar("timelimit"))
	  		  
	  		  return
		end
		
		self:SetVar("Time", cTimer)
		UI:SendMessage("UpdateSG", { {"sgTimer", tostring(cTimer) } })
		GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "Count", self )
 	end
end

function onTimerDone(self,msg)
	if msg.name =="Count" then
		addTimer(self)
	elseif msg.name == "flytimer" then
		local visObj = getObjectByName(self, "currentDisplayModel")
		visObj:SetPosition{pos = visObj:GetSubNodePosition{}.pos}
		visObj:AttachFlytoScreenPos{screenDestination = {x = self:GetVar("bagPosX"), y = self:GetVar("bagPosY")}, effectType = "flytobag", effectID = 595, boxExtents = self:GetVar("bagWidth"), bUseInitialScale = true}
		visObj:SetVisible{fadeTime = 0.2, visible = false}
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

function onSendActivitySummaryLeaderboardData(self, msg)
    --print('Activity Summary Sent to client Zone')    
    if (msg) then
	    self:SetVar("HoldingTopScore", false) 

        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()) 		
        local MaxCount = msg.leaderboardData["Result[0].RowCount"]        
        local n = 1
        local tableValues = {}
        
        for i = 0, MaxCount do            
            tableValues[(n)] =  msg.leaderboardData["Result[0].Row["..i.."].name"]         
            n = n + 1

            tableValues[(n)] =  msg.leaderboardData["Result[0].Row["..i.."].Score"]	
            n = n + 1		

            if msg.leaderboardData["Result[0].Row["..i.."].Relationship"]  then    
                if msg.leaderboardData["Result[0].Row["..i.."].Relationship"] ~= 0 then 
                    self:SetVar("FoundFriendGuild", true )
                end
            end
        end
     
        self:SetVar("bShowedPlayer", false)          
        self:SetVar("NextBest", false)  	
        self:SetVar("NextBestName", false)	
        self:SetVar("LeaderTable", tableValues)         
        setNextHighScore(self, iScore)
    end
end

function setNextHighScore(self, iScore)
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()) 	
    local name_value = 0
    local score_value = 0
    local name = 0
    local score = 0    
    local tableValues = self:GetVar("LeaderTable") or {}
    local curName = self:GetVar("NextBestName")
    local playerScore = 0
     
    if not curName then
        curName = player:GetName().name
    end
    
    for testName = 1, #tableValues do            
        if tableValues[testName] == curName then
            name = testName
            score = (testName + 1)     
            
            if name ~= 1 then 
                name_value = (testName - 2)
                score_value = (testName - 1)
            elseif name_value == 3 then
                name_value = 1
                score_value = 2
            elseif name_value == 1 then
                name_value = 1
                score_value = 2
            end
        end
        
        if tableValues[testName] == player:GetName().name then
            self:SetVar("FoundPlayer", true )
            playerScore = tableValues[(testName + 1)]
        end
    end        
        
    local uiName = tableValues[name] or player:GetName().name
    local uiScore = tableValues[score] or playerScore
    local uiNextName = tableValues[name_value] or player:GetName().name
    local uiNextScore = tableValues[score_value] or playerScore
                        
    if not self:GetVar("bShowedPlayer") then     
        uiNextName = uiName
        uiNextScore = uiScore		   
    end
    
    if not iScore then 
        iScore = 0
    end
    
    local highestScore = tableValues[2] or 0
    
    if iScore >= math.floor(highestScore) or not self:GetVar("FoundPlayer") then
        local pass = false
        
        for k,v in ipairs(tMedals) do
            if iScore < v.score then
                uiNextName = v.name
                uiNextScore = v.score
                pass = true
                break
            end
        end
        
        if not pass then
            self:SetVar("HoldingTopScore", true) 
            uiNextName = player:GetName().name
            uiNextScore = iScore      
        end      
    end    
    
    UI:SendMessage("UpdateSG", {{"opponentScore",  tostring(uiNextScore) }, 
                                {"opponentName", tostring(uiNextName) } } )        
    self:SetVar("NextBest", math.floor(uiNextScore) )  	
    self:SetVar("NextBestName", uiNextName )	
    self:SetVar("bShowedPlayer", true)    
    
    --GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{ name= "ClientZone_SetNextBest", paramStr = tostring(tableValues[score_value]) }
end

function checkNextHighScore(self, iScore)
    if not iScore or not self:GetVar('NextBest') then return end
    
    if iScore >= self:GetVar('NextBest') then        
        setNextHighScore(self, iScore)
    end
end

function onShootingGalleryFire(self, msg)
	local CBskillID = self:GetVar("CBSkill")
	self:CastSkill{skillID = CBskillID, lastClickedPosit=msg.targetPos, bUsedMouse=true}--, optionalTargetID = msg.objId}
	self:PlayFXEffect{effectType = "onfire"}
	self:PlayFXEffect{effectType = "onfire2"}
	if getActivityUser(self):Exists() then
	    getActivityUser(self):PlayFXEffect{effectType = "SG-fire"}
	end
end

function getActivityUser(self)
	local targetID = self:GetActivityUser().userID
	
	if (targetID == 0 or targetID == nil) then
		return nil
	else
		return targetID
	end
end
------------------------------------------------------------------- from client zone script
function onShutdown(self, msg)
	self:SetVar("TotalScore", 0 )

	StopLuaNotify(self)
	
	local player = GAMEOBJ:GetControlledID()
	
    if player:Exists() then
        player:SetEmotesEnabled{ bEnableEmotes = true }
    end
    
	UI:SendMessage( "EnableSpeedChat", {} )
    UI:SendMessage("Togglesg_scoreboard", { {"visible", false } })
    UI:SendMessage("UpdateSG", { {"sgTimer", " " } })
    UI:SendMessage( "ToggleScoreboardinfo", { {"visible", false } } )
	UI:SendMessage( "popGameState", { {"state", "shootinggallery" } } )	
end

function notifyStartModelVisualization(self, other, msg)	
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
		GAMEOBJ:GetTimer():AddTimerWithCancel(  2 , "flytimer", self )
		playChestAnimation(self)
		storeObjectByName(self, "currentDisplayModel", msg.childID)
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
	
	if not rewards then return false end
	
	local index = self:GetVar("modelVisIndex")
	
	if index <= #rewards then
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
		    chest:PlayNDAudioEmitter{m_NDAudioEventGUID = audioVars.ChestOpening} -- play sound for chest opening
			chest:PlayAnimation{animationID = "open"}
		end
	else
		print("ERROR: Failed to find chest object.")
	end
end 