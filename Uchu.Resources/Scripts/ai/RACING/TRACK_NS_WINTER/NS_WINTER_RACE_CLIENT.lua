--------------------------------------------------------------
-- Winter Racetrack Zone Script: Including this 
-- file adds racingParams and Snow effects for the Winter Racetrack.
-- Updated pml... 9/2/10 - add Devon's snow camera particle function
--------------------------------------------------------------


function onStartup(self)

	--print("RACING: onStartup")
	
    -- configure the racing control
    local racingParams =
		{
			{ "IntroCinematic", "P1" },
			{ "ExitCinematic", "FinishLine" },
			--{ "CountdownCinematic", "Countdown" },
			{ "NDAudioMusicCueName1", "GF_Race-Track"},
			{ "NDAudioMusicCueName2", "GF_Race-Track2"},
			{ "NDAudioMusicCueName3", "GF_Race-Track3"},
			{ "NDAudioMusicCueName4", "GF_Race-Track4"},			
		}
		
	--print("ConfigureRacingControl...")
	
	self:ConfigureRacingControlClient{ parameters = racingParams }
	--print("...Done")

end  

function onCreateCameraParticles(self, msg)
	local sceneID = msg.sceneID

	LEVEL:DetachCameraParticles( "prototype/snow/snow" )

	--print("Creating camera particles for scene: "..sceneID)

	if sceneID == 0 then
		LEVEL:AttachCameraParticles( "prototype/snow/snow", { x = 0, y = 50, z = 150 } )
            LEVEL:AttachCameraParticles( "prototype/snow_stationary/snow_stationary", { x = 0, y = 30, z = 60 } )
	end
end
