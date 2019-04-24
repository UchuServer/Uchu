function onCollisionPhantom(self, msg)
    local target = msg.objectID    
    local lead = self:GetVar('leadIn')
    if lead == nil then
        lead = 0.5
    end
    
    if target then
        target:PlayCinematic{ pathName = self:GetVar('camera'), leadIn = lead}
    end
end

-- onBouncerTriggered not working !!!! --

--function onBouncerTriggered(self, msg)
--    local target = msg.objId
    
--    if target == GAMEOBJ:GetLocalCharID() then
--        target.PlayCinematic{ pathName = bCam, leadIn = 0.5}
--    end
--end
