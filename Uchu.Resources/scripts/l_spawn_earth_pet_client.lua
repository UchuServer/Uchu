--------------------------------------------------------------
-- Client side script on the object to spawn the gryphon pet
-- this script controls the interactablity of the object

-- created by Brandi... 3/2/11
--------------------------------------------------------------

require('L_SPAWN_PET_BASE_CLIENT')

--------------------------------------------------------------
-- set up all the variables needed
--------------------------------------------------------------
local TooManyPetsText = "LION_SUMMON_FAIL"
local TooManyIcon = 4683

--------------------------------------------------------------
-- on startup, put all variables to set vars
--------------------------------------------------------------
function onStartup(self,msg)
	self:SetVar("TooManyPetsText",TooManyPetsText)
	self:SetVar("TooManyPetsIcon",TooManyIcon)
	baseStartup(self,msg)
end
