--------------------------------------------------------------

-- L_BANK_INTERACT_CLIENT.lua

-- Script that adds interactive bank UI functionality to an object
-- created abeechler ... 2/16/11
-- updated abeechler... 3/3/11		-- Added obj icon prox radius

--------------------------------------------------------------

local objAlertIconID = 107		-- Object Proximity Interact Icon ID 

----------------------------------------------
-- Adjust the interact display on set-up
----------------------------------------------
function onStartup(self,msg)
	-- Establish proximity radius for object identify icon display
	self:SetProximityRadius{iconID = objAlertIconID, radius = 80, name = "Icon_Display_Distance"}
end

----------------------------------------------
-- Check to see if the player can use the bank
----------------------------------------------
function onCheckUseRequirements(self, msg)
	-- Determine if interaction is valid based on current use
	if(self:GetVar('isInUse')) then 
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

----------------------------------------------
-- Servers tell the client script when it is to
-- open the bank UI
----------------------------------------------
function onNotifyClientObject(self,msg)
	if(msg.name == "OpenBank") then
		-- Turn OFF object interaction
		toggleActivatorIcon(self, true)
	elseif(msg.name == "CloseBank") then
		-- Turn ON object interaction
		toggleActivatorIcon(self, false, true)
	end
end

----------------------------------------------
-- Sent when the local player initiates a 
-- termination of their bank interaction
----------------------------------------------
function onTerminateInteraction(self,msg)
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
	-- Turn OFF Bank UI
    self:FireEventServerSide{args = "ToggleBank", senderID = player} 
end

----------------------------------------------
-- Toggles the activator icon based on bHide, 
-- to toggle it on you dont have to pass bHide
----------------------------------------------
function toggleActivatorIcon(self, bHide, bFromTerminate)
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    
    if not bHide then 
		-- Show the icon, cancel notification, set isInUse to false
        bHide = false
        self:SetVar('isInUse', false)
		if not bFromTerminate then
			player:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self} 
		end    
    else 
		-- Hide the icon, request notification, set isInUse to true
        self:SetVar('isInUse', true)
    end
    
    -- Request the interaction update
    self:RequestPickTypeUpdate()
end 

----------------------------------------------
-- Sent when the object checks its pick type
----------------------------------------------
function onGetPriorityPickListType(self, msg)  
	if(self:GetVar("isInUse")) then
		msg.ePickType = -1
    else
		local myPriority = 0.8
		
		if ( myPriority > msg.fCurrentPickTypePriority ) then    
			msg.fCurrentPickTypePriority = myPriority 
 
			msg.ePickType = 14    -- Interactive pick type
		end
    end  
  
    return msg      
end 