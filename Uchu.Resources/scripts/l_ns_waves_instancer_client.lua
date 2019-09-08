--------------------------------------------------------------
-- Description:
--
-- Client script for AG Survival Instancer.
-- Lets client know the object can be interacted with
-- updated pml... 11/17/10
--------------------------------------------------------------
require('BASE_INSTANCER')

local tVars = {
     releaseVersion = 182, -- which version release # the content should be made available for Beta 1
     misID = 1246,
     missionState = 2,
     transferZoneID = 1204,
     UI_Type = "NS_Waves_01",
     failText = Localize("MINIGAME_LOBBY_NS_SUVIVAL_FAIL_MESSAGE"),}
     
function onStartup(self)
	self:SetVar("nofade", 1)
    baseSetVars(self, tVars)
end
