--****************************************************************
-- THIS SCRIPT IS NO LONGER IN USE. KEPT ONLY FOR REFERENCE
-- PLEASE DELETE BEFORE GOLD MASTER
--******************************************************************
local misID = 482

function onHasBeenCollected(self, msg)
    local iFlag = tonumber(self:GetVar('flagNum'))
    --print(iFlag)
    if iFlag then
        msg.playerID:SetFlag{iFlagID = iFlag, bFlag = true}
    else
        print('missing flagNum configData')
    end
    
    --print(msg.playerID:GetMissionState{missionID = misID}.missionState)
    if msg.playerID:GetMissionState{missionID = misID}.missionState == 2 then        
        --print('updateMission now ' .. msg.playerID:GetName().name)
		msg.playerID:UpdateMissionTask{taskType = "complete", value = misID, value2 = 1, target = self}        
    end
end 