--------------------------------------------------------------
-- Description:
--
-- handles effects on the broombot and enables interaction
--------------------------------------------------------------
require('o_mis')

local effect_interval = 1.0


--------------------------------------------------------------
-- Called when object is added to world
--------------------------------------------------------------
function onStartup(self)

    self:SetPickType{ePickType = 14}

end


--------------------------------------------------------------
-- Returns true if the object is in the idle rebuild state
--------------------------------------------------------------
function IsActive(self)

    -- get the rebuild state
    local rebuildState = self:GetRebuildState()
    
    -- if the state is idle we are active
    if (rebuildState and tonumber(rebuildState.iState) == 3) then
        return true
    else
        return false
    end

end


--------------------------------------------------------------
-- Handled when rendering is ready
--------------------------------------------------------------
function onRenderComponentReady(self, msg) 

    if (IsActive(self) == true) then

    	-- start a timer that will play the effects if active
    	-- and set the animation set to the right state
        self:SetAnimationSet{strSet = ""}
		GAMEOBJ:GetTimer():AddTimerWithCancel( effect_interval, "DoEffect", self )	

	else
	    -- show this as broken if it is supposed to be
	    self:SetAnimationSet{strSet = "broken"}
        self:PlayFXEffect{effectType = "broken"}		

    end

end


--------------------------------------------------------------
-- Handle timers
--------------------------------------------------------------
function onTimerDone(self, msg)

    -- play the effect
    if (msg.name == "DoEffect" and IsActive(self) == true) then 	
	
		-- play the effect
		self:PlayFXEffect{effectType = "onmove"}
		
		-- start a timer that will play the effects
		GAMEOBJ:GetTimer():AddTimerWithCancel( effect_interval, "DoEffect", self )

    end    
    
end 


--------------------------------------------------------------
-- Handle notification of rebuild changes
--------------------------------------------------------------
function onRebuildNotifyState(self, msg)

    -- if we just hit the idle state
	if (msg.iState == 3) then
	
	    -- change animations to non-broken
	    self:SetAnimationSet{strSet = ""}

	    -- cancel all timers
	    GAMEOBJ:GetTimer():CancelAllTimers( self )

	    -- find our rebuild animation time then start effects
	    GAMEOBJ:GetTimer():AddTimerWithCancel( 1.0 , "DoEffect", self )
	    
	else
	
	    -- cancel all timers to prevent effects
	    GAMEOBJ:GetTimer():CancelAllTimers( self )
	    
	    -- set animations to broken
	    self:SetAnimationSet{strSet = "broken"}
	    
	end
	
end   