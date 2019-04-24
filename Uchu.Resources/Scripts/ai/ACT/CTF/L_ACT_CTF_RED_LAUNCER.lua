require('client/ai/L_BOUNCER_BASIC')

function onCollisionPhantom(self, msg)
	local team = GAMEOBJ:GetZoneControlID():GetVar(msg.objectID:GetName().name)

	if team == "B" then

		local target = msg.objectID
		local Groupname = self:GetVar("grp_name")
		local QBGroup = self:GetObjectsInGroup{ group = Groupname}.objects
		for i = 1, 3 do
			if QBGroup[i]:GetLOT().objtemplate == 4810 or QBGroup[i]:GetLOT().objtemplate == 4811 then
				if QBGroup[i]:GetNetworkVar("broken") ~= nil and QBGroup[i]:GetNetworkVar("broken") == false then
						bounceObj(self, target)

						msg.ignoreCollision = true
						return msg
				end
			end

		end
 	end
 end




        