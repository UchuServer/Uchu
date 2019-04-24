function onMissionDialogueOK(self,msg)
    --print('onMissionDialogueOK ' .. msg.iMissionState .. ' - ' .. msg.missionID)
    
    if msg.missionID == 228 and msg.bIsComplete == true then
		
			local player = msg.responder
			--"Collect Imagination to use as AMMO for your pistol."
			player:DisplayTooltip{ bShow = true, strText = Localize("HELP_FIRST_GUN"), iTime = 3000 }
		
	end
end