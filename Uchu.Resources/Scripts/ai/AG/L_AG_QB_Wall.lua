--------------------------------------------------------------

-- L_AG_QB_Wall.lua

-- Adds special behavior to the Avant Gardens Sentinel Camp QB
-- Spawns a mob of Stromlings on successful build
-- Modified by abeechler... 1/28/11	-- Addressing ability to spawn an unlimited amount fo Stromlings
-- Modified by abeechler... 1/31/11	-- Addressing aggro on parent errors by refactoring spawn functionality

-------------------------------------------------------------

----------------------------------------------
-- Called when the rebuild is done
----------------------------------------------
function onRebuildComplete( self, msg )
	self:SetVar("player", msg.userID)
        
	-- Obtain the spawner object group
	local targetWallSpawners = self:GetVar("spawner")
	if targetWallSpawners then
		local groupObjs = self:GetObjectsInGroup{group = targetWallSpawners, ignoreSpawners = true}.objects
		-- Iterate through the group and trigger a mob spawn on every associated spawner object
		for k,obj in ipairs(groupObjs) do
			if obj:Exists() then
				-- Transmit the player triggering the spawn
				obj:SetVar("player", self:GetVar("player"))
				-- Call for the mob spawn
				obj:FireEvent{args = "spawnMobs", senderID = self}
			end
		end
	end
end
