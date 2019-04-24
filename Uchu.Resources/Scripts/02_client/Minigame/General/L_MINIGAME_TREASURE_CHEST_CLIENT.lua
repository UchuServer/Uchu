--------------------------------------------------------------
-- Client side script for the lootable chest for the dragon fight.
--
-- created by mrb... 10/28/10 -- copied script from dragon treasure chest
--------------------------------------------------------------

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

----------------------------------------------
-- sent when the object checks it's pick type
----------------------------------------------
function onGetPriorityPickListType(self, msg)  
    local myPriority = 0.8
  
    if ( myPriority > msg.fCurrentPickTypePriority ) then    
        msg.fCurrentPickTypePriority = myPriority 
 
        if self:GetVar('isInUse') or self:GetNetworkVar("bUsed") then -- dont show the icon if it's in use
            msg.ePickType = -1
        else
            msg.ePickType = 14    -- Interactive pick type     
        end
    end  
  
    return msg      
end 

----------------------------------------------
-- toggles the activator Icon based on bHide, 
-- to toggle it on you dont have to pass bHide
----------------------------------------------
function toggleActivatorIcon(self, bHide)
    local player = GAMEOBJ:GetControlledID()
    
    if not bHide then -- show the icon, cancel notification, set isInUse to false
        self:SetVar('isInUse', false)
        player:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self}
    else -- hide the icon, request notification, set isInUse to true
        self:SetVar('isInUse', true)
    end
    
    -- request the interaction update
    self:RequestPickTypeUpdate()
end 

function onCheckUseRequirements(self, msg)
    -- only let the player use the chest one time
    if self:GetVar("isInUse") or self:GetNetworkVar("bUsed") then
        msg.bCanUse = false
    end
    
    return msg
end

function onScriptNetworkVarUpdate(self,msg) 
    for k,v in pairs(msg.tableOfVars) do
        if k == "bUsed" then
            closeChest(self)
            
            --print("local player = " .. GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()):GetName().name)
        end
    end
end

function closeChest(self)
    -- get the animation time for the timers
    local animTime = self:GetAnimationTime{animationID = sInteractAnim}.time
        
    -- turn off the interaction icon
    toggleActivatorIcon(self, true)
    
    -- play the open fx/animation
    self:PlayFXEffect{name = "openFX", effectType = sOpenFX}
    self:PlayAnimation{animationID = sInteractAnim, bPlayImmediate = true}
        
    -- add in timers to stop fx and remove the chest
    GAMEOBJ:GetTimer():AddTimerWithCancel(animTime, "killChest", self)
    GAMEOBJ:GetTimer():AddTimerWithCancel(2, "StopFX", self)    
end

function onTimerDone(self, msg)
    if msg.name == "killChest" then
        -- get the animation time for the timers
        local animTime = self:GetAnimationTime{animationID = sDeathAnim}.time
        
        -- add timer to hide the chest
        GAMEOBJ:GetTimer():AddTimerWithCancel(animTime - 0.2, "HideChest", self)
        
        -- play death anim and stop the idle fx
        self:PlayAnimation{animationID = sDeathAnim, bPlayImmediate = true}
        self:StopFXEffect{name = "onCreate"}
		self:ActivatePhysics{bActivate = false}
    elseif msg.name == "StopFX" then
        --toggleActivatorIcon(self)
        -- stop the open fx
        self:StopFXEffect{name = "openFX"}        
    elseif msg.name == "HideChest" then
        -- hide the chest
        self:SetVisible{visible = false, fadeTime = 0.2}
		local player = GAMEOBJ:GetControlledID()
		player:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self} 
    end
end 