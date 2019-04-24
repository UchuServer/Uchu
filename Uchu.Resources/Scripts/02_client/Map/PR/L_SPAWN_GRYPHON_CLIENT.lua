--------------------------------------------------------------
-- client side script on the object to spawn the grypon pet

-- created by Brandi... 7/18/11 - based on L_SPAWN_PET_BASE_CLIENT
-- updated abeechler ... 8/29/11 - refactored experience for Sentinel specific NT Pet mission
--------------------------------------------------------------

require('02_client/Map/General/L_SPAWN_PET_BASE_CLIENT')

local TooManyPetsText = "PET_SUMMON_FAIL"
local TooManyIcon = 2977

local ProxRadius = 90           -- Sense radius for mission interactivity checks
local missionNum = 1391         -- The NT Sentinel griffon pet creation mission

--------------------------------------------------------------
-- On object instantiation, establish a proximity radius value
-- and define starting state values
--------------------------------------------------------------
function onStartup(self)
    self:SetProximityRadius{radius = ProxRadius, name = "checkPlayer"}
    self:SetVar("bInteractive", true)
    
    baseStartup(self,msg)
end

--------------------------------------------------------------
-- Check to see if the player can use the console
--------------------------------------------------------------
function onCheckUseRequirements(self, msg)
	-- Determine if interaction is valid based on current use
	
	-- Find out if there is already too many pets out, or if the local player already has a pet out
	if(self:GetNetworkVar("TooManyPets")) or self:GetVar("playerPetAlready") then 
		
			if msg.isFromUI then
				msg.HasReasonFromScript = true
				msg.Script_IconID = TooManyIcon
				msg.Script_Reason = Localize(TooManyPetsText)
				msg.Script_Failed_Requirement = true
			end
			msg.bCanUse = false 
		
	end
	
    return msg
    
end

--------------------------------------------------------------
-- Receive notifications of Player approaches, and request pick
-- type updates when appropriate
--------------------------------------------------------------
function onProximityUpdate(self, msg)
    if(msg.name ~= "checkPlayer") then return end
    
    if(msg.status == "ENTER") then
        local player = GAMEOBJ:GetControlledID()
        
        if not player:Exists() then 
			-- Tell the zone control object to tell the script when the local player is loaded
			self:SendLuaNotificationRequest{requestTarget = GAMEOBJ:GetZoneControlID() , messageName="PlayerReady"}
			return
			
		elseif(msg.objId:GetID() == player:GetID()) then
		    -- Custom function
		    self:RequestPickTypeUpdate()
		end
    end
end

--------------------------------------------------------------
-- The zone control object says the player is loaded
--------------------------------------------------------------
function notifyPlayerReady(self,zoneObj,msg)
	-- Get the player
	local player = GAMEOBJ:GetControlledID()
	
	if not player:Exists() then return end
	-- Custom function to see if the players flag is set
	self:RequestPickTypeUpdate()
	-- Cancel the notification request
	self:SendLuaNotificationCancel{requestTarget=player, messageName="PlayerReady"}
end

--------------------------------------------------------------
-- Receive updates to mission states and process for 
-- NT Sentinel specific missions
--------------------------------------------------------------
function notifyNotifyMission(self, player, msg)
    if (msg.missionID == missionNum) and (msg.missionState >= 4) then
        -- We are on the NT Sentinel mission, and need to release interaction
        -- for precondition checking
        self:SetVar("bInteractive", true)
        self:SendLuaNotificationCancel{requestTarget = player, messageName = "NotifyMission"}
        self:RequestPickTypeUpdate()
    end
end

--------------------------------------------------------------
-- Receive updates for local player use and process accordingly
-- when intercepting NT Sentinel specific missions
--------------------------------------------------------------
function onClientUse(self, msg)
	local player = msg.user
	
	if player:GetMissionState{missionID = missionNum}.missionState == 2 then
        self:SetVar("bInteractive", false)
        self:SendLuaNotificationRequest{requestTarget = player, messageName="NotifyMission"}
        self:RequestPickTypeUpdate()
	end
end 

----------------------------------------------
-- If you cant internet act with the bird nest, you should see any icons
----------------------------------------------
function onGetPriorityPickListType(self, msg)  
    local myPriority = 0.8
  
    if ( myPriority > msg.fCurrentPickTypePriority ) then    
        msg.fCurrentPickTypePriority = myPriority 
        
        msg.ePickType = -1
        
        local bInteractive = self:GetVar("bInteractive")
        if(bInteractive) then
            msg.ePickType = 14
        
		    local preConVar = self:GetVar("CheckPrecondition")

		    if preConVar and preConVar ~= "" then
			    local player  = GAMEOBJ:GetControlledID()
			    if not player:Exists() then return end
			    -- We have a valid list of preconditions to check
			    local check = player:CheckListOfPreconditionsFromLua{PreconditionsToCheck = preConVar, requestingID = self}
		    
			    if not check.bPass then 
				    msg.ePickType = -1
			    end
		    end 
		end
          
		return msg
    end  
end
