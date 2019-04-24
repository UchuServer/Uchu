function onStartup(self)

    self:SetProximityRadius { radius = 40, name = "swapScheme" };

end

 

function onProximityUpdate(self, msg)



     if msg.status == "ENTER" then
	
                --print("start") 
		msg.objId:ShowActivityCountdown

                                    {
					bPlayCountdownSound = false,

					bPlayAdditionalSound = false,

					sndName = "no_sound",

					stateToPlaySoundOn = 0
                                    }
			
			end
		
			
			
end

