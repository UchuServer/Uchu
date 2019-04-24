----------------------------------------------------------
-- client side script on the baby spider in a cage in AG
-- updated 6/7/11 mrb... 
-------------------------------------------------

local sAnim = "cage-interact2"
local sCinematic = "SpiderCage_Cam"
local sCameshake = "npcdeathshake"
local fxID = 1039
local flagID = 74

function onStartup(self)
	local player = GAMEOBJ:GetControlledID()
	
	if player:GetFlag{iFlagID = flagID}.bFlag then return end
	
	self:SetVar("isInUse", true)
	self:SetVisible{visible = false, fadeTime = 0.0}
end

function onGetPriorityPickListType(self, msg)  
    local myPriority = 0.8
  
    if ( myPriority > msg.fCurrentPickTypePriority ) then    
        msg.fCurrentPickTypePriority = myPriority 
 
        if self:GetVar('isInUse') then
            msg.ePickType = -1
        else
            msg.ePickType = 14    -- Interactive pick type     
        end
    end  
  
    return msg
end 

function onFireEventClientSide(self, msg)
	if msg.args == "toggle" then
		self:SetVar("isInUse", false)
		self:SetVisible{visible = true, fadeTime = 0.25}
		runEvent(self)
	end
end

function onClientUse(self, msg)
	if msg.user:GetID() ~= GAMEOBJ:GetLocalCharID() then return end
	
	runEvent(self)
end

function runEvent(self)
	if self:GetVar('bPlayingAnim') then return end
	
	local player = GAMEOBJ:GetControlledID()
	local animTime = self:GetAnimationTime{animationID = sAnim}.time - 1.35
	local cineTime = LEVEL:GetCinematicInfo(sCinematic) + 1
	
	self:SetVar('bPlayingAnim', true)        
	GAMEOBJ:GetTimer():AddTimerWithCancel(animTime, "StartFX", self)    
	GAMEOBJ:GetTimer():AddTimerWithCancel(cineTime, "StopFX", self)       
	self:PlayAnimation{animationID = sAnim, bPlayImmediate = true}
	-- TODO: refactor to use onCinematicUpdate
	player:PlayCinematic{pathName = sCinematic}
end

function onTimerDone(self, msg)
	local player = GAMEOBJ:GetControlledID()
	
    if msg.name == "StartFX" then
        player:PlayFXEffect{name = "camshake", effectType = sCameshake, effectID = fxID}
    elseif msg.name == "StopFX" then
        self:SetVar('bPlayingAnim', false)
        player:StopFXEffect{name = "camshake"}
        player:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self}
        self:RequestPickTypeUpdate()
    end        
end 