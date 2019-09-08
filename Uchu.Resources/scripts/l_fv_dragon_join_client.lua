--------------------------------------------------------------
-- Description:
--
-- Client script for FV Dragon Instance.
-- Lets client know the object can be interacted with
-- updated mrb... 7/16/10
--------------------------------------------------------------
require('BASE_INSTANCER')

local tVars = {
     releaseVersion = 182, -- which version release # the content should be made available for Beta 1
     UI_Type = "FV_Dragon_01",}
     
function onStartup(self)
    baseSetVars(self, tVars)
end
