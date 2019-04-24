-----------------------------------------------------------------------
--volume that destroys the monster
-----------------------------------------------------------------------



function onCollisionPhantom(self, msg)

   --print("collided")

    local playerID = GAMEOBJ:GetControlledID():GetID()
    
    if (msg.objectID:GetID() == playerID) then
    
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "killMonster", self )
      
        local volume = self:GetObjectsInGroup{group = "MonsterSpawnVolume", ignoreSpawners = true}.objects[1]
      
        if volume then
            volume:NotifyObject{name = "MonsterDespawned"}
        end
       
        local gate = self:GetObjectsInGroup{group = "Gate", ignoreSpawners = true}.objects[1]
       
        if gate then
            gate:NotifyObject{name = "GateActive"}
        end
    end

end


function onTimerDone(self, msg)

    if msg.name == "killMonster" then
    
        monster = self:GetObjectsInGroup{group = "Monster"}.objects[1]
        
        if monster then
            monster:Die()
        end
    
    end

end