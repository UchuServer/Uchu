--------------------------------------------------------------

-- L_NT_IMAGIMETER_VISIBILITY_CLIENT.lua

-- Client side NT Imagimeter Plinth script
-- Process character flag updates, rebuild status, and static object visibility
-- created abeechler ... 4/14/11

--------------------------------------------------------------

local plinthRebuiltFlag = 1919      -- Player flag representing the rebuild state of the Imagineter Plinth

----------------------------------------------
-- Catch when the local player comes within ghosting distance
----------------------------------------------
function onScopeChanged(self,msg)
	-- Has the player entered ghosting range?
    if msg.bEnteredScope then
		-- Obtain the local player object
		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		if not player then
			-- Subscribe to a zone control object notification alerting the script
            -- when the local player object is ready
			self:SendLuaNotificationRequest{requestTarget = GAMEOBJ:GetZoneControlID() , messageName = "PlayerReady"}
			return
		end
		
		-- Custom function to check the status of the plinth player flag
		CheckFlag(self, player)
	end
end

----------------------------------------------
-- The zone control object says when the player is loaded
----------------------------------------------
function notifyPlayerReady(self, zoneObj, msg)
    -- Get the player
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    -- Custom function to check the status of the plinth player flag
    CheckFlag(self, player)
    -- Cancel the notification request
    self:SendLuaNotificationCancel{requestTarget = player, messageName = "PlayerReady"}
end

----------------------------------------------
-- Utility function used by the plinth to determine if its
-- rebuilt flag is set by a given player
----------------------------------------------
function CheckFlag(self, player)
	-- Determine if the player has built the plinth
	if player:GetFlag{iFlagID = plinthRebuiltFlag}.bFlag then 
		-- The player has already built the plinth, 
        -- spawn its static counterpart
		SpawnStaticPlinth(self)
	end
end

----------------------------------------------
-- Catch and process calls from the object server counterpart
----------------------------------------------
function onNotifyClientObject(self, msg)
	if msg.name == "PlinthBuilt" then 
		-- The player has built the quickbuild plinth
		SpawnStaticPlinth(self)
	end
end

----------------------------------------------
-- Object spawn function for the Imagimeter Plinth
-- called when the local player has already completed
-- the object quickbuild
----------------------------------------------
function SpawnStaticPlinth(self)
	-- Get the position and rotation of the quickbuild plinth
	local mypos = self:GetPosition().pos
	local myrot = self:GetRotation()
	
	-- Spawn in the static plinth at the quickbuild plinth's position
	RESMGR:LoadObject {objectTemplate =  14180, x = mypos.x , y = mypos.y , z = mypos.z , rw = myrot.w, rx = myrot.x, ry = myrot.y , rz = myrot.z, owner = self}
end
