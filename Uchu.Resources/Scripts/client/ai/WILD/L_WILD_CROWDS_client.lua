function onStartup(self,msg)

    self:SetProximityRadius { radius = 20 }

end

function onProximityUpdate(self, msg)

	local target = msg.objId
	local faction = target:GetFaction()

    if faction and faction.faction == 1 then
        if msg.status == "ENTER" then
            self:PlayAnimation{ animationID = "cheer" }
        elseif msg.status == "LEAVE" then
            self:PlayAnimation{ animationID = "idle" }
        end
    end

end