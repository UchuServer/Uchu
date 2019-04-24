--------------------------------------------------------------
-- client side Script for the moving platforms in the earth transition of the monastery
-- 
-- created by brandi... 6/6/11 
--------------------------------------------------------------

function onCollisionPhantom(self, msg)
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	--Checking to see if the player is the local player and it still exists
	if (not( player:GetID() == msg.objectID:GetID()) ) or ( not player:Exists() ) then return end
	-- tell the server script that a player is on the pillar
	self:FireEventServerSide{args = "PlayerOnPillar", senderID = player} 
	
end

function onOffCollisionPhantom(self, msg)
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	--Checking to see if the player is the local player and it still exists
	if (not( player:GetID() == msg.objectID:GetID()) ) or ( not player:Exists() ) then return end
	-- tell the server script that a player has left the pillar
	self:FireEventServerSide{args = "PlayerOffPillar", senderID = player} 
end
