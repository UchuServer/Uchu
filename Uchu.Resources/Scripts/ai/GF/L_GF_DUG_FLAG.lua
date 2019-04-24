


function onNotifyObject(self,msg)
	if msg.name == "flagDugUp" then
		self:NotifyClientObject{ name = "changePhysics" , paramObj = GAMEOBJ:GetObjectByID(msg.param1) , rerouteID = msg.paramObj }
		--print(" ##################### server object notified")
	end

end