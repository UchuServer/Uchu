--L_NPC_FIREFLY_FLOWER

local aggroRadius = 4
local skillID = 33

function onProximityUpdate(self, msg)
	if msg.status == "ENTER" then
		local faction = msg.objId:GetFaction()
		if faction and faction.faction == 1 then
			self:CastSkill{skillID = skillID, optionalTargetID = msg.objId}
		end
	end
end

function onStartup(self)
	self:SetProximityRadius { radius = aggroRadius }
end
