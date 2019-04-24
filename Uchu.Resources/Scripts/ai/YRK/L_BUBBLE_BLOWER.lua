function onStartup(self)

    self:SetProximityRadius { radius = 10, FOVradius = 90, name = "BubbleRadius" }
	self:SetVar("rebuildDone", false)
	
end

 



function onProximityUpdate(self, msg)

    if msg.status == "ENTER" and self:GetVar("rebuildDone") == true then

	self:CastSkill{optionalTargetID = msg.objId, skillID = 116}
	msg.objId:ActivateBubbleBuff{}
	
    end

end

function onRebuildNotifyState(self, msg)

    -- if we just hit the idle state
	if (msg.iState == 3) then
	
	self:SetVar("rebuildDone", true)
	else
		self:SetVar("rebuildDone", false)

	end
	
end     