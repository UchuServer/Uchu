--------------------------------------------------------------
-- server script on the imagination pipes in the beam 

-- updated mrb... 10/26/10
--------------------------------------------------------------

function onScopeChanged(self,msg)
	-- if the player entered ghosting range
    if msg.bEnteredScope then  
		-- get the local player
		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		if not player:Exists() then
			-- tell the zone control object to tell the script when the local player is loaded
			self:SendLuaNotificationRequest{requestTarget = GAMEOBJ:GetZoneControlID() , messageName="PlayerReady"}
			return
		end
		-- custom function to see if the players flag is set
		CheckFlag(self,player)
	end
end

-- the zone control object says the player is loaded
function notifyPlayerReady(self,zoneObj,msg)
	-- get the player
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
	if not player:Exists() then return end
	-- custom function to see if the players flag is set
	CheckFlag(self,player)
	-- cancel the notification request
	self:SendLuaNotificationCancel{requestTarget=player, messageName="PlayerReady"}
end

function CheckFlag(self,player)
	-- get the flag number off the object, set through HF
	local flag = self:GetVar('flag')
	-- if there isnt a flag number, something is wrong and kill the script before it crashes
	if not flag then return end
		
	-- see if the player has built this pipe before
	if player:GetFlag{iFlagID = flag}.bFlag then 
		-- go to custom function
		SpawnPipe(self)
	end
end

-- server has notified the client script because the player built the quickbuild pipe
function onNotifyClientObject(self, msg)

	if msg.name == "PipeBuilt" then 
		-- go to custom function
		SpawnPipe(self)
	end
	
end

-- CUSTOM FUNCTION: spawns static pipe and kills the fx object
function SpawnPipe(self)

	-- get the flag number off the object, set through HF
	local flag = self:GetVar('flag')
	-- parse the flag number to get the last digit
	local pipeNum = tonumber(string.sub(tostring(flag),-1,-1))
	-- get the fx object assicated with this quickbuild using the last number of the flag number
	local fxObj = self:GetObjectsInGroup{group = "BeamPipeFX"..pipeNum, ignoreSpawners = true}.objects
	
	-- make sure the object exists and delete it
	for k,v in ipairs(fxObj) do
		if v:Exists() then
			GAMEOBJ:DeleteObject(v)
		end
	end
		
	-- get the position and rotation of the quickbuild
	local mypos = self:GetPosition().pos
	local myrot = self:GetRotation()
	
	-- spawn in the static pipe at the quickbuild pipes position
	RESMGR:LoadObject { objectTemplate =  12939, x = mypos.x , y = mypos.y , z = mypos.z , rw = myrot.w, rx = myrot.x, ry = myrot.y , rz = myrot.z, owner = self, configData = config }
end