--------------------------------------------------------------
-- L_BASE_CONSOLE_TELEPORT_CLIENT.lua

-- Client Base Script for a Teleport Console interact
-- Created abeechler... 3/22/11
-- updated mrb... 4/15/11 - added zone summary
--------------------------------------------------------------

local baseTeleportTooltip = ""
local baseObjAlertIconID = 105		-- Object Proximity Interact Icon ID 

----------------------------------------------
-- Adjust the interact display icon on set-up
----------------------------------------------
function onStartup(self,msg) 
	baseStartup(self,msg)
end

function baseStartup(self,msg)	
    -- set up the proximity radius
    self:SetProximityRadius{radius = 30, name = "TransferZoneSign"}
	self:SetProximityRadius{iconID = objAlertIconID, radius = 80, name = "Icon_Display_Distance"}
end

----------------------------------------------
-- Check to see if the player can use the console
----------------------------------------------
function onCheckUseRequirements(self, msg)	
	return baseCheckUseRequirements(self, msg)
end

function onZoneSummaryDismissed(self, msg)
	local player = GAMEOBJ:GetControlledID()
	
	-- done with the summary
	self:FireEventServerSide{args = "summaryComplete", senderID = player}
end

function baseCheckUseRequirements(self, msg)
	if self:GetVar("bInUse") then
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

function onClientUse(self, msg)
	baseClientUse(self, msg)
end

function baseClientUse(self, msg)
	if self:GetVar("bInUse") then return end
	
	-- in use
	self:SetVar("bInUse", true)
	self:RequestPickTypeUpdate()
end

----------------------------------------------
-- Code script info request for the custom npc icon display
----------------------------------------------
function onGetInteractionDetails(self, msg) 
	return baseGetInteractionDetails(self, msg)
end

function baseGetInteractionDetails(self, msg)
	-- if we have a teleportTooltip then display it on the shift icon
	local tooltipLocString = self:GetVar("teleportTooltip") or baseTeleportTooltip
	
    msg.TextDetails = Localize(tooltipLocString)
    
    return msg
end

----------------------------------------------
-- Checking for distance based termination of interaction
-- to ensure proper shutdown of open interaction windows
----------------------------------------------
function onTerminateInteraction(self, msg) 
	baseTerminateInteraction(self, msg)
end

function baseTerminateInteraction(self, msg)
    local player = msg.ObjIDTerminator
    
    -- if this is from the interaction then close the message box
	if msg.type ~= "fromInteraction" then
	    player:DisplayMessageBox{bShow = false, identifier = "TransferBox"}
	end
	
	-- not in use
	self:SetVar("bInUse", false)
	self:RequestPickTypeUpdate()
end

--------------------------------------------------------------
-- Sent when a player enter/leave a Proximity Radius
--------------------------------------------------------------
function onProximityUpdate(self, msg)
	baseProximityUpdate(self, msg)
end

function baseProximityUpdate(self, msg)
    local playerID = GAMEOBJ:GetLocalCharID()
    -- check to see if we are the correct player
    if playerID ~= msg.objId:GetID() then return end
    
    -- send the correct UI message 
    if msg.name == "TransferZoneSign" then
        if msg.status == "ENTER" then
            local mapNum = self:GetVar("transferZoneID")
            
            if not mapNum then
                mapNum = 20
            end
            
            local mapName = Localize("ZoneTable_" .. mapNum .. "_DisplayDescription")
            
            UI:SendMessage( "ToggleRocketSignage", { {"bVisible", true}, {"name", mapName}, {"zoneNumber", tostring(mapNum)} }, self )
        elseif msg.status == "LEAVE" then 
            UI:SendMessage( "ToggleRocketSignage", { {"bVisible", false} }, self )
        end
    end    
end 

function onGetPriorityPickListType(self, msg)	
    return baseGetPriorityPickListType(self, msg)
end 

function baseGetPriorityPickListType(self, msg)	
	if self:GetVar("bInUse") then
		msg.ePickType = -1
		self:UnsetProximityRadius{name = "Icon_Display_Distance"}
	else
		local myPriority = 0.8
			
		if ( myPriority > msg.fCurrentPickTypePriority ) then

		   msg.fCurrentPickTypePriority = myPriority
		   msg.ePickType = 14    -- Interactive pick type
		end
		
		-- Establish proximity radius for object identify icon display
		local alertIconLOT = self:GetVar("objAlertIconID") or baseObjAlertIconID
		
		self:SetProximityRadius{iconID = alertIconLOT, radius = 80, name = "Icon_Display_Distance"}
	end
	
    return msg
end 