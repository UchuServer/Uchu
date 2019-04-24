--------------------------------------------------------------
-- Displays a fancy animation as you walk up to the rocket pad
-- created by cassie
-- updated brandi... 6/8/10 added local variable, and different text based on launcher types, and localized text
-- updated mrb... 7/21/10 - added check all preconditions
--------------------------------------------------------------

local PropertyLauncher = 7678 --object id of the property rocket launcher
local RocketText = "ROCKET_SIGNAGE" -- localization string for normal rocket launchers
local PropertyRocketText = "ROCKET_SIGNAGE_PROPERTY" -- localization string for property rocket launchers
--------------------------------------------------------------
-- Sent when the script is started.
--------------------------------------------------------------
function onStartup(self,msg)
    -- set up the proximity radius
    self:SetProximityRadius{radius = 30, name = "RocketSign"}
    
    local instanceIcon = 74
    
    if self:GetLOT().objtemplate == PropertyLauncher then
        instanceIcon = 83
    end
    
    -- set up the two proximity radius for icon display
    self:SetProximityRadius{iconID = instanceIcon, radius = 80, name = "Icon_Display_Distance"}
end

function onGetPriorityPickListType(self, msg)
    local myPriority = 0.8
        
    -- set the pick type
    if ( myPriority > msg.fCurrentPickTypePriority ) then
        msg.fCurrentPickTypePriority = myPriority
        msg.ePickType = 14    -- Interactive pick type
    end

    return msg
end 

function split(str, pat)
    local t = {}
    
    if str and pat then
        string.gsub(str .. pat, "(.-)" .. pat, function(result) table.insert(t, result) end)
    end
    
    return t
end

----------------------------------------------
-- sent when the local player interacts with the
-- object before ClientUse, checks to see if we 
-- in a beta 1 and sends a fail message.
----------------------------------------------
function onCheckUseRequirements(self, msg)    
    if not isFromUI then        
    
        local preConVar = self:GetVar("CheckPrecondition")
        local bPass = true
        
        -- if we dont have CheckPreconditions set in HF then return out of this function
        if preConVar and preConVar ~= "" then             
            local check = msg.objIDUser:CheckListOfPreconditionsFromLua{PreconditionsToCheck = preConVar, requestingID = self}
            
            if not check.bPass then 
                msg.HasReasonFromScript = true
                msg.Script_IconID = check.IconID
                msg.Script_Reason = check.FailedReason
                msg.Script_Failed_Requirement = true
                msg.bCanUse = false
                bPass = false
            end
        end
        
        if bPass then
            msg.HasReasonFromScript = true
            msg.Script_IconID = 2872
            msg.Script_Failed_Requirement = true
			-- if its a property rocket launcher, display different text than if its a normal rocket launcher
			if self:GetLOT().objtemplate == PropertyLauncher then	
				msg.Script_Reason = Localize(PropertyRocketText)
			else
				msg.Script_Reason = Localize(RocketText)
			end
        end
    end
    
    msg.bCanUse = false
    
    return msg
end

--------------------------------------------------------------
-- Sent when a player enter/leave a Proximity Radius
--------------------------------------------------------------
function onProximityUpdate(self, msg)
    local playerID = GAMEOBJ:GetLocalCharID()
    -- check to see if we are the correct player
    if playerID ~= msg.objId:GetID() then return end
    
    -- send the correct UI message 
    if msg.name == "RocketSign" then
        if msg.status == "ENTER" then
            local mapNum = self:GetVar("transferZoneID")
            
            if not mapNum then
                mapNum = 20
            end
            
            local mapName = Localize("ZoneTable_" .. mapNum .. "_DisplayDescription")
            
            --UI:SendMessage( "TurnOn", {}, self )
            UI:SendMessage( "ToggleRocketSignage", { {"bVisible", true}, {"name", mapName}, {"zoneNumber", tostring(mapNum)} }, self )
        elseif msg.status == "LEAVE" then 
            --UI:SendMessage( "TurnOff", {}, self )    
            UI:SendMessage( "ToggleRocketSignage", { {"bVisible", false} }, self )
        end
    end    
end 
