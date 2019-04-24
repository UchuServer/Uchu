function onRebuildComplete(self, msg)
       
		local player = msg.userID:GetID()
        local object = self:GetObjectsInGroup{group = "qbtele_01", ignoreSpawners = true}.objects[1]
        if object then
            local tele = object:GetPosition().pos
            msg.userID:Teleport {pos = {x = tele.x, y =  tele.y, z = tele.z}, bIgnoreY = false}
            --object:PlayFXEffect{name = "febuildpop", effectID = 105, effectType = "create"}
        end
end