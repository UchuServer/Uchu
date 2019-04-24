-------------------------------------------------------------------
--Plays FX for the player when the cross the finish line on their last lap
-------------------------------------------------------------------

function onCollisionPhantom(self, msg)

    local player = msg.objectID
    local lap = player:VehicleGetCurrentLap{}.uiCurLap

	if player:GetID() == GAMEOBJ:GetControlledID():GetID() then
        
        if lap == 3 then
        
            local object = self:GetObjectsInGroup{group = "Track", ignoreSpawners = true}.objects[1]

            if object then
                
                object:PlayFXEffect{name = "FinishFX", effectID = 3577, effectType = "create"}
            end
        end
    end

end
