--------------------------------------------------------------
-- Server side script on the crab pet
-- this script control the crab life before he is tamed by a player

-- created by Brandi... 2/17/10
-- updated by Dallas    7/29/10 - fixed 'or' statement
--------------------------------------------------------------
function onStartup(self)
	-- if the pet is someones tamed pet, ignore the rest of the script
	if self:IsPetWild{}.bIsPetWild == false then
		return
	end
	
	local player = self:GetVar("tamer")
	
	self:SetNetworkVar("crabtamer", player)
	
	--kill the lion after 2 minutes if the player who spawned it doesn't tame it
	GAMEOBJ:GetTimer():AddTimerWithCancel( 45, "killSelf",self )
	
end


function onNotifyPetTamingMinigame(self,msg)

	--if the player begins the taming minigame, cancel the timer that kills the lion
	if msg.notifyType == "BEGIN" then
	
		GAMEOBJ:GetTimer():CancelTimer("killSelf",self)
		
	--if the player fails or quits the minigame, kill the lion
	elseif msg.notifyType == ("QUIT") or msg.notifyType == ("FAILED") then
	
		self:RequestDie{killerID = self, killType = "SILENT"}
		
		
	--if the player succeeds in the minigame, command the lion to go to the player, otherwise he could teleport off 
	-- because the minigame tells him to go to his spawn point, and he doesnt have one
	elseif  msg.notifyType == "SUCCESS" then	
		
		local player = msg.PlayerTamingID
		
		self:CommandPet{iPetCommandType = 6}
		self:RemoveObjectFromGroup{group = "crab"..player:GetID()}
				
	end

end

function onTimerDone (self,msg)
    
    -- kill the lion if he's been alive for too long. Lions have a very short life span
    if (msg.name == "killSelf") then
		
		--double check not to kill a lion that is actually someones pet
		if self:IsPetWild{}.bIsPetWild == false then
			return
		end
		
		self:RequestDie{killerID = self, killType = "SILENT"}
		
	end
	
end
