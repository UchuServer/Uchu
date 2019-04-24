--------------------------------------------------------------
-- Server side script on the object to spawn the lion pet
-- lion has to be spawned for a server side script because the lion is a pet to be tamed
-- pet taming minigame runs on the server as well as the client

-- created by Brandi... 2/11/10
--------------------------------------------------------------

function onUse(self,msg)

	
	local mypos = self:GetPosition().pos
	local player = msg.user
	
	local lions = self:GetObjectsInGroup{ group = "lions", ignoreSpawners = true }.objects
	local playerlion = self:GetObjectsInGroup{ group = "lion"..player:GetID(), ignoreSpawners = true }.objects
	
	if #playerlion > 0 then 
	
		--print("You already spawned a lion")
		self:FireEventClientSide{args = "playerLion", senderID = player}
		return 
		
	end
	if #lions > 4 then 
	
		--print("There are too many lions right now, try again in a few minutes")
		self:FireEventClientSide{args = "tooManyLions", senderID = player}
		return 
		
	end
	
	-- off sets the lion spawn so it doesn't spawn in the middle of the spawner object
	-- this is arbitrary and will be changed depending on where this goes in a level
	mypos.x = mypos.x + 20
	
	--set the tamer as the player to check that only this player can tame that lion
	-- and set the group id to include the player id to make sure this player can only spawn one lion at a time
	local config = { { "tamer", player:GetID() } , { "groupID", "lion"..player:GetID()..";lions" } }
	
	RESMGR:LoadObject { objectTemplate = 3520 , x = mypos.x , y = mypos.y , z = mypos.z ,owner = self, configData = config }
    
end