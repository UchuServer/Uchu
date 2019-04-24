-----------------------------------------------------------
-- exploding crate smashable for racing
-----------------------------------------------------------

function onStartup(self)
    self:SetVar("bIsDead", false)
end

function onCollisionPhantom(self, msg)
   
    if ( self:GetVar("bIsDead") == true ) then
        return
    end

    --print("Boom!")
   
    local target = msg.objectID
   
	if ( target:GetID() == GAMEOBJ:GetControlledID():GetID() ) then

		target:VehicleNotifyHitExploder{ objHit = self }	
	
		self:SetVar("bIsDead", true)
		
	end

end