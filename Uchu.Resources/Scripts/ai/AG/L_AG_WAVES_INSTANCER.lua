--local misID = 479 -- missionID from DB

--function onUse(self, msg)
--    -- get player who clicked on us
--	local player = msg.user
--	if (player:GetMissionState{missionID = misID}.missionState > 1 ) then --(player:GetMissionState{missionID = misID}.missionState > 4 ) -- complete
--        self:NotifyClientObject{name = 'ShowWaveHelpUI', paramObj = player, rerouteID = msg.player}
--    else 
--        player:DisplayTooltip{ bShow = true, strText = "Visit Nimbus Station to unlock this Survival Mini-Game!", iTime = 3500 } --print('you need to finish mission ' .. misID)
--	end
	
--	--self:Help{rerouteID = player, iHelpID = 0}  
--end