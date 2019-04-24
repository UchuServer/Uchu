--require('ai/ACT/L_BOUNCER_BASIC')
require('o_mis')

CONSTANTS = {}
CONSTANTS["radius"] = 6
CONSTANTS["switchTemplate"] = {}
CONSTANTS["switchTemplate"][3353] = 3354
CONSTANTS["switchTemplate"][3427] = 3430
CONSTANTS["switchTemplate"][3428] = 3431

CONSTANTS["offsetMagnitude"] = 20

function onStartup(self)
	self:SetProximityRadius { radius = CONSTANTS["radius"] }
--end

--function AddedToWorld(self, msg)
	print("bouncer added to world")
	local newPos = calcPos(self:GetPosition().pos, getHeading(self), CONSTANTS["offsetMagnitude"])
	local myRot = self:GetRotation()
	RESMGR:LoadObject { objectTemplate = CONSTANTS["switchTemplate"][self:GetLOT().objtemplate], 
	                    bIsSmashable = false,
	                    x = newPos.x,
						y = newPos.y,
						z = newPos.z,
						rw = myRot.w,
						rx = myRot.x,
						ry = myRot.y,
						rz = myRot.z,
	                    owner = self }
end

function onShutdown(self)
	GAMEOBJ:DeleteObject(GetObjectByName(self, "switchObject"))
end


function onNotifyObject(self, msg)
	if msg.name == "switchPressed" then
		local objs = self:GetProximityObjects().objects
		local index = 1
	
		while index <= table.getn(objs)  do
			local target = objs[index]
			local faction = target:GetFaction()
			--verify that we are only bouncing players
			if faction and faction.faction == 1 then
				self:BouncerTriggered{triggerObj = target}
			end
			index = index + 1
		end
	end
end

function onChildLoaded(self, msg)
	if msg.templateID == CONSTANTS["switchTemplate"][self:GetLOT().objtemplate] then
		storeObjectByName(self, "switchObject", msg.childID)
		print (self:GetID())
		storeParent(self, msg.childID)
	end
end

function calcPos(pos, heading, magnitude)
	local newPos = {}
	newPos.x = pos.x + (heading.x * magnitude)
	newPos.y = pos.y + (heading.y * magnitude)
	newPos.z = pos.z + (heading.z * magnitude)
	return newPos
end
