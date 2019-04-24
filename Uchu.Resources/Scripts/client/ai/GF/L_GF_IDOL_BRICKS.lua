function onMissionDialogueOK(self,msg)
    --print('onMissionDialogueOK ' .. msg.iMissionState .. ' - ' .. msg.missionID)
    
    if msg.missionID == 381 then
        if msg.iMissionState < 2 then
            --print('give duck bricks')
            msg.responder:AddItemToInventory{iObjTemplate = 610, itemCount = 1}
            msg.responder:AddItemToInventory{iObjTemplate = 109, itemCount = 2}
            msg.responder:AddItemToInventory{iObjTemplate = 1199, itemCount = 1}
            msg.responder:AddItemToInventory{iObjTemplate = 576, itemCount = 1}
			msg.responder:AddItemToInventory{iObjTemplate = 72, itemCount = 2}
        --[[elseif msg.iMissionState == 4 then
            --print('player has ' .. msg.responder:GetInvItemCount{iObjTemplate = 6086}.itemCount .. ' thinking caps 1')
            if msg.responder:GetInvItemCount{iObjTemplate = 6086}.itemCount > 0 then
                print('remove thinking cap 1')
                -- thinking cap = 6086
                msg.responder:RemoveItemFromInventory{iObjTemplate = 6086}
            end]]--
        end          
    end    
    
end 