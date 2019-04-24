--------------------------------------------------------------
-- Display's tool tip to see bob if the player hasn't completed 
-- the imagination mission
--
-- updated mrb... 5/19/11 -- cleaned up script
--------------------------------------------------------------

function onCollisionPhantom(self, msg)
	-- check if this is the local player
    if msg.objectID:GetID() ~= GAMEOBJ:GetLocalCharID() then return end
    
    --Check the Bob mission to see the status 
	local BobMissionStatus = msg.objectID:GetMissionState{missionID = 173}.missionState         
	
	--If the player has not accepted the mission, print the hint text.
	if BobMissionStatus < 2 then 
		msg.objectID:DisplayTooltip{bShow = true, 
								strText = Localize("TOOLTIP_MISSION_GIVER"), 
								strImageName = "../../textures/ui/tooltips/first_mission.dds", 
								iTime = 6000 }
	end      
end 