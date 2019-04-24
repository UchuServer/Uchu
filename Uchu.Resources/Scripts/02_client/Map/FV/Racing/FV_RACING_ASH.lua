--------------------------------------------------------------
-- Adds ash effects while in this volume.
-- created sry... 10/12/10
--------------------------------------------------------------


function onCollisionPhantom(self, msg)
	--print("onCollisionPhantom")
	local playerID = GAMEOBJ:GetControlledID():GetID()
	
	if (msg.objectID:GetID() == playerID) then
	    LEVEL:AttachCameraParticles( "FVracetrack/environment/rfv_ash_trailing/rfv_ash_trailing", { x = 0, y = 0, z = 100 } )
        LEVEL:AttachCameraParticles( "FVracetrack/environment/rfv_ash_stationary/rfv_ash_stationary", { x = 0, y = 0, z = 100 } )
    end    
end


function onOffCollisionPhantom(self, msg)
	--print("offCollisionPhantom")
	local playerID = GAMEOBJ:GetControlledID():GetID()
	
	if (msg.objectID:GetID() == playerID) then
        LEVEL:DetachCameraParticles( "FVracetrack/environment/rfv_ash_trailing/rfv_ash_trailing" )
        LEVEL:DetachCameraParticles( "FVracetrack/environment/rfv_ash_stationary/rfv_ash_stationary")
    end
end 