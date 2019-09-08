require('o_mis')

function onStartup(self) 

	 self:SetVar("currentSpawn", 1)	
	 self:SetVar("Debug", false)
end



function onPlayerLoaded(self,msg)

	local player = msg.playerID 

	for i = 1, 4 do
		if self:GetVar("Con.player_"..i) == nil then
			self:SetVar("Con.player_"..i, player:GetID() )
			player:SetRespawnGroup{findClosest=true, respawnGroup= "spawn1"}
			-------------------------------------------------------------------------------
			-- Temp
			local obj = self:GetObjectsInGroup{ignoreSpawners=true, group = "spawn1" }.objects
			local Markpos = obj[1]:GetPosition().pos 
			local Markrot = obj[1]:GetRotation()	
			player:Teleport{pos = Markpos, y = Markrot.y, w = Markrot.w, bSetRotation = true }
			
			if self:GetVar("Debug") then
				player:SetMaxImagination{imagination = 20}
				player:SetMaxArmor{armor = 2000 }
				player:SetMaxHealth{health = 20}			
				player:SetHealth{health = 20}
				player:SetArmor{armor = 2000}
				player:SetImagination{imagination = 20}
			end
			
			---------------------------------------------------------------------------------
			break
		end
	
	end
	
	
end

function onPlayerExit( self, msg)
		
	local player = msg.playerID
		
	for i = 1, 4 do
		if self:GetVar("Con.player_"..i) == player:GetID() then
			self:SetVar("Con.player_"..i, nil )
			break
		end
	
	end
	
end

--------------------------------------------------------------
-- Sent from a player when responding from a messagebox
--------------------------------------------------------------
function onMessageBoxRespond(self, msg)	

    local player = msg.sender
    
    if msg.identifier == "reset" then
	 	local ActivityObj = self:GetObjectsInGroup{ group = "ActivityObj" ,ignoreSpawners = true }.objects
	 	
	 	ActivityObj[1]:NotifyObject{name = "reset" }
	elseif msg.identifier == "intbosshearts" then
		local ActivityObj = self:GetObjectsInGroup{ group = "ActivityObj" ,ignoreSpawners = true }.objects
		
		ActivityObj[1]:NotifyObject{name = "intbossGUI" }
	
	
	end
	
	
end
