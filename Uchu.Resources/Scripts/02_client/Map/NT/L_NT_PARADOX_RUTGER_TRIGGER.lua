----------------------------------------------------------------
---- Client side script on Rutger, the vampire mission giver/vendor

---- created by brandi.. 2/15/11
----------------------------------------------------------------

---- mission to talk to Rutger from Olivia
--local ToStomMis		= 1396
---- mission to heal Rutger from Overbuild
--local healMisID		= 1399

------------------------------------------------
---- When the render on Rutger is loaded for the client
------------------------------------------------
--function onCollisionPhantom(self,msg)
--	-- get the player
--	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
--	-- check to see if the player has taken the mission from Olivia and not healed Rutger net
--	if player:GetMissionState{missionID = ToStomMis}.missionState >= 2 and player:GetMissionState{missionID = healMisID}.missionState < 4 then
--		-- get Rutger
--		local Rutger = self:GetObjectsInGroup{group = "Rutger", ignoreSpawners = true}.objects[1]
--		-- if Rutger isnt found return out
--		if not Rutger then return end
--		-- notify rutger to turn his fx on, this is in case he hasnt unloaded from clients machine yet
--		Rutger:NotifyClientObject{ name = "StartMaelstrom" , rerouteID = player }
--	end
	
--end