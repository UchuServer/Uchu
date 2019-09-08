require('o_mis')


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

