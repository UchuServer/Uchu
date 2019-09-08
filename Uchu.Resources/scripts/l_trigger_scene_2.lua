--------------------------------------------------------------
-- (CLIENT SIDE) Trigger for Scene 2 client-side event
--
-- Responsible for starting scene 2 events based on mission
-- state of the player or tooltip help
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_NimbusPark')


--------------------------------------------------------------
-- Object specific constants
--------------------------------------------------------------


--------------------------------------------------------------
-- Called when object is loaded into the level
--------------------------------------------------------------
function onStartup(self) 

end


--------------------------------------------------------------
-- On Collision
--------------------------------------------------------------
function onCollision(self, msg)

	-- if the local character is passing through the trigger
	if (msg.objectID:GetID() == GAMEOBJ:GetLocalCharID()) then

		-- test flag for event
		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		local tooltipMsg = player:GetTooltipFlag{ iToolTip = CONSTANTS["SCENE_2_EVENT_FLAG_BIT"] }
		
		-- if the player has NOT seen the event
		if ((tooltipMsg) and (tooltipMsg.bFlag == false)) then

			-- set the flag
			player:SetTooltipFlag{ iToolTip = CONSTANTS["SCENE_2_EVENT_FLAG_BIT"], bFlag = true }

			-- start the scene event			
			GAMEOBJ:GetZoneControlID():NotifyObject{ name="scene_2_start" } 

		end
	
	end

	-- ignore collisions
	msg.ignoreCollision = true
	return msg
  
end




