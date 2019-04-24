--------------------------------------------------------------
-- Winter Racetrack Zone Script: Including this 
-- file adds racingParams and Snow effects for the Winter Racetrack.
-- Updated pml... 9/2/10 - add Devon's snow camera particle function
-- Updated pml... 10/20/10 - changed the snow particle
--------------------------------------------------------------

function onCreateCameraParticles(self, msg)
	local sceneID = msg.sceneID

	LEVEL:DetachCameraParticles( "environment/snow_level/snow_level" )

	--print("Creating camera particles for scene: "..sceneID)

	if sceneID == 0 then
		LEVEL:AttachCameraParticles( "environment/snow_level/snow_level", { x = 0, y = 0, z = 50 } )
	end
end
