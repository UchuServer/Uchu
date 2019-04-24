--------------------------------------------------------------
-- Server side script on the object to spawn the lion pet
-- lion has to be spawned for a server side script because the lion is a pet to be tamed
-- pet taming minigame runs on the server as well as the client

-- created by Brandi... 2/17/10
--------------------------------------------------------------

function onCheckUseRequirements(self,msg)
    local player = msg.objIDUser
    local lions = self:GetObjectsInGroup{ group = "lions", ignoreSpawners = true }.objects
	local playerlion = self:GetObjectsInGroup{ group = "lion"..player:GetID(), ignoreSpawners = true }.objects
	
	if #playerlion > 0 or #lions > 4 then 
	
		self:FireEventClientSide{args = "tooManyLions", senderID = player}
		msg.bCanUse = false
        return msg 
		
	end

end

function onUse(self,msg)

	
	local mypos = self:GetPosition().pos
	local myrot = self:GetRotation()
	local player = msg.user
	
	-- off sets the lion spawn so it doesn't spawn in the middle of the spawner object
	-- this is arbitrary and will be changed depending on where this goes in a level
	mypos.x = mypos.x + 30
	mypos.z = mypos.z-20
	--set the tamer as the player to check that only this player can tame that lion
	-- and set the group id to include the player id to make sure this player can only spawn one lion at a time
	local config = { { "tamer", player:GetID() } , { "groupID", "lion"..player:GetID()..";lions" }, {"spawnAnim", "spawn-lion"}, {"spawnTimer", 1.0}}
	
	local newLion = RESMGR:LoadObject { objectTemplate = 3520 , x = mypos.x , y = mypos.y , z = mypos.z , rw = myrot.w, rx = -1 , ry = -1 , rz = myrot.z ,owner = self, configData = config }
    
    player:PlayCinematic { pathName = "Lion_spawn" } 
end