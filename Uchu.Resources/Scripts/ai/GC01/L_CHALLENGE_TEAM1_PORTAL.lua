

CONSTANTS = {}

CONSTANTS["DOOR1_LOT"] = 2892
CONSTANTS["DOOR2_LOT"] = 2893
CONSTANTS["DOOR3_LOT"] = 2894
CONSTANTS["DOOR4_LOT"] = 2895

function onCollision(self, msg)
	local player = msg.objectID
	
	local teleportTarget = {}

	local LOT = self:GetLOT().objtemplate

	if( LOT == CONSTANTS["DOOR1_LOT"] ) then
		teleportTarget = { x = -529, y = 208, z = 796 }
	elseif( LOT == CONSTANTS["DOOR2_LOT"] ) then
		teleportTarget = { x = -316, y = 198, z = 640 }
	elseif( LOT == CONSTANTS["DOOR3_LOT"] ) then
		teleportTarget = { x = -714, y = 208, z = 916 }
	elseif( LOT == CONSTANTS["DOOR4_LOT"] ) then
		teleportTarget = { x = 632, y = 208, z = 558 }
	end

	player:Teleport{ pos = teleportTarget }
end
