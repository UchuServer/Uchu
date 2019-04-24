-- OnEnter in HF Trigger system
function onCollisionPhantom(self, msg)
    if msg.objectID:CheckPrecondition{PreconditionID = 33}.bPass == false then return end 
    local buildObj = self  
    
    buildObj:PlayAnimation{animationID = "Light_Up", bPlayImmediate = true}
    GAMEOBJ:GetTimer():CancelAllTimers( self )
    
    --local animTime = self:GetAnimationTime{animationID = "Light_Up"}        
    GAMEOBJ:GetTimer():AddTimerWithCancel( 0.417, "Idle2", self )    
end

-- OnExit in HF Trigger system
function onOffCollisionPhantom(self, msg )
    if msg.objectID:CheckPrecondition{PreconditionID = 33}.bPass == false then return end
	
    local buildObj = self   
    
    buildObj:PlayAnimation{animationID = "Dim_Down", bPlayImmediate = true}
    
    GAMEOBJ:GetTimer():CancelAllTimers( self )
    
    --local animTime = self:GetAnimationTime{animationID = "Dim_Down"}        
    GAMEOBJ:GetTimer():AddTimerWithCancel( 0.375, "Idle1", self )    
end

-- timers...
function onTimerDone(self, msg)
    if msg.name == "Idle1" then    
        local buildObj = self   
        
        buildObj:PlayAnimation{animationID = "Dim_Loop", bPlayImmediate = true}
    elseif msg.name == "Idle2" then    
        local buildObj = self   
        
        buildObj:PlayAnimation{animationID = "Lit_Up_Loop", bPlayImmediate = true}
    end

end