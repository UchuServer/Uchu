require('o_mis')

function onRenderComponentReady(self, msg)

    self:AddObjectToGroup{ group = "Orange" }

end

function onClientUse(self, msg)

    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	storeObjectByName(self, "TeleGuy", player)

	GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "TeleToBlue",self )

end

function onTimerDone(self, msg)

	if msg.name == "TeleToBlue" then
        local BlueObject = self:GetObjectsInGroup{ group = "Blue"}.objects

        for i = 1, table.maxn (BlueObject) do      
            if BlueObject[i]:GetLOT().objtemplate == 5895 then
                local bluetelepos = BlueObject[i]:GetPosition().pos
                local bluex = bluetelepos.x
                local bluey = bluetelepos.y + 2
                local bluez = bluetelepos.z + 2
                getObjectByName(self,"TeleGuy"):SetPosition {pos = {x = bluex, y = bluey, z = bluez}}
            end              
        end
    end

end

function onGetOverridePickType(self, msg)
    msg.ePickType = 14
	return msg
end