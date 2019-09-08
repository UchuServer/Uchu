--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')

CONSTANTS = {}
CONSTANTS["GOAL_LINE_LOT"] = 2717
CONSTANTS["CHECKPOINT_LOT"] = 2718
CONSTANTS["SPEEDBOOST_LOT"] = 2839
CONSTANTS["TRACK_WALL_LOT"] = 2487

--------------------------------------------------------------
-- Startup of the object
--------------------------------------------------------------
function onStartup(self) 
	
	-- register with zone control object
	GAMEOBJ:GetZoneControlID():ObjectLoaded{objectID = self, templateID = self:GetLOT().objtemplate}
	
end


--------------------------------------------------------------
-- On Collision
--------------------------------------------------------------
function onCollision(self, msg)

	-- only do this for players with faction 1
	local faction = msg.objectID:GetFaction()
	
	if faction and faction.faction == 1 then

		-- get the LOT
		local templateID = self:GetLOT().objtemplate
		local strType = ""
		
  		-- send a message to the zone control object about this
  		-- Goals send OffCollision / Checkpoints send Collision
		if (templateID == CONSTANTS["GOAL_LINE_LOT"]) then
		
   			GAMEOBJ:GetZoneControlID():OffCollision{objectID = msg.objectID, senderID = self}
   			
		elseif (templateID == CONSTANTS["CHECKPOINT_LOT"]) then
		
   			GAMEOBJ:GetZoneControlID():Collision{objectID = msg.objectID, senderID = self}
   			
		elseif (templateID == CONSTANTS["SPEEDBOOST_LOT"]) then
	
   			msg.objectID:ActivateRacingPowerup{PowerupType = BOOST}

		elseif (templateID == CONSTANTS["TRACK_WALL_LOT"]) then

   			msg.objectID:ActivateRacingPowerup{PowerupType = 0}

		end
			
	end

	msg.ignoreCollision = true
	return msg
  
end

















