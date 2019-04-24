require('o_mis')
require('client/ai/NP/L_NP_NPC')

function onStartup(self)
	                       
	SetMouseOverDistance(self, 60)
	AddInteraction(self, "mouseOverAnim", "bounce_05")

end

function onNotifyObject(self, msg)
    if msg.name == "repeatloop" then
	   self:PlayFXEffect{ name = "repeat_05", effectType = "repeat_05" }
       storeObjectByName ( self, "repeat_effect", true )	
    end

    if msg.name == "stop" then
		if getObjectByName ( self, "repeat_effect") == true then
            self:StopFXEffect{ name = "repeat_05" }
			storeObjectByName ( self, "repeat_effect", false )
		end
    end
end

function onCollisionPhantom(self, msg)

	local target = msg.objectID
	local faction = target:GetFaction()
	    if faction and faction.faction == 1 then
		    local elast = self:GetVar("springpad_bouncemod")
		    local maximumSpeed = self:GetVar("springpad_maxspeed")
		    local vec = self:GetUpVector().niUpVector
		    local vel = self:GetLinearVelocity().linVelocity
		    local playerVel = target:GetLinearVelocity().linVelocity
	    	target:Deflect{direction = vec, velocity = vel,	elasticity = elast, maxSpeed = maximumSpeed, playerVelocity = playerVel}
        end
	return msg

end
