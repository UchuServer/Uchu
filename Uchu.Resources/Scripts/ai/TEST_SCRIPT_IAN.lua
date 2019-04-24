
function onProximityUpdate(self, msg)
    if msg.status == "ENTER" and msg.objId:GetPossessedObject().possessedObject:GetID() ~= self:GetID() then
		print "TIME TO FLY!"
		msg.objId:PossessObject{ objToPossess = self }
    end      
end

function onStartup(self)
	self:SetProximityRadius{ radius = 10 }
end
