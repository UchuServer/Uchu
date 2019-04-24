
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

	-- We require imagination to use this object
	local playerImagination = msg.objIDUser:GetImagination().imagination
	
	-- The player can only use this object is they have enough imagination
	local canUse = playerImagination >= imaginationRequirement;

	-- We don't need to report most of this information unless this check is coming from the UI
	if ( msg.isFromUI ) then
	
		-- This MUST be set to true, otherwise all requirements from this script will be ignored
	    msg.HasReasonFromScript = true
	    
	    -- True if player has less than four imagination
	    msg.Script_Failed_Requirement = (playerImagination < imaginationRequirement)
	    
	    if not canUse then
	    
			-- Pass the text we want to show, we only need to pass text if they fail the requirement
			msg.Script_Reason = Localize("NOT_ENOUGH_IMAGINATION")

	    end
	    
	    -- Pass an icon to show as well, we pass this whether or not they failed the requirement
	    -- This iconID references the 'IconID' column in the 'Icons' DB table.
	    msg.Script_IconID = 3118;
	    
	    -- This is the amount that will show up on the icon (as a number)
	    msg.Script_TargetAmount = imaginationRequirement;
	    
	    -- If you pass a 'TargetLOT' AND a 'TargetAmount', the icon will grab the appropriate iconID
	    -- automatically based on the LOT of the object. This way if you don't know the IconID, you can
	    -- use this instead. In the case of this script however, we're already passing an IconID, so
	    -- we'll leave this commented out.
	    --msg.Script_TargetLOT = 1234;
	    
	end
	
	if not canUse then
	
		-- This is the bool we set to actually prevent interaction. This should NEVER
		-- get set to true under any circumstance. Only set it to false, and only when
		-- the player fails a requirement check.
	    msg.bCanUse = false
	    
	    if not msg.isFromUI then
	    
			print("PLAYER FAILED TO USE THIS OBJECT");
		
		end
	
	end
	
	return msg

end

-- This function is called when the object starts up or someone requests a pick type update
-- Handling this to set pick type on an object, which makes it able to be interactive
function onGetPriorityPickListType(self, msg)  

    local myPriority = 0.8
  
    if ( myPriority > msg.fCurrentPickTypePriority ) then    
        
        msg.fCurrentPickTypePriority = myPriority 
 
        msg.ePickType = 14    -- Interactive pick type (Setting to -1 makes something non-interactive) 

    end  
  
    return msg      
end 

-- This function is called when the player tries to interact with an object, but only if the player 
-- passes the CheckUseRequirements check. 'onClientUse' is client-only, 'onUse' is server-only
function onClientUse(self, msg)

	-- If I wanted to do something on the client when the player successfully uses this object,
	-- I could do that here. In the case of this script we're removing imagination on the server
	-- so we don't need to do anything here.
	
	print("CLIENT INTERACTION SUCCESS");

end