
function onStartup(self)
    self:SetVar("bIsDead", false)
end

function onCollisionPhantom(self, msg)

    if ( self:GetVar("bIsDead") == true ) then
        return
    end

    local target = msg.objectID

	if ( target:GetID() == GAMEOBJ:GetControlledID():GetID() ) then
   
		self:PlayFXEffect{effectType = "pickup"}
        --target:PlayFXEffect{name = "bouncer", effectID = 194, effectType = "onbounce"}

		target:VehicleNotifyHitSmashable{ objHit = self }
		
		self:SetVar("bIsDead", true)

   end
end
