--------------------------------------------------------------------------------------
-- teleports the player above and to the center of the nimbus property map
-- and kill enemies
--
-- updated 1/28/11 mrb... checking faction correctly
-- updated 7/20/11 abeechler ... included logic to prevent teleport spam
--------------------------------------------------------------------------------------

function onCollisionPhantom(self, msg)
	local factionList = msg.objectID:GetFaction().factionList
	local bFaction = false
    
    -- Check for Player via faction
	for k,faction in ipairs(factionList) do
		if faction == 1 then 
			bFaction = true
		end
	end

	if bFaction then
        -- We have a Player
        local player = msg.objectID
        local playerID = player:GetID()
        
        if(self:GetVar("playerID")) then 
            -- We have a teleport lock for this player
            return
            
        else
            -- OK to teleport
            -- Set a timed lock
            self:SetVar("playerID", true)
            GAMEOBJ:GetTimer():AddTimerWithCancel(0.5, "playerID", self)
            
		    local object = self:GetObjectsInGroup{group = "Teleport", ignoreSpawners = true}.objects[1]

		    if object then
			    local tele = object:GetPosition().pos
			    player:Teleport {pos = {x = tele.x, y =  tele.y, z = tele.z}, bIgnoreY = false}
			    return
		    end	 
		    
        end
	end
	
	msg.objectID:RequestDie{killerID = self, killType = VIOLENT}	
end 

----------------------------------------------------------------
-- Called when timers are done
----------------------------------------------------------------
function onTimerDone(self,msg)
    -- We are receiving a playerID, unlock teleports for it
    self:SetVar(msg.name, nil)
end
