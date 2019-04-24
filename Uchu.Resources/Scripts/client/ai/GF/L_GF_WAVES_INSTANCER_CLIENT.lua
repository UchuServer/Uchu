--------------------------------------------------------------
-- Description:
--
-- Client script for AG Survival Instancer.
-- Lets client know the object can be interacted with
-- updated mrb... 3/23/10
--------------------------------------------------------------
require('client/ai/MINIGAME/BASE_INSTANCER')

local tVars = {
     releaseVersion = 182, -- which version release # the content should be made available for Beta 1
     misID = 381,
     missionState = 8,
     transferZoneID = 1301,
     UI_Type = "GF_Survival_01",
     failText = Localize("MINIGAME_LOBBY_GF_SUVIVAL_FAIL_MESSAGE"),}
     
function onStartup(self)
    baseSetVars(self, tVars)
end
