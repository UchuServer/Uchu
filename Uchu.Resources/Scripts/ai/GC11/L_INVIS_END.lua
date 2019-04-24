function onStartup(self)

    self:SetProximityRadius { radius = 40, name = "swapScheme" };

end

 onTimerDone = function(self, msg)

    if (msg.name == "Go") then
		print("end") 
		UI:DisplayToolTip{strDialogText = "You reached the end! Good Job!", strImageName = "", bShow=true, iTime=0}
	end

	end
function onProximityUpdate(self, msg)



     if msg.status == "ENTER" then
	--print("end") 
      
		msg.objId:PlayFXEffect{effectType = "fireworks"}
	end
			
					
			
end
