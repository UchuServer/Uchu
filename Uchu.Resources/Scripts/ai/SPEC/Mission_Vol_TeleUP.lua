require('o_mis')
function onStartup(self) 
self:SetVar("found", 0)
self:SetVar("ready", false)
end


function onCollisionPhantom(self, msg)
   
    local target = msg.senderID
   if not self:GetVar("ready") then
        local obj = self:GetObjectsInGroup{ group = "rb" }.objects
        for i = 1, #obj do      
            if obj[i]:GetLOT().objtemplate == 4984 then 
                if   obj[i]:GetVar("Built") then
                    self:SetVar("found", self:GetVar("found") + 1)
                end
            end
             if obj[i]:GetLOT().objtemplate == 4846 then 
                 storeObjectByName(self, "telespot", obj[i])
             end
             
        end
        if self:GetVar("found") >= 2 then
             self:SetVar("ready", true)
             self:SetVar("found", 0)
           
        end
        
        
   end
    local faction = target:GetFaction()
    local mstate = target:GetMissionState{missionID = 239}.missionState
    
    if faction and faction.faction == 1 and mstate == 2 and self:GetVar("ready") then
        local spawn =  getObjectByName(self, "telespot")
        local Markpos = spawn:GetPosition().pos 
        local Markrot = spawn:GetRotation()                    
        target:Teleport{pos = Markpos, y = Markrot.y, w = Markrot.w, bSetRotation = true }
        self:SetVar("ready", false)
    
   end
    
end


