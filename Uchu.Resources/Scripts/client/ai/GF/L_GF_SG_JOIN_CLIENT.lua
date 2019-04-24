--------------------------------------------------------------
-- Description:
--
-- Client script for Shooting Gallery NPC in GF area.
-- Lets client know the object can be interacted with
-- updated mrb... 3/23/10
--------------------------------------------------------------
require('client/ai/MINIGAME/BASE_INSTANCER')

local tVars = {
     releaseVersion = 181, -- which version release # the content should be made available for Beta 1
     misID = 230,
     missionState = 8,
     UI_Type = "GF_SG_01",
     failText = Localize("MINIGAME_LOBBY_GF_SG_FAIL_MESSAGE"),}
     
function onStartup(self)
    baseSetVars(self, tVars)
end