--------------------------------------------------------------
-- client side Script on the lightning balls in the lightning garden

-- created by brandi... 7/3/11
--------------------------------------------------------------


-- delay for the orb fx to start
local delay = .7

function onRenderComponentReady(self,msg)

	-- add time to the delay, taking advantage that all the orbs use the same variable, 
	-- so it offsets their fx from each other
	delay = delay + 1.4

	-- time to start the fx
	GAMEOBJ:GetTimer():AddTimerWithCancel(delay, "StartFX", self)
end



function onTimerDone(self,msg)
	
	if msg.name == "StartFX" then
		-- start fx on this orb
		self:PlayFXEffect{ name = "start" , effectType = "start" }	
	end
end

