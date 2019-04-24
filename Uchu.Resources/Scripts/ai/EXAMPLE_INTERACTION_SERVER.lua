
-------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------
-- EXAMPLE INTERACTION SCRIPT (Client Version)
-- For server version look for EXAMPLE_INTERACTION_SERVER.lua

-- This script allows a player to interact with an object. It only allows interaction if
-- the player has more than certain amount of imagination.

-- The server version of this script will actually remove the imagination from the player

-------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------

local imaginationRequirement = 4

function onCheckUseRequirements( self, msg )

	-- We do the same checks here as we did on the client, for security purposes.

	-- We require imagination to use this object
	local playerImagination = msg.objIDUser:GetImagination().imagination
	
	-- The player can only use this object is they have enough imagination
	local canUse = playerImagination >= imaginationRequirement;
	
	if not canUse then
	
		-- This is the bool we set to actually prevent interaction. This should NEVER
		-- get set to true under any circumstance. Only set it to false, and only when
		-- the player fails a requirement check.
	    msg.bCanUse = false
	
	end
	
	return msg

end

-- This function is called when the player tries to interact with an object, but only if the player 
-- passes the CheckUseRequirements check. 'onUse' is server-only, 'onClientUse' is client-only
function onUse(self, msg)

	print("SERVER INTERACTION SUCCESS, " .. imaginationRequirement .. " imagination removed by the server");
	
	-- Drain some imagination from the player
	msg.user:ModifyImagination{ amount = -imaginationRequirement }

end