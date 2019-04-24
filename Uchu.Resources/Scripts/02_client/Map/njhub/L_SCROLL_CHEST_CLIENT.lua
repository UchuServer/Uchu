--------------------------------------------------------------
-- Client side script for the lootable chest for the dragon fight.
--
-- updated by abeechler... 8/10/11 - remove object hiding and physics toggle, add new anims 
--------------------------------------------------------------

-- local constants
local sOpenFX = "glow"
local sIdleFX = "idiot"

local sOpenAnim = "open"
local sOpenedAnim = "opened"
local sCloseAnim = "close"

local openTime = 60

function onRenderComponentReady(self, msg)      
    -- play the idle fx    
    self:PlayFXEffect{name = "onCreate", effectType = sIdleFX}
end

function onScopeChanged(self,msg)
	-- if the player entered ghosting range
    if msg.bEnteredScope then  
	-- get the player
		local player = GAMEOBJ:GetControlledID()
		if not player:Exists() then 
			-- tell the zone control object to tell the script when the local player is loaded
			self:SendLuaNotificationRequest{requestTarget = GAMEOBJ:GetZoneControlID() , messageName="PlayerReady"}
			return
		end
		-- custom function
		CheckMissions(self,player)
	end
end

-- the zone control object says the player is loaded
function notifyPlayerReady(self,zoneObj,msg)
	-- get the player
	local player = GAMEOBJ:GetControlledID()
	
	if not player:Exists() then return end
	-- custom function to see if the players flag is set
	CheckMissions(self,player)
	-- cancel the notification request
	self:SendLuaNotificationCancel{requestTarget=player, messageName="PlayerReady"}
end

----------------------------------------------
-- decide to hide the X or not
----------------------------------------------
function CheckMissions(self,player)
	local misID = self:GetVar("ScrollMission")
	-- if the player is not on the mission, or has used this X, hid it
	if(player:GetMissionState{missionID = misID}.missionState >= 4) then 
		self:SetVar('bInUse', true)
        
	end
	self:RequestPickTypeUpdate()
end

function onCheckUseRequirements(self, msg)

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
    

    return msg
end



function onClientUse(self,msg)
    -- get the animation time for the timers
    local animTime = self:GetAnimationTime{animationID = sOpenAnim}.time or 1.5
        
    self:SetVar('bInUse', true)
    self:RequestPickTypeUpdate()
    
    -- play the open fx/animation
    self:PlayFXEffect{name = "openFX", effectType = sOpenFX}
    self:PlayAnimation{animationID = sOpenAnim, bPlayImmediate = true}
        
    -- add in timers to stop fx and remove the chest
    GAMEOBJ:GetTimer():AddTimerWithCancel(animTime, "openedChest", self)
    GAMEOBJ:GetTimer():AddTimerWithCancel(2, "StopFX", self)    
end

function onTimerDone(self, msg)
    if msg.name == "closeChest" then
        -- get the animation time for the timers
        local animTime = self:GetAnimationTime{animationID = sCloseAnim}.time or 1
        
        -- play death anim and stop the idle fx
        self:PlayAnimation{animationID = sCloseAnim, bPlayImmediate = true}
        self:StopFXEffect{name = "onCreate"}
        
        GAMEOBJ:GetTimer():AddTimerWithCancel(animTime, "chestClosed", self) 
        
	elseif msg.name == "chestClosed" then
        
		local player = GAMEOBJ:GetControlledID()
		player:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self} 
		
        self:SetVar('bInUse',false)
        CheckMissions(self,player)
    
    elseif msg.name == "openedChest" then
        -- keep the chest open for a set time
        self:PlayAnimation{animationID = sOpenedAnim, bPlayImmediate = true}
        GAMEOBJ:GetTimer():AddTimerWithCancel(openTime, "closeChest", self) 
    elseif msg.name == "StopFX" then
        -- stop the open fx
        self:StopFXEffect{name = "openFX"}        
    end
end 

----------------------------------------------
-- sent when the object checks it's pick type
----------------------------------------------
function onGetPriorityPickListType(self, msg)  
    local myPriority = 0.8
  
    if ( myPriority > msg.fCurrentPickTypePriority ) then    
        msg.fCurrentPickTypePriority = myPriority 
 
        if self:GetVar('bInUse') then -- dont show the icon if it's in use
            msg.ePickType = -1
        else
            msg.ePickType = 14    -- Interactive pick type     
        end
    end  
  
    return msg      
end 