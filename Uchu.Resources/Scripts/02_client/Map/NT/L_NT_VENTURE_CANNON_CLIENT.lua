--------------------------------------------------------------

-- PROTOTYPE: L_NT_VENTURE_CANNON_CLIENT.lua

-- Client side NT Venture Cannon interact processing
-- created abeechler ... 3/23/11
-- updated abeechler ... 4/13/11 - Changed player polling to scope sensing for anim preloads

--------------------------------------------------------------

local transportSendAnim = "scale-down"						-- Transport send animation
local transportReceiveAnim = "venture-cannon-out"			-- Transport receive animation

local cannonUseTooltip = "NT_VENTURE_CANNON_IN_USE"			-- Alert players the cannon is in use

----------------------------------------------
-- Ensure necessary anim loading for the cannon
----------------------------------------------
function preloadCannonAnims(self, player)
    -- Ensure the use player has all the necessary objects ready for use
	player:PreloadAnimation{animationID = transportSendAnim, respondObjID = self}
	player:PreloadAnimation{animationID = transportReceiveAnim, respondObjID = self}

end

----------------------------------------------
-- Catch when the local player comes within ghosting distance
----------------------------------------------
function onScopeChanged(self, msg)
    -- Has the player entered ghosting range?
    if(msg.bEnteredScope) then
        -- Obtain the local player object
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        if(not player) then
            -- Subscribe to a zone control object notification alerting the script
            -- when the local player object is ready
            self:SendLuaNotificationRequest{requestTarget = GAMEOBJ:GetZoneControlID(), messageName="PlayerReady"}
            return
        end
        
        -- Custom function to preload necessary venture cannon animations
        preloadCannonAnims(self, player)
    end
end

----------------------------------------------
-- The zone control object says when the player is loaded
----------------------------------------------
function notifyPlayerReady(self, zoneObj, msg)
    -- Get the player
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    -- Custom function to preload necessary venture cannon animations
    preloadCannonAnims(self, player)
    -- Cancel the notification request
    self:SendLuaNotificationCancel{requestTarget=player, messageName="PlayerReady"}
end

		
----------------------------------------------
-- Check to see if the player can use the cannon
----------------------------------------------
function onCheckUseRequirements(self, msg)
    
    local bIsInUse = self:GetNetworkVar("bIsInUse")
	if bIsInUse then
		-- If the interact is in use, we can break immediately and report failure
		-- Provide object specific use feedback
		if msg.isFromUI then
			msg.HasReasonFromScript = true
			msg.Script_IconID = 4206
			msg.Script_Reason = Localize(cannonUseTooltip)
			msg.Script_Failed_Requirement = true
		end
				
		msg.bCanUse = false
	else
		-- Obtain preconditions
		local preConVar = self:GetVar("CheckPrecondition")
    
		if preConVar and preConVar ~= "" then
			-- We have a valid list of preconditions to check
			local check = msg.objIDUser:CheckListOfPreconditionsFromLua{PreconditionsToCheck = preConVar, requestingID = self}
        
			if not check.bPass then 
				-- Failed the precondition check
				if msg.isFromUI then
					msg.HasReasonFromScript = true
					msg.Script_IconID = check.IconID
					msg.Script_Reason = check.FailedReason
					msg.Script_Failed_Requirement = true
				end
			
				msg.bCanUse = false
			end
		end
    end
    
    return msg
end

------------------------------------------------
---- Sent when the object checks its pick type
------------------------------------------------
function onGetPriorityPickListType(self, msg)  
    local myPriority = 0.8
  
    if (myPriority > msg.fCurrentPickTypePriority) then    
        msg.fCurrentPickTypePriority = myPriority 
		
		msg.ePickType = 14    -- Interactive pick type     

        return msg  
    end  
end
