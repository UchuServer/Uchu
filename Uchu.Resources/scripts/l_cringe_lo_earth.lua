----------------------------------------
-- client side script for cringe lo in the earth temple
-- requiring mission visibility script, which does all the work
--
-- created by brandi... 6/15/11
---------------------------------------------

require('L_SET_VISIBILITY_FROM_MISSION')

local MissionVisTable = {	
							{missionID = 1823, state = 2,visible = true},
							{missionID = 1826, state = 2,visible = false}
							
						}
local startVisible = false

function onStartup(self)
	setVisTableVariables(self, MissionVisTable,startVisible)	
end 
