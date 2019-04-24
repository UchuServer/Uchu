-- Rocket Landing Head
require('o_mis')

function onCharacterUnserialized(self, msg)

    local player=msg.charID

	local mypos = self:GetPosition().pos 
	local myrot = self:GetRotation()

    local config = { {"player", "|" .. player:GetID() }, { "custom_script_client" , "scripts/client/zone/NEW_ROCKET_LANDING_TAIL.lua" }  }
		
	RESMGR:LoadObject { objectTemplate = 6 , x = mypos.x , y = mypos.y , z = mypos.z ,owner = self, rw=myrot.w, rx=myrot.x, ry=myrot.y, rz=myrot.z, configData = config }

end
