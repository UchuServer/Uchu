----------------------------------------
-- client side script for cringe lo in the monastery
-- requiring mission visibility script, which does all the work
--
-- created by brandi... 6/15/11
---------------------------------------------

require('02_client/Map/General/L_SET_VISIBILITY_FROM_MISSION')

local MissionVisTable = {	-- chon ord and mut exclus
							{missionID = 1823, state = 2,visible = false},
							{missionID = 1826, state = 2,visible = true}
						}
local startVisible = true

function onStartup(self)
	setVisTableVariables(self, MissionVisTable,startVisible)	
end 