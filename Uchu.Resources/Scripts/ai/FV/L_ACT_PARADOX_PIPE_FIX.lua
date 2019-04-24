------------------------------------------------------------------------------
--paradox refinery pipe quickbuilds that update the mission once all are built
--
--Created by SY 7/28/10
------------------------------------------------------------------------------



function onRebuildComplete(self, msg)
    
    local myGroup = "AllPipes"
    local groupObjs = self:GetObjectsInGroup{ group = myGroup, ignoreSpawners = true }.objects	
    local indexCount = 0
            
    self:SetVar("PlayerID", "|" .. msg.userID:GetID())

    for k,v in ipairs(groupObjs) do
        if v:GetRebuildState().iState == 2 then
            indexCount = indexCount + 1
        end
    end

    if indexCount >= 2 then
             
        local refinery = self:GetObjectsInGroup{ group = "Paradox", ignoreSpawners = true }.objects[1]
        
        if refinery then
            refinery:PlayFXEffect{name = "pipeFX", effectID = 3999, effectType = "create"}
        end

        for k,v in ipairs(groupObjs) do
            local player = GAMEOBJ:GetObjectByID(v:GetVar("PlayerID"))
            
            if player and player:Exists() then
		        player:UpdateMissionTask{taskType = "complete", value = 769, value2 = 1, target = self}
                player:PlayCinematic{pathName = "ParadoxPipeFinish", leadIn = 2.0}
		    end  
            
            v:SetVar("PlayerID", false)          
        end
    end
end


function onRebuildNotifyState(self, msg)

    if msg.iState == 4 then
        local refinery = self:GetObjectsInGroup{ group = "Paradox", ignoreSpawners = true }.objects[1]
            
        if refinery then
            refinery:StopFXEffect{name = "pipeFX"}
        end
    end    
end
