require('o_mis')
--///////////////////////////////////////////////////////////////////////////////////////
--//            Generic Rebuild -- Script (CLIENT)
--//   - The spawned entity that will be breaking a rebuild
--///////////////////////////////////////////////////////////////////////////////////////

local effect_interval = 0.25
    
function onRenderComponentReady(self, msg) 

		-- start a timer that will play the effects
		GAMEOBJ:GetTimer():AddTimerWithCancel( effect_interval, "DoEffect", self )
		
end


onTimerDone = function(self, msg)

    -- play the effect
    if msg.name == "DoEffect" then 

		-- play the wake effect every interval
		self:PlayFXEffect{effectType = "onmove"}

		-- start a timer that will play the effects
		GAMEOBJ:GetTimer():AddTimerWithCancel( effect_interval, "DoEffect", self )
    end    
    
end 
