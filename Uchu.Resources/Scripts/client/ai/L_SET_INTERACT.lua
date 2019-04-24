--------------------------------------------------------------
-- Client side script for basic play animation on interact
--
-- updated mrb... 11/13/10 -- made so it could only be used when not inuse
--------------------------------------------------------------
----------------------------------------------
-- sent when the local player interacts with the
-- object before ClientUse
----------------------------------------------
function onCheckUseRequirements(self, msg)
	-- if we're in use dont let the client use this object
    if self:GetVar('bIsInUse') then 
        msg.bCanUse = false
        
        return msg
    end        
end
function onClientUse(self, msg)
	-- see if this object has an animation on it by getting the time
	local animTime = self:GetAnimationTime{animationID = "interact"}.time
	
	if not animTime then return end
	
	-- if the time isn't more than 0 we dont want to do anything
	if animTime > 0 then
		-- play the interact animation
		self:PlayAnimation{ animationID = "interact" }
		-- start up the reset timer
		GAMEOBJ:GetTimer():AddTimerWithCancel( animTime, "reset", self )
		-- set inuse and update pick type
		self:SetVar("bIsInUse", true)
		self:RequestPickTypeUpdate()
	end
end

----------------------------------------------
-- sent when the object checks it's pick type
----------------------------------------------
function onGetPriorityPickListType(self, msg)  
    local myPriority = 0.8  
    if ( myPriority > msg.fCurrentPickTypePriority ) then    
        msg.fCurrentPickTypePriority = myPriority  		-- if we are inuse then dont let this object be picked
        if self:GetVar('bIsInUse') then
            msg.ePickType = -1
        else
            msg.ePickType = 14    -- Interactive pick type     
        end
    end    
    return msg
end 

function onTimerDone(self, msg)
    if msg.name == "reset" then
		-- get the local player
		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		
		-- terminate the interaction for the player		player:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self}
		-- set inuse to false and update pick type
        self:SetVar('bIsInUse', false)
        self:RequestPickTypeUpdate()
    end
end 