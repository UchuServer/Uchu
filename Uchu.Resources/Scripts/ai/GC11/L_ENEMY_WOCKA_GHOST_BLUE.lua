function onStartup(self)

    self:SetProximityRadius { radius = 10, name = "swapScheme" };

end

 

function onProximityUpdate(self, msg)



     if msg.status == "ENTER" then
	
        msg.objId:SetPlayerControlScheme { iScheme = 3, bSwitchCam = true}
	self:PlayFXEffect {effectType = "intro"}
	
     
      
 
     
		msg.objId:ShowActivityCountdown

                                    {
					bPlayCountdownSound = false,

					bPlayAdditionalSound = false,

					sndName = "no_sound",

					stateToPlaySoundOn = 0
                                    }
			
			
		msg.objId:PlayFXEffect{effectType = "fireworks"}
	
			
			end
			
		
			
			
end







