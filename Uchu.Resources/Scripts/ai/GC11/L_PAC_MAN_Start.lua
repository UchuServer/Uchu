function onStartup(self)

    self:SetProximityRadius { radius = 40 ,name = "swapScheme" };

end

 

function onProximityUpdate(self, msg)



     if msg.status == "ENTER" then
	--print("Pac start") 
        msg.objId:SetPlayerControlScheme { iScheme = 3, bSwitchCam = true}
	self:PlayFXEffect {effectType = "intro"}
	
     	
end

end

