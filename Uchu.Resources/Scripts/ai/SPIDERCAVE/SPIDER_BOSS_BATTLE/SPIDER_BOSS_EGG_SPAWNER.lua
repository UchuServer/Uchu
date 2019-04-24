require('o_mis')

function onStartup(self)

	GAMEOBJ:GetTimer():AddTimerWithCancel( 10 , "notifiyActivityObj", self )

end




function onTimerDone(self,msg)



	if msg.name == "notifiyActivityObj" then
			
		local ActivityObj = self:GetObjectsInGroup{ group = "ActivityObj" ,ignoreSpawners = true }.objects
        if ActivityObj[1] then
            ActivityObj[1]:NotifyObject{ name="Egg_Spawner", ObjIDSender = self }
		end
	end


end