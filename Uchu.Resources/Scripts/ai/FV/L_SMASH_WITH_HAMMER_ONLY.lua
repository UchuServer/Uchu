--------------------------------------------------------------
-- Server side to make an object only be smashable with a specific hammer
-- this script is intended to prototype gameplay, not as a final solution.

-- updated Brandi... 2/25/10
--------------------------------------------------------------
local hammers = { 2963,3014,3015,3016}

function onOnHit(self,msg)

	local player = msg.attacker
	
	--makes sure the player is the only client to see this 
	
	local item = player:GetEquippedItemInfo{ slot = "special_r" }.lotID
	local smash = false
	
	-- check to see if the player had one of the hammers equipped, door can only be smashed with one of the hammers
	for k,v in ipairs(hammers) do
	
		if item == v then
		
			smash = true
			break
		end
		
	end
	
	--if the player hit the door with one of the hammers
	if smash == true then
		self:RequestDie{killerID = self, killType = VIOLENT}
	else
		self:SetHealth{health = 99999}
	end
	
end