--------------------------------------------------------------
-- Server side script on the panda pet
-- this script control the pandas life before he is tamed by a player

-- created by Steve... 
-- created from Brandi's Lion script... 2/17/10
-- updated by Dallas                    7/29/10 - fixed 'or' statement
--------------------------------------------------------------




function onStartup(self)
	-- if the pet is someones tamed pet, ignore the rest of the script
	if self:IsPetWild{}.bIsPetWild == false then
		return
	end
	
	local player = self:GetVar("tamer")
	
	self:SetNetworkVar("pandatamer", player)
	
	--kill the panda after 2 minutes if the player who spawned it doesn't tame it
	GAMEOBJ:GetTimer():AddTimerWithCancel( 45, "killSelf",self )
    
    --print("panda pet server script starting up")
	
end





function onNotifyPetTamingMinigame(self,msg)

	--if the player begins the taming minigame, cancel the timer that kills the panda
	if msg.notifyType == "BEGIN" then
	
		GAMEOBJ:GetTimer():CancelTimer("killSelf",self)
		
	--if the player fails or quits the minigame, kill the panda
	elseif msg.notifyType == ("QUIT") or msg.notifyType == ("FAILED") then
	
		self:Die{killerID = self, killType = "SILENT"}
		
		
	--if the player succeeds in the minigame, command the panda to go to the player, otherwise he could teleport off 
	-- because the minigame tells him to go to his spawn point, and he doesnt have one
	elseif  msg.notifyType == "SUCCESS" then	
		
		local player = msg.PlayerTamingID
		
		self:CommandPet{iPetCommandType = 6}
		local pandas = self:GetObjectsInGroup{ group = "pandas", ignoreSpawners = true }.objects
		--print("before "..#pandas)
		
		--print("remove object from group panda"..player:GetID())
		self:RemoveObjectFromGroup{group = "pandas"}
		self:RemoveObjectFromGroup{group = "panda"..player:GetID()}
		
		local pandas2 = self:GetObjectsInGroup{ group = "pandas", ignoreSpawners = true }.objects
		--print("after "..#pandas2)
        
        player:SetFlag{iFlagID = 82, bFlag = true}
	end

end





function onTimerDone (self,msg)
    
    --print("panda server timer done")
    
    -- kill the panda if he's been alive for too long. pandas have a very short life span
    if (msg.name == "killSelf") then
		
		--print("panda timer finished, killSelf")
        
        --double check not to kill a panda that is actually someones pet
		if self:IsPetWild{}.bIsPetWild == false then            
            --print("pet is wild")
            return
		end
		
		self:Die{killerID = self, killType = "SILENT"}
		
	end
	
end
