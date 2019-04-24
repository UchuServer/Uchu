--------------------------------------------------------------
-- Client side script the chest thats when the player completes the  dragon emblem daily mission
-- using the general treasure chest as a base
-- commiting out the animation and fx calls until we get animation for the chest being used
--
-- created by brandi... 7/27/11
--------------------------------------------------------------

-- flag that is set for the player
local ChestFlag = 2099

-- local constants
local sOpenFX = "glow"
local sIdleFX = "idiot"
local sInteractAnim = "open"
local sDeathAnim = "death"
local sCreateAnim = "create"

function onRenderComponentReady(self, msg)      
    -- play the spawn animation and idle fx
    self:PlayAnimation{animationID = sCreateAnim, bPlayImmediate = true}      
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
		CheckFlag(self,player)
	end
end

-- the zone control object says the player is loaded
function notifyPlayerReady(self,zoneObj,msg)
	-- get the player
	local player = GAMEOBJ:GetControlledID()
	
	if not player:Exists() then return end
	-- custom function to see if the players flag is set
	CheckFlag(self,player)
	-- cancel the notification request
	self:SendLuaNotificationCancel{requestTarget=player, messageName="PlayerReady"}
end

----------------------------------------------
-- decide to hide the X or not
----------------------------------------------
function CheckFlag(self,player)
	-- if the player is not on the mission, or has used this X, hid it
	local preConVar = self:GetVar("CheckPrecondition")
	local check = player:CheckListOfPreconditionsFromLua{PreconditionsToCheck = preConVar, requestingID = self}
	
	-- dont let the playe use this if the minigame is active or they dont meet the precondition check.
	if not check.bPass  then
		self:SetVisible{visible = false, fadeTime = 0.0}
		self:SetVar("CanUse",false)
		self:ActivatePhysics{bActivate = false}
	else
		self:SetVisible{visible = true, fadeTime = 0.0}
		self:SetVar("CanUse",true)
		self:ActivatePhysics{bActivate = true}
	end

end

----------------------------------------------
-- sent when the object checks it's pick type
----------------------------------------------
function onGetPriorityPickListType(self, msg)  
    local myPriority = 0.8
  
    if ( myPriority > msg.fCurrentPickTypePriority ) then    
        msg.fCurrentPickTypePriority = myPriority 
 
        if not self:GetVar("CanUse") then -- dont show the icon if it's in use
            msg.ePickType = -1
        else
            msg.ePickType = 14    -- Interactive pick type     
        end
    end  
  
    return msg      
end 

----------------------------------------------
-- Check to see if the player can use the chest
----------------------------------------------
function onCheckUseRequirements(self, msg)

	-- Obtain preconditions
	local preConVar = self:GetVar("CheckPrecondition")

	if preConVar and preConVar ~= "" then
		-- We have a valid list of preconditions to check
		local check = msg.objIDUser:CheckListOfPreconditionsFromLua{PreconditionsToCheck = preConVar, requestingID = self}
	
		if not check.bPass then 
			self:SetVar("CanUse",false)
			msg.bCanUse = false
		end
	end
    
	self:RequestPickTypeUpdate()
    return msg
	
end

----------------------------------------------
-- chest is notififed from sensei wu
----------------------------------------------
function onNotifyClientObject(self,msg)

	if msg.name == "showChest" then
		local player = GAMEOBJ:GetControlledID()
		if not (player:GetID() == msg.paramObj:GetID()) then return end
		
		local vis = false
		if msg.param1 == 1 then
			vis = true
		end
		
		self:SetVisible{visible = vis, fadeTime = 0.0}
		self:SetVar("CanUse",vis)
		self:ActivatePhysics{bActivate = vis}
		self:RequestPickTypeUpdate()
		
	end
		
end

----------------------------------------------
-- the player uses the chest
----------------------------------------------
function onClientUse(self,msg)
    -- get the animation time for the timers
    local animTime = self:GetAnimationTime{animationID = sInteractAnim}.time 
	
	if animTime == 0 then
		animTime = 1
	end
        
    self:SetVar("CanUse",false)
    self:RequestPickTypeUpdate()
    
    -- play the open fx/animation
    --self:PlayFXEffect{name = "openFX", effectType = sOpenFX}
    --self:PlayAnimation{animationID = sInteractAnim, bPlayImmediate = true}
        
    -- add in timers to stop fx and remove the chest
    GAMEOBJ:GetTimer():AddTimerWithCancel(animTime, "killChest", self)
    --GAMEOBJ:GetTimer():AddTimerWithCancel(2, "StopFX", self)    
end

----------------------------------------------
-- Catch timer events
----------------------------------------------
function onTimerDone(self, msg)
    if msg.name == "killChest" then
    
        -- get the animation time for the timers
        local animTime = self:GetAnimationTime{animationID = sDeathAnim}.time
        
		if animTime == 0 then
			animTime = 1
		end
	
        -- add timer to hide the chest
        GAMEOBJ:GetTimer():AddTimerWithCancel(animTime - 0.2, "HideChest", self)
        
        -- play death anim and stop the idle fx
        --self:PlayAnimation{animationID = sDeathAnim, bPlayImmediate = true}
        --self:StopFXEffect{name = "onCreate"}
		--self:ActivatePhysics{bActivate = false}
		
    elseif msg.name == "StopFX" then
    
        -- stop the open fx
        --self:StopFXEffect{name = "openFX"} 
       
    elseif msg.name == "HideChest" then
    
        -- hide the chest
        self:SetVisible{visible = false, fadeTime = 0.2}
        self:ActivatePhysics{bActivate = false}
        
		local player = GAMEOBJ:GetControlledID()
		player:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self} 
		
    end
end 