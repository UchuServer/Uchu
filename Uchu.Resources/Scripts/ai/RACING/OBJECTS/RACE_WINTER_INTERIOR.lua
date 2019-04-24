--------------------------------------------------------------
-- Removes snow effects when in this volume.
-- created pml... 10/4/10
--------------------------------------------------------------


function onCollisionPhantom(self, msg)
	--print("onCollisionPhantom")
	local vehicle = msg.objectID
	--print(vehicle:GetID())
	--print(GAMEOBJ:GetControlledID():GetID())
	if vehicle:GetID() == GAMEOBJ:GetControlledID():GetID() then
	    LEVEL:DetachCameraParticles( "prototype/snow/snow" )
	    LEVEL:DetachCameraParticles( "prototype/snow_stationary/snow_stationary")
	end
end


function onOffCollisionPhantom(self, msg)
	--print("offCollisionPhantom")
	local vehicle = msg.objectID
	
	if vehicle:GetID() == GAMEOBJ:GetControlledID():GetID() then
	    LEVEL:AttachCameraParticles( "prototype/snow/snow", { x = 0, y = 50, z = 150 } )
        LEVEL:AttachCameraParticles( "prototype/snow_stationary/snow_stationary", { x = 0, y = 30, z = 60 } )
    end
end 