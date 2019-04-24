require('o_mis')

function onStartup(self)

	
end

function onCollisionPhantom (self,msg)

	 local player = msg.objectID
 	 if  player:GetFaction().faction == 100  or player:GetFaction().faction == 101 then 
         
            if GAMEOBJ:GetZoneControlID():GetVar(player:GetName().name) == "A" then
            
                local spawnPoint = self:GetObjectsInGroup{ group = "spawn_blue" }.objects
                for i = 1, table.maxn (spawnPoint) do
                    
                    	local ran = math.random(1,4)        
                        local spawnpos = spawnPoint[ran]:GetPosition().pos
                        player:Teleport{pos = spawnpos }
                     
                   
                    break
                end
            else
                 local spawnPoint = self:GetObjectsInGroup{ group = "spawn_red" }.objects
                for i = 1, table.maxn (spawnPoint) do
                    
                    	local ran = math.random(1,4)        
                        local spawnpos = spawnPoint[ran]:GetPosition().pos
                        player:Teleport{pos = spawnpos }
                     
                  
                    break
                end       
            
            end
      end
end
