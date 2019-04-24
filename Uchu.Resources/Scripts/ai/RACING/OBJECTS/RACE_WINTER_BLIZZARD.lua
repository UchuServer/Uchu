--------------------------------------------------------------
-- Adds blizzard effects while in this volume.
-- created pml... 10/4/10
--------------------------------------------------------------


function onCollisionPhantom(self, msg)
	--print("onCollisionPhantom")
	local vehicle = msg.objectID
	
	if vehicle:GetID() == GAMEOBJ:GetControlledID():GetID() then
	    LEVEL:DetachCameraParticles( "prototype/snow/snow" )
	    LEVEL:DetachCameraParticles( "prototype/snow_stationary/snow_stationary")
	
	    LEVEL:AttachCameraParticles( "prototype/blizzard/blizzard", { x = 0, y = 50, z = 150 } )
        LEVEL:AttachCameraParticles( "prototype/blizzard_stationary/blizzard_stationary", { x = 0, y = 0, z = 3 } )
    end
end


function onOffCollisionPhantom(self, msg)
	--print("offCollisionPhantom")
	local vehicle = msg.objectID
	
	if vehicle:GetID() == GAMEOBJ:GetControlledID():GetID() then
	    LEVEL:DetachCameraParticles( "prototype/blizzard/blizzard" )
	    LEVEL:DetachCameraParticles( "prototype/blizzard_stationary/blizzard_stationary")
	
	    LEVEL:AttachCameraParticles( "prototype/snow/snow", { x = 0, y = 50, z = 150 } )
        LEVEL:AttachCameraParticles( "prototype/snow_stationary/snow_stationary", { x = 0, y = 30, z = 60 } )
    end
end 