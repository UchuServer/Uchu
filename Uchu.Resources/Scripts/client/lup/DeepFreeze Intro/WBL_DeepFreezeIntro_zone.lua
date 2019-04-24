-----------------------------------------
---   Deep Freeze Intro Zone Script   ---
-----------------------------------------

function onCreateCameraParticles(self, msg)
	--local sceneID = msg.sceneID

	LEVEL:DetachCameraParticles( "prototype/snow/snow" )
	LEVEL:DetachCameraParticles( "prototype/snow_stationary/snow_stationary" )

	LEVEL:AttachCameraParticles( "prototype/snow/snow", { x = 0, y = 20, z = 150 } )
	LEVEL:AttachCameraParticles( "prototype/snow_stationary/snow_stationary", { x = 0, y = 30, z = 60 } )

end