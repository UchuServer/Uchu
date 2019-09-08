require('o_mis')


function onNotifyClientObject(self, msg)

	if msg.name == "OpenCannon" then
		self:SetCollisionGroup{colGroup = 25}
	elseif msg.name == "CloseCannon" then
		self:SetCollisionGroup{ colGroup = 1 }
	end	
end
