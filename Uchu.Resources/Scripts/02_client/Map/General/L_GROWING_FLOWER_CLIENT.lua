--------------------------------------------------------------
-- Client Script for all grow flowers

-- edited brandi 10/25/10... changed the way the checks in onScriptNetworkVarUpdate and changed the timer on bloom to be based on the animation time
--------------------------------------------------------------
-- *** If you duped a new flower out and it is crashing the client, make sure in the render component, the animation is set to preload

function onScriptNetworkVarUpdate(self,msg)
	
	for k,v in pairs(msg.tableOfVars) do
        -- start the qb smash fx
        if k == "blooming" then
			self:BlendPrimaryAnimation{animationID = "bloom"}
			local bloomTimer = self:GetAnimationTime{animationID = "bloom"}.time
			GAMEOBJ:GetTimer():AddTimerWithCancel( bloomTimer, "Wilt", self )
		end
	end
    
end

function onTimerDone (self, msg)

    if msg.name == "Wilt" then
        self:BlendPrimaryAnimation{animationID = "idle"}
    end

end