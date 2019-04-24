require('o_mis')

function onRenderComponentReady(self, msg)

    self:AddObjectToGroup{ group = "Blue" }

end

function onClientUse(self, msg)

    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    storeObjectByName(self, "TeleGuy", player)
    
	GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "TeleToOrange",self )

end

function onTimerDone(self, msg)

	if msg.name == "TeleToOrange" then
        local OrangeObject = self:GetObjectsInGroup{ group = "Orange"}.objects

        for i = 1, table.maxn (OrangeObject) do
            if OrangeObject[i]:GetLOT().objtemplate == 5896 then
                local orangetelepos = OrangeObject[i]:GetPosition().pos
                local orangex = orangetelepos.x
                local orangey = orangetelepos.y + 2
                local orangez = orangetelepos.z + 2
                getObjectByName(self,"TeleGuy"):SetPosition {pos = {x = orangex, y = orangey, z = orangez}}
            end
        end
    end
    
end

function onGetOverridePickType(self, msg)
    msg.ePickType = 14
	return msg
end
