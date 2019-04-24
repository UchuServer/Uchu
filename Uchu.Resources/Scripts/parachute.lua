local TELEPORT_POS = { x = 559.55, y = 1700, z = -464.50 }
local PARACHUTE = 2867

function onStartup(self)
print "hey dummy"
end


function onCollision(self, msg)

	local player = msg.objectID;
	if(player) then
		-- if the colliding object is a player
		if(player:GetFaction().faction == 1) then
		
			-- if he doesn't already have a parachute
			if(player:GetInvItemCount{ itemID = PARACHUTE, inventoryType = 0 }.itemCount == 0 ) then
				player:AddNewItemToInventory{ iObjTemplate = PARACHUTE }
			end
			
			player:Teleport{ pos = TELEPORT_POS, bIgnoreY = false }		
		end
	end
end

