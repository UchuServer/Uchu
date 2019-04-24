--****************************************************************
-- THIS SCRIPT IS NO LONGER IN USE. KEPT ONLY FOR REFERENCE
-- PLEASE DELETE BEFORE GOLD MASTER
--******************************************************************

local misID = 482

function onMissionDialogueOK(self,msg)
    --print('onMissionDialogueOK ' .. msg.iMissionState .. ' - ' .. msg.missionID)
    
    if msg.missionID == misID and msg.iMissionState < 2 then             
        self:FireEventServerSide{ senderID = msg.responder, args = 'updateMission' }
    end    
    
end 