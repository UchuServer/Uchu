require('o_mis')


function onRenderComponentReady(self, msg)
    self:PlayAnimation{animationID = "shoveldigup", bPlayImmediate = true} 
    local anim_time = self:GetAnimationTime{  animationID = "shoveldigup" }.time
    GAMEOBJ:GetTimer():AddTimerWithCancel( anim_time, "ChestDugUp", self )
end


onTimerDone = function(self, msg)

    
	if msg.name == "ChestDugUp" then
        self:PlayAnimation{animationID = "open", bPlayImmediate = true}
        local anim_time = self:GetAnimationTime{  animationID = "open" }.time
        GAMEOBJ:GetTimer():AddTimerWithCancel( anim_time , "ChestOpened", self )
    end
        
    if msg.name == "ChestOpened" then
        self:PlayAnimation{animationID = "death", bPlayImmediate = true}
    end

end
