--------------------------------------------------------------
-- Adds falling debris in FV Racing - Paradox facility
-- created 10/21/10
--------------------------------------------------------------


function onCollisionPhantom(self, msg)
	--print("onCollisionPhantom")
	
	LEVEL:AttachCameraParticles( "FVracetrack/environment/FVracing_fallingDebris/FVracing_fallingDebris", { x = 0, y = 0, z = 180 } )
end


function onOffCollisionPhantom(self, msg)
	--print("offCollisionPhantom")
	LEVEL:DetachCameraParticles( "FVracetrack/environment/FVracing_fallingDebris/FVracing_fallingDebris" )

end 