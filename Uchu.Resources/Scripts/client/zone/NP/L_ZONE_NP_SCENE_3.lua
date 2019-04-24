--------------------------------------------------------------
-- Nimbus Park - Scene 3 Specific Client Zone Script Functions
--------------------------------------------------------------


--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function Scene3Startup(self)

    -- var to record if scene 3 has been completed, so we can setup animations and effects
    self:SetVar("Scene3Complete", false)
    
    -- load Mission State information
    -- Mission 4 (dance for me)
    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_3_MISSION_4_ID"], MISSION_READY_TO_COMPLETE, function(self)
         GAMEOBJ:GetZoneControlID():NotifyObject{ name="scene_3_mission_4_ready_to_complete" } 
    end)    

    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_3_MISSION_4_ID"], MISSION_COMPLETE, function(self)
         GAMEOBJ:GetZoneControlID():NotifyObject{ name="scene_3_mission_4_ready_to_complete" } 
    end)    
    
end


--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
function Scene3OnTimerDone(self, msg)

    if (msg.name == "BBlockDance") then

        DoSceneAction(3,"anim","start")

    end    

end


--------------------------------------------------------------
-- Called when Player Ready from loading into zone
--------------------------------------------------------------
function Scene3OnPlayerReady(self, msg)

	-- if local character is ready
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	if (GAMEOBJ:GetLocalCharID() ~= CONSTANTS["NO_OBJECT"]) then
		
		-- check for mission state help, this will setup the scene
		local m4 = player:GetMissionState{ missionID = CONSTANTS["SCENE_3_MISSION_4_ID"] }
		ActivateHelp(self, MISSION_DATA, CONSTANTS["SCENE_3_MISSION_4_ID"], m4.missionState)
			
	end
	
end	


--------------------------------------------------------------
-- Called when zone object gets an onFireEvent for "SceneActorReady"
-- event.
--------------------------------------------------------------
function Scene3ActorReady(self, actor)

	-- If this is a valid actor and the scene is complete
	local bScene3Complete = self:GetVar("Scene3Complete")
	if ( bScene3Complete == true and IsValidActor(3, actor:GetLOT().objtemplate) == true ) then

		-- start any concert effects
		DoObjectAction(actor,"effect","concert")
        
		-- play cheer animations on actors
		DoObjectAction(actor,"anim","cheer")
        	
		-- see what scene this object is for
		DoObjectAction(actor,"anim","start")		
		
	end

end


--------------------------------------------------------------
-- Generic notification message
--------------------------------------------------------------
function Scene3OnNotifyObject(self, msg)

    -- mic returned
    if (msg.name == "scene_3_mission_3_complete") then
    
--        DoSceneAction(3,"anim","cheer")

    elseif (msg.name == "scene_3_mission_4_accept") then

        DoSceneAction(3,"anim","wait")

    elseif (msg.name == "scene_3_mission_4_ready_to_complete") then

		-- set state of the scene
		self:SetVar("Scene3Complete", true)
		
        -- start any concert effects
        DoSceneAction(3,"effect","concert")
        
        -- play cheer animations on actors
        DoSceneAction(3,"anim","cheer")
        
        -- play pump animations on bboys
        DoSceneAction(3,"anim","pump")
        
        -- start a timer to trigger dancing/singing animations on bboys when pump is complete
        GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["SCENE_3_BBOY_DANCE_DELAY"], "BBlockDance",self )

    elseif (msg.name == "scene_3_mission_4_complete") then

--        DoSceneAction(3,"anim","breakdance")

    end

end