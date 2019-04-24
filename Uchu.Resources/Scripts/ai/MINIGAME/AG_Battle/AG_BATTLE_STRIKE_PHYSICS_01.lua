
-----------------------------------------------------------
-- Physics volume script, knocks player back.  Can be turned off with other object scripts
-- Updated 3/15 Darren McKinsey
-----------------------------------------------------------


function onStartup(self)
   self:SetVar("Active", true)
end

function onCollisionPhantom(self, msg)

	print("Collision")
    -- Gets the target id that has collided
    if msg.objectID then
		if self:GetVar("Active") == true then
        local dir = self:GetObjectDirectionVectors().forward          
        -- push-back effect 1378 -- PlayFXEffect
        msg.objectID:PlayFXEffect{name = "pushBack", effectID = 1378, effectType = "create"}--effectID = 1378, effectType = "push-back"}
        msg.objectID:PlayAnimation{ animationID = "knockback-recovery" }
        dir.y = dir.y + 25
        dir.x = dir.x * 25
        dir.z = dir.z * 25
        msg.objectID:Knockback { vector = dir }
		end
	end	
end