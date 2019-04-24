function onStartup(self)

    self:SetProximityRadius { radius = 40, name = "swapScheme" };

end

 

function onProximityUpdate(self, msg)



     if msg.status == "ENTER" then
	--print("Pac end") 
        msg.objId:SetPlayerControlScheme { iScheme = 1, bSwitchCam = true}
	
			
	end
			
		
			
			
end

