--------------------------------------------------------------
-- Script on the red X to spawn certain pets digs
-- 
-- created by brandi 3/9/10
-- updated by mrb... 1/18/11 - kill the treasure chest onDie if it's alive
--------------------------------------------------------------

function onRebuildComplete(self,msg)	
	--get the position and rotation of the X to spawn the pet treasure node in the same location.
	local oPos = { pos = {}, rot = {}}
	
	oPos.pos = self:GetPosition().pos
	
	local player = msg.userID:GetID()
	
	-- if this is one of the x builds in GF, they have flagNum config data put on them in HF
	if self:GetVar("flagNum") then	
		--load the pet treasure node, passing the player the built the X as a varible and the config data of the collible flag number
		local config = { { "groupID" , "Flag"..self:GetVar("flagNum") },{ "builder" , '|'..player },{ "X" , '|' .. self:GetID() } } --X is to smash this quickbuild
		
		RESMGR:LoadObject { objectTemplate = 7410, x= oPos.pos.x, y= oPos.pos.y + .5 , z= oPos.pos.z, owner = self, configData = config}	
	-- the only other x build is in AG
	else	
		local mission = msg.userID:GetMissionState{missionID = 746}.missionState
		
		--if the player is not on the missions 746 from Captain jack, then spawn a normal pet dig
		if mission ~= 2  then		    
		    local config = { { "builder" , '|'..player },{ "X" , '|' .. self:GetID() } } --X is to smash this quickbuild

			RESMGR:LoadObject { objectTemplate = 3495, x= oPos.pos.x, y= oPos.pos.y + .5 , z= oPos.pos.z, owner = self, configData = config}		
		-- the player is on mission 746, spawn the pet dig that spits out pic of Captain Jacks mom
		else			
			local config = { { "builder" , '|'..player },{ "X" , '|' .. self:GetID() } } --X is to smash this quickbuild
			
			RESMGR:LoadObject { objectTemplate = 9307, x= oPos.pos.x, y= oPos.pos.y + .5 , z= oPos.pos.z, owner = self, configData = config}			
		end		
	end	
end

function onChildLoaded(self, msg)	
	local childLOT = msg.childID:GetLOT().objtemplate or 0
	
	if childLOT == 3495 or childLOT == 9307 or childLOT == 7410 then
		self:SetVar("chestObj", "|" .. msg.childID:GetID())
	end
end

function onDie(self, msg)
	local chestID = self:GetVar("chestObj") or 0
	local chestObj = GAMEOBJ:GetObjectByID(chestID)
	
	if chestObj:Exists() then	
		-- killing the treasure node with violent is what spits out the loot, and the loot shows up for the pet owner
		chestObj:RequestDie{ killType = "SILENT" , killerID = self}
	end
end 