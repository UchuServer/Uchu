--------------------------------------------------------------
-- Description:
--
-- Client script for AG Survival Instancer.
-- Lets client know the object can be interacted with
-- updated mrb... 3/23/10
--------------------------------------------------------------
require('BASE_INSTANCER')

local tVars = {
     releaseVersion = 182, -- which version release # the content should be made available for Beta 1
     misID = 479,
     missionState = 2,
     transferZoneID = 1101,
     UI_Type = "AG_Survival_01",
     failText = Localize("MINIGAME_LOBBY_AG_SUVIVAL_FAIL_MESSAGE"),}
     
function onStartup(self)
    baseSetVars(self, tVars)
end
