

function onUse(self, msg)
	--print "in onUse"
	if (msg.user ~= nil) then
		if msg.user:GetPossessedObject().possessedObject:GetID() ~= self:GetID() then
			--print "Vroooom!"
			msg.user:PossessObject{ objToPossess = self }
		end     
	else
		--print "user is nil"
	end 
end


function onNotifyObject(self,msg)
	if msg.name == "updateCarNumber" then
		self:NotifyClientObject{ name = "updateCarNumber", param1 = StartPos }
	end
end