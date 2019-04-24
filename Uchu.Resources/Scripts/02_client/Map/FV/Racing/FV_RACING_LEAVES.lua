--------------------------------------------------------------
-- Adds leaves effect while in this volume.
-- created sry... 10/12/10
--------------------------------------------------------------


function onCollisionPhantom(self, msg)
	--print("------------onCollisionPhantom")
	local playerID = GAMEOBJ:GetControlledID():GetID()
	
	if (msg.objectID:GetID() == playerID) then
        LEVEL:AttachCameraParticles( "FVracetrack/environment/rfv_leave_static/rfv_leave_static", { x = 0, y = 0, z = 3 } )
        LEVEL:AttachCameraParticles( "FVracetrack/environment/rfv_leave_trailing/rfv_leave_trailing", { x = 0, y = 0, z = 100 } )
    end    
end


function onOffCollisionPhantom(self, msg)
	--print("-----------offCollisionPhantom")
	local playerID = GAMEOBJ:GetControlledID():GetID()
	
	if (msg.objectID:GetID() == playerID) then
        LEVEL:DetachCameraParticles( "FVracetrack/environment/rfv_leave_static/rfv_leave_static" )
        LEVEL:DetachCameraParticles( "FVracetrack/environment/rfv_leave_trailing/rfv_leave_trailing")
    end
end 