function onStartup(self)

    self:SetProximityRadius { radius = 10, FOVradius = 90, name = "BubbleRadius" }

	GAMEOBJ:GetTimer():AddTimerWithCancel( 30.0, "RemoveBubbleBlower",self )

end

 



function onProximityUpdate(self, msg)

    if msg.status == "ENTER" then


	self:CastSkill{optionalTargetID = msg.objId, skillID = 116}
	msg.objId:ActivateBubbleBuff{}

	
    end

end






onTimerDone = function(self, msg)

	if msg.name == "RemoveBubbleBlower" then
		
		self:Die{killType = "SILENT"}

	end

end
