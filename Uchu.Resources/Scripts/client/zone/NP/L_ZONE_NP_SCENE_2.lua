--------------------------------------------------------------
-- Nimbus Park - Scene 2 Specific Client Zone Script Functions
--------------------------------------------------------------

function Scene2Startup(self)

    -- var to record if scene 2 has been completed, so we can setup animations and effects
    self:SetVar("Scene2Complete", false)

    -- load Mission State information
    -- Mission 1 (collection)
    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_2_MISSION_1_ID"], MISSION_COMPLETE, function(self)
         local doNothing = 0
    end)    
    
end


--------------------------------------------------------------
-- Called when a Child is loaded
--------------------------------------------------------------
function Scene2OnChildLoaded(self, msg)

--[[
	-- Do scene 2 stuff
	if (msg.templateID == CONSTANTS["SCENE_2_METEOR_LOT"]) then

		-- get the platform moving
		msg.childID:SetMovingPlatformParams{ wsPlatformPath = CONSTANTS["SCENE_2_METEOR_PATH"], iStartIndex = 0 }

	elseif (msg.templateID == CONSTANTS["SCENE_2_METEOR_SHARD_LOT"]) then

		-- get the platform moving
		msg.childID:SetMovingPlatformParams{ wsPlatformPath = CONSTANTS["SCENE_2_METEOR_SHARD_PATH"], iStartIndex = 0 }

	elseif (msg.templateID == CONSTANTS["SCENE_2_CAMERA_LOT"]) then

		-- give the camera path data, and start it moving
		msg.childID:SetVar("camPath", CONSTANTS["SCENE_2_CAMERA_PATH"])
		msg.childID:NotifyObject{ name = "startCam", param1 = 2 }

	end
--]]
end


--------------------------------------------------------------
-- Generic notification message
--------------------------------------------------------------
function Scene2OnNotifyObject(self, msg)

	-- the scene mission has been completed
    if (msg.name == "scene_2_mission_1_complete") then
    
		-- trigger the 'building' state
		DoSceneAction(2, "stopeffects")
        DoSceneAction(2, "effect", "building")
        
        -- timer to trigger next state after build
        GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["SCENE_2_MONUMENT_BUILD_TIME"], "MonumentBuildDone",self )

	-- the trigger was hit to start the scene
	elseif (msg.name == "scene_2_start") then
	
        -- pause player
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        player:SetUserCtrlCompPause{bPaused = true}

        -- start cinematic
        player:PlayCinematic{ pathName = "MeteorPath_01" }    

        DoSceneAction(2, "anim", "scene")    
        
        -- timer to delay player teleport and animation
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1.0, "PlayerTeleAndAnimate",self )
        
        -- timer to break the monument based on animations
        GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["SCENE_2_CINE_TIMING_STATUE_HIT"], "BreakMonument",self )

        -- timer to end cinematic
        GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["SCENE_2_CINE_LENGTH"], "CinematicEnd",self )
        
--[[        		
		-- Load Camera Object
		RESMGR:LoadObject { objectTemplate = CONSTANTS["SCENE_2_CAMERA_LOT"],
		                    x = 142,
		                    y = 275,
		                    z = -342,
		                    owner = self }		
		
		-- Load Meteor
		RESMGR:LoadObject { objectTemplate = CONSTANTS["SCENE_2_METEOR_LOT"],
		                    x = 142,
		                    y = 275,
		                    z = -342,
		                    owner = self }		

		-- Load Shard
		RESMGR:LoadObject { objectTemplate = CONSTANTS["SCENE_2_METEOR_SHARD_LOT"],
		                    x = 142,
		                    y = 275,
		                    z = -342,
		                    owner = self }		
		
		-- @TODO: OnCollision/play animation on monument (breaking)
--]]	
	-- the cinematic has ended
	elseif (msg.name == "scene_2_end") then

        -- unpause player
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        player:SetUserCtrlCompPause{bPaused = false}
--[[
	elseif (msg.name == "meteor_path_complete") then

		-- if the object is the shard
		if (msg.param1 == CONSTANTS["SCENE_2_METEOR_SHARD_LOT"]) then

			DoSceneAction(2, "stopeffects")
			DoSceneAction(2, "effect", "breaking")		
			
			-- timer to trigger next state after build
			GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["SCENE_2_MONUMENT_BREAK_TIME"], "MonumentBreakDone",self )
			
		end	
--]]	
    end

end


--------------------------------------------------------------
-- Called when Player Ready from loading into zone
--------------------------------------------------------------
function Scene2OnPlayerReady(self, msg)

	-- if local character is ready
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	if (GAMEOBJ:GetLocalCharID() ~= CONSTANTS["NO_OBJECT"]) then
		
		-- check for mission state help
		local m1 = player:GetMissionState{ missionID = CONSTANTS["SCENE_2_MISSION_1_ID"] }
		local bMissionComplete = ActivateHelp(self, MISSION_DATA, CONSTANTS["SCENE_2_MISSION_1_ID"], m1.missionState)

		-- get tooltip flag for this event
		local tooltipMsg = player:GetTooltipFlag{ iToolTip = CONSTANTS["SCENE_2_EVENT_FLAG_BIT"] }
		
		-- if the player has NOT seen the event OR the player has completed the mission
		if ((tooltipMsg) and (tooltipMsg.bFlag == false)) or (bMissionComplete == true) then

			DoSceneAction(2, "effect", "working")
		
		else

			DoSceneAction(2, "effect", "broken")

		end
		
	end
	
end	


--------------------------------------------------------------
-- Called when zone object gets an onFireEvent for "SceneActorReady"
-- event.
--------------------------------------------------------------
function Scene2ActorReady(self, actor)

	-- If this is a valid actor and the scene is complete
	local bScene2Complete = self:GetVar("Scene2Complete")
	if ( IsValidActor(2, actor:GetLOT().objtemplate) == true ) then
	
		-- set the state of the scene
		if (bScene2Complete == true) then

			DoSceneAction(2, "effect", "working")

		else
		
			DoSceneAction(2, "effect", "broken")

		end
		
	end

end


--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
function Scene2OnTimerDone(self, msg)

    if (msg.name == "MonumentBuildDone") then

		DoSceneAction(2, "stopeffects")
        DoSceneAction(2, "effect", "working")

    elseif (msg.name == "MonumentBreakDone") then

		DoSceneAction(2, "stopeffects")
        DoSceneAction(2, "effect", "broken")

    elseif (msg.name == "BreakMonument") then
    
        DoSceneAction(2, "stopeffects")
        DoSceneAction(2, "effect", "breaking")		
        
        -- timer to trigger next state after build
        GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["SCENE_2_MONUMENT_BREAK_TIME"], "MonumentBreakDone",self )

	-- the cinematic has ended
	elseif (msg.name == "CinematicEnd") then

        -- unpause player
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        player:SetUserCtrlCompPause{bPaused = false}
    
    elseif (msg.name == "PlayerTeleAndAnimate") then
    
        -- teleport the player to the right location in the scene
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        player:Teleport{ pos = CONSTANTS["SCENE_2_PLAYER_CINE_POS"], 
                         x = CONSTANTS["SCENE_2_PLAYER_CINE_ROT"].x,
                         y = CONSTANTS["SCENE_2_PLAYER_CINE_ROT"].y,
                         z = CONSTANTS["SCENE_2_PLAYER_CINE_ROT"].z,
                         w = CONSTANTS["SCENE_2_PLAYER_CINE_ROT"].w,
                         bSetRotation = true,
                         bIgnoreY = true }

        -- play an animation on the player
        player:PlayAnimation{ animationID = "meteor-reaction" }
    
    end    

end
