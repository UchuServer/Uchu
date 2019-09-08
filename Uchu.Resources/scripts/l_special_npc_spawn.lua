require('o_mis')
function onStartup(self, msg)

    self:SetVar("isTable_spawn", false ) 

    self:SetVar("RareSpawned", false)
end


function onFireEvent(self, msg)
 

 
 



 

    if  msg.args == "spawn" then
        if self:GetVar("rarespawn") ~= nil then
            local s = self:GetVar("rarespawn")
            local t = split(s, '-')
            local percent = t[2]
            local ran = math.random(1,100) 
             
             if tonumber(percent) <= ran and tonumber(percent) >= ran then
                self:SetVar("RareSpawned", true)
                local mypos = self:GetPosition().pos
                local myRot = self:GetRotation()
                RESMGR:LoadObject { objectTemplate = t[1], x= mypos.x, y= mypos.y , z= mypos.z, owner = self, rw= myRot.w, rx= myRot.x, ry= myRot.y, rz = myRot.z } 
             end

        end
      end 
      if not self:GetVar("RareSpawned") then
                if string.find (self:GetVar("spawn"), "-") then
                    self:SetVar("isTable_spawn", true ) 
                    local s = self:GetVar("spawn")
                    local t = split(s, '-')
                    local ran = math.random(1,#t) 

                    local mypos = self:GetPosition().pos
                    local myRot = self:GetRotation()
                    RESMGR:LoadObject { objectTemplate = t[ran], x= mypos.x, y= mypos.y , z= mypos.z, owner = self, rw= myRot.w, rx= myRot.x, ry= myRot.y, rz = myRot.z } 
               
                end 
          
        end

    
end


