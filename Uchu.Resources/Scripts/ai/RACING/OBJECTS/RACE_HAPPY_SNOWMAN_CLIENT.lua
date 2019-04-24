--------------------------------------------------------------
-- Winter Racetrack Zone Script: Including this 
-- file adds racingParams and Snow effects for the Winter Racetrack snowmen.
-- Created pml... 9/17/10 - add Particle attachment to Happy Snowmen.
--------------------------------------------------------------

function onStartup(self)
    self:SetVar("bIsDead", false)
end

function onCollisionPhantom(self, msg)

    if ( self:GetVar("bIsDead") == true ) then
        return
    end

    local target = msg.objectID
    
    --print("Pow!")
    
	if ( target:GetID() == GAMEOBJ:GetControlledID():GetID() ) then
   
		LEVEL:AttachCameraParticles( "racing/racing_nimbus_snow_cam_impact/racing_nimbus_snow_cam_impact", { x = 0, y = 0, z = 3} )
				
		target:PlayFXEffect{effectID = 4226, effectType = "onHit"}

		target:VehicleNotifyHitSmashable{ objHit = self }
		
		self:SetVar("bIsDead", true)

   end
end
