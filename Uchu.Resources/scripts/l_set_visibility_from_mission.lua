----------------------------------------
-- client side script for any object that you want to make visible or invisible based on mission state
-- requiring mission toggle script, for everything except the check missions function, this script will override that
--
-- created by brandi... 6/15/11
---------------------------------------------

require('L_VIS_TOGGLE_OBJ')

local MissionVisTable  = {}
local startVisible = true

function setVisTableVariables(self, passedMissionVisTable,passedstartVisible)
	MissionVisTable = passedMissionVisTable
	startVisible = passedstartVisible
end

----------------------------------------------
-- decide to hide the X or not
----------------------------------------------
function CheckMissions(self,player)
	-- create a visible variable
	local isVisible = startVisible
	for k,VisTable in pairs(MissionVisTable) do  
		local missionState = player:GetMissionState{missionID = VisTable.missionID}.missionState
		-- check to see if the player has reached and passed a specific mission state for a specific mission
		if (missionState >= VisTable.state)  then 
			-- tell the varible to be true or false
			isVisible = VisTable.visible
		
		end
	end
	-- the the visibility based on the variable state from the for loop
	self:SetVisible{visible = isVisible, fadeTime = 0.0}
end

