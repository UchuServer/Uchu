--------------------------------------------------------------
-- Description: Client side script to hide pedestals on
-- external builds, qa/beta/live
--
-- Created 4/27/10 mrb... 
--------------------------------------------------------------
function onStartup(self)
    local verInfo = self:GetVersioningInfo()
    
    -- check to see if the object is supposed to be blocked for beta
    if not verInfo.bIsInternal  then
        self:SetVisible{visible = false, fadeTime = 0}
    end
end 