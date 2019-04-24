
require('o_mis')
require('client/ai/YRK/L_YRK_NPC_INTERACTIONS')

function onStartup(self)
	                     
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 35)
	SetProximityDistance(self, 35)
	
	AddInteraction(self, "interactionText", Localize("NPC_YRK_AMB_THIS_IS_MY_FIRST_DAY"))
	AddInteraction(self, "interactionText", Localize("NPC_YRK_AMB_DO_YOU_KNOW_HOW_TO_WORK_THIS_THING"))

	ActivateInteractions( self )
	
	-- let the mini-map know that there are missions related to this guy
	self:AddObjectToGroup{ group = "Minimap_MissionGivers" }
    
end





function onRenderComponentReady( self, msg )

	-- we don't want to see it in its starting position, so pause briefly before displaying
	self:SetObjectRender{ isRendering = false }
	
	GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "renderTimer", self )
end




--------------------------------------------------------------
-- called when timers expire
--------------------------------------------------------------
function onTimerDone(self, msg)

	if (msg.name == "renderTimer") then
		self:SetObjectRender{ isRendering = true }
	end
	
end





function onCursorOn(self, msg)
	
	-- which cursor to use depends on whether the player has a hazmat guy mission
	
	if ( UseMissionCursor( self ) == true ) then
		self:SetPickType{ePickType = 4}		-- PICK_LIST_MISSION_NPC
		
	else
		self:SetPickType{ePickType = 14}	-- PICK_LIST_INTERACTIVE
	end
end





function UseMissionCursor( self )

	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
	if ( player == nil or player == 0 ) then
		return false
	end
	

	local myMissionState1 = player:GetMissionState{ missionID = 171 }.missionState	-- inventor's mission to talk to trainee
	local myMissionState2 = player:GetMissionState{ missionID = 141 }.missionState	-- trainees' mission to repair truck
	local myMissionState3 = player:GetMissionState{ missionID = 123 }.missionState	-- trainees' mission to talk to skunk buster boss again
   
	-- if any of the 3 missions is either available or active, then use the mission cursor
  
	if ( IsAcceptableMissionState( myMissionState1 ) or
		IsAcceptableMissionState( myMissionState2 ) or
		IsAcceptableMissionState( myMissionState3 ) ) then
		
		return true
		
	end


	return false
                
end



function IsAcceptableMissionState ( state )

	-- returns whether the given mission state should have a mission cursor
	

	if ( state == 1 or state == 9 or 		-- LWO_MISSION_STATE_STATE_AVAILABLE
		state == 2  or state == 10 or		-- LWO_MISSION_STATE_STATE_ACTIVE
		state == 4 ) then					-- LWO_MISSION_STATE_READY_TO_COMPLETE

		return true	 
	end
	
	return false
	
end



