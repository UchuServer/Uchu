---------------------------------------------------------
--FV QB turret 1
---------------------------------------------------------




function onStartup(self)

   self:SetVar("AmBuilt", false)
end




function onRebuildNotifyState(self, msg)

    if msg.iState == 2 then
      
        self:SetVar("AmBuilt", true)
        --self:PlayFXEffect{name = "laser", effectID = 478, effectType = "create"}
      
        for groupName in string.gmatch(self:GetVar("groupID"), "%w+;") do
            
            --------------------------------------------------------------
            --if the rebuild is complete, get my group name and trim off the ';'s
            --------------------------------------------------------------
            
            groupName = string.sub(groupName, 1, -2)
            mygroup = self:GetObjectsInGroup{group = groupName, ignoreSpawners = true}.objects
            
            --------------------------------------------------------------
            --tell the object in my group with ID 7816 (the horseman) to die
            --tell the effects node (9889) to play effects
            --------------------------------------------------------------
            
            for i, object in ipairs(mygroup) do
            
                if object:GetLOT().objtemplate == 7816 then
            
                    --object:PlayFXEffect{name = "bullet_explosion", effectID = 1133, effectType = create}
                    object:Die{killType = "VIOLENT"}
                
                elseif object:GetLOT().objtemplate == 9889 then
                
                    --object:PlayFXEffect{name = "glow", effectID = 935, effectType = "create"}
                    object:NotifyObject{name = "DoorUp"}
                end
            end
        end
  
    elseif msg.iState == 4 then
      
        self:SetVar("AmBuilt", false)
        --print("iState == 4")
        
        for groupName in string.gmatch(self:GetVar("groupID"), "%w+;") do
            
            --------------------------------------------------------------
            --if I'm broken, find the objects in my group and trim off the ';'s
            --------------------------------------------------------------
            
            groupName = string.sub(groupName, 1, -2)
            mygroup = self:GetObjectsInGroup{group = groupName, ignoreSpawners = true}.objects
            
            --------------------------------------------------------------
            --tell the effects node in my group (9889) to stop playing effects
            --------------------------------------------------------------
            
            for i, object in ipairs(mygroup) do
            
                if object:GetLOT().objtemplate == 9889 then
                    
                    --object:StopFXEffect{name = "glow"}
                    object:NotifyObject{name = "DoorDown"}
                end
            end
        end
    end
end




function onFireEvent(self, msg)

   --print("notify object")
   
   if msg.args == "ISpawned" and self:GetVar("AmBuilt") then
      
      msg.senderID:Die{killType = "VIOLENT"}
   end
end