--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')

CONSTANTS = {}
CONSTANTS["SMASHABLE_CONE"] = 2883

--------------------------------------------------------------
-- Startup of the object
--------------------------------------------------------------
function onStartup(self) 

	-- pick a random explode factor
	local ran = math.random(2,3)
	
	-- set explode factor
	self:SetSmashableParams{fExplodeFactor = ran}
	
end


--------------------------------------------------------------
-- On Collision
--------------------------------------------------------------
function onCollision(self, msg)

    if (self:IsDead().bDead == false) then
		-- only do this for players with faction 1
		local faction = msg.objectID:GetFaction()
		
		if faction and faction.faction == 1 then
	
			-- smash self
			self:Die{ killerID = self }
			
			-- do specials
--			local templateID = self:GetLOT().objtemplate
--			if (templateID == CONSTANTS["SMASHABLE_CONE"]) then
	   			msg.objectID:ActivateRacingPowerup{PowerupType = 1}
	   			msg.objectID:PlayAnimation{ animationID = "death-activity-racing", fPriority = 1.1 }
			
--			end   			
			
	
		end
	end
	
	msg.ignoreCollision = true
	return msg
  
end



--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
onTimerDone = function(self, msg)
    
    if msg.name == "Smash" then 
		GAMEOBJ:GetTimer():CancelAllTimers( self )
		self:Die{ killerID = self }
    end  
        
end



















