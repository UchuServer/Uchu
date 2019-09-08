require('o_mis')
require('L_NP_NPC')

local cooldown = 23.0

function onStartup(self)

    self:SetVar("Usable", true)
    
	--set the vars for interaction. NOTE: any/all of thses are optional
	--SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 25)

	AddInteraction(self, "proximityText", "Grrrrrrrr!")
	AddInteraction(self, "proximityText", "Rowr!")
    
end

function onClientUse(self, msg)

    if (self:GetVar("Usable") == false) then
        return
    else
        -- otherwise set us as not usable (because we are using now)
        self:SetVar("Usable", false)
        self:PlayAnimation{animationID = "interact"}
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1.0, "DoSound",self )
    end

    
    
end

--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
onTimerDone = function(self, msg)
	
	-- set our state as usable
    if (msg.name == "Cooldown") then
	    self:SetVar("Usable", true)
		
	-- keep moving
    elseif (msg.name == "DoSound") then
	    
        local player = GAMEOBJ:GetObjectByID( GAMEOBJ:GetLocalCharID() )
        player:PlaySound{strSoundName="raptor"}
        GAMEOBJ:GetTimer():AddTimerWithCancel( cooldown, "Cooldown",self )
    end	
end
