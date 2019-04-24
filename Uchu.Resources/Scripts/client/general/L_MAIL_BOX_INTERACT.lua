--------------------------------------------------------------
-- Mail Box Script
-- created mrb... 4/30/10 -- updated releaseVersion to 184
-- updated abeechler... 3/2/11 -- removed unnecessary checks and function calls
-- updated abeechler... 3/3/11 -- factored into client/server scripts, added obj icon prox radius
--------------------------------------------------------------

local objAlertIconID = 106		-- Object Proximity Interact Icon ID 

----------------------------------------------
-- Adjust the interact display on set-up
----------------------------------------------
function onStartup(self,msg)
	-- Establish proximity radius for object identify icon display
	self:SetProximityRadius{iconID = objAlertIconID, radius = 80, name = "Icon_Display_Distance"}
end

----------------------------------------------
-- Check to see if the player can use the mailbox
----------------------------------------------
function onCheckUseRequirements(self, msg)
	-- Determine if interaction is valid based on current use
	if(self:GetVar('isInUse')) then 
		msg.bCanUse = false 
	end
    
    return msg
end

----------------------------------------------
-- Servers tell the client script when it is to
-- open the mail UI
----------------------------------------------
function onNotifyClientObject(self,msg)
	if(msg.name == "OpenMail") then
		-- Turn OFF object interaction
		toggleActivatorIcon(self, true)
	elseif(msg.name == "CloseMail") then
		-- Turn ON object interaction
		toggleActivatorIcon(self, false, true)
	end
end

function onTerminateInteraction(self,msg)
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
    -- Turn OFF Mail UI
    self:FireEventServerSide{args = "toggleMail", senderID = player} 
end

----------------------------------------------
-- toggles the activator Icon based on bHide, 
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