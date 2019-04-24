


function onCollisionPhantom(self, msg)
        local player = msg.objectID
		-- print("player id" .. player)
        local object = self:GetObjectsInGroup{group = "switchTree", ignoreSpawners = true}.objects[1]
        if object then
            local tele = object:GetPosition().pos
            player:Teleport {pos = {x = tele.x + 4, y =  tele.y - 10, z = tele.z + 4}, bIgnoreY = false}
            --object:PlayFXEffect{name = "febuildpop", effectID = 105, effectType = "create"}
        end
end

