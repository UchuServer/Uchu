-- When the flying dragon arrives at certain waypoints it tells objects on the platforms to fire skills.
function onArrived(self, msg)
    if msg.wayPoint == 4 then
        self:PlayAnimation{ animationID = "attack1", fPriority = 2 }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1.75 , "platform1attack", self )
    end
    
    if msg.wayPoint == 12 then
        self:PlayAnimation{ animationID = "attack2", fPriority = 2 }
        local group2 = self:GetObjectsInGroup{ group = "dragonFireballs2" , ignoreSpawners = true}.objects
        
        if group2 == nil then
            return
        end
        group2[1]:CastSkill { skillID = 762 }
        local minionCount = 1
        for i = 2, #group2 do
            if minionCount == 4 then  
                return
            end
            if math.random(1,5) > 3 then
                group2[i]:CastSkill { skillID = 762 }
                minionCount = minionCount + 1
            end
	    end
    end
    
    if msg.wayPoint == 16 then
        self:PlayAnimation{ animationID = "attack3", fPriority = 2 }
        GAMEOBJ:GetTimer():AddTimerWithCancel( .5 , "platform3attack", self )
    end
end


function onTimerDone(self, msg)
  if msg.name == "platform1attack" or msg.name == "platform3attack"  then
        local groupName 
        if msg.name == "platform1attack" then
            groupName = "dragonFireballs1"
        elseif msg.name == "platform3attack" then
            groupName = "dragonFireballs3"
        end

        local group = self:GetObjectsInGroup{ group = groupName , ignoreSpawners = true}.objects 
        if group == nil then
            return
        end
        group[1]:CastSkill { skillID = 762 }
        local minionCount = 1
        for i = 2, #group do 
            if minionCount == 4 then  
                return
            end 
            if math.random(1,5) > 3 then         
                group[i]:CastSkill { skillID = 762 }
                minionCount = minionCount + 1
            end
        end
   end
end
