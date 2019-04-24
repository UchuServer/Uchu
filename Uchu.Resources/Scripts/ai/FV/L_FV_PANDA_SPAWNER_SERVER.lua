----------------------------------------------------------
--client side script to spawn the Panda
----------------------------------------------------------


function onCollisionPhantom(self, msg)

    --print("player collided")
    
    local player = msg.senderID
    
    --print("activity ID = " .. player:GetCurrentActivityID().iActivity)
    
    if player:GetFlag{iFlagID = 81}.bFlag --[[and not player:GetFlag{iFlagID = 82}.bFlag and player:GetCurrentActivityID().iActivity == 48--]] then
    
        --player:SetFlag{iFlagID = 81, bFlag = false}
        
        --self:FireEventClientSide{args = msg.args}
        
        
        local raceObject = self:GetObjectsInGroup{group = "PandaRaceObject", ignoreSpawners = true}.objects[1]
      
        if raceObject then
            
            --print("found race object")
            
            if raceObject:ActivityUserExists{userID = player}.bExists then
            
                --print("here comes the panda")
                
                local pandas = self:GetObjectsInGroup{ group = "pandas", ignoreSpawners = true }.objects
                local playerpanda = self:GetObjectsInGroup{ group = "panda"..player:GetID(), ignoreSpawners = true }.objects
                
                if #playerpanda > 0 then 
	
                    --print("You already spawned a panda")
                    self:FireEventClientSide{args = "playerPanda", senderID = player}
                    return 
		
                end
        
                if #pandas > 4 then 
	
                    --print("There are too many pandas right now, try again in a few minutes")
                    self:FireEventClientSide{args = "tooManyPandas", senderID = player}
                    return 
		
                end
        
                --print("spawning Panda Pet")
        
                --mypos.x = mypos.x + 20

                --set the tamer as the player to check that only this player can tame that panda
                -- and set the group id to include the player id to make sure this player can only spawn one panda at a time
                local config = { { "tamer", player:GetID() } , { "groupID", "panda"..player:GetID()..";pandas" } }
                local mypos = self:GetPosition().pos

                RESMGR:LoadObject { objectTemplate = 5643 , x = mypos.x , y = mypos.y , z = mypos.z ,owner = self, configData = config }
            end
        end
    end
end