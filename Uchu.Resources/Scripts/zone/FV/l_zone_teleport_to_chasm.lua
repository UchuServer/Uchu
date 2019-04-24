


function onCollisionPhantom(self, msg)
        local player = msg.objectID
		if( player:CheckPrecondition{PreconditionID = 60}.bPass == true ) then
			local object = self:GetObjectsInGroup{group = "switchValley", ignoreSpawners = true}.objects[1]
			if object then
				local tele = object:GetPosition().pos
				player:Teleport {pos = {x = tele.x + 4, y =  tele.y - 10, z = tele.z + 4}, bIgnoreY = false}
				print("x = 783, y =  200 z = 123")
				--object:PlayFXEffect{name = "febuildpop", effectID = 105, effectType = "create"}
			end
		end
end

