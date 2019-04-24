-- Dragon Collision Volume to preload and play a certain animation on a certain lap
-- Updated by EB, 10.12.10 - Changed lap numbering and added exist checks

function onStartup(self)

    local Drag = self:GetObjectsInGroup{ group = "dragon"}.objects

    for i = 1, table.maxn (Drag) do
        if Drag[i]:Exists() then
            if Drag[i]:GetLOT().objtemplate == 11898 then
                Drag[i]:PreloadAnimation{animationID = "lap_01", respondObjID = self}
            end
        end
    end

end

function onCollisionPhantom(self, msg)

    local player = msg.objectID

    if not player:Exists() then return end

    local lap = player:VehicleGetCurrentLap{}.uiCurLap

    if lap == 3 then
        local Drag = self:GetObjectsInGroup{ group = "dragon"}.objects

        for i = 1, table.maxn (Drag) do
            if Drag[i]:Exists() then
                if Drag[i]:GetLOT().objtemplate == 11898 then
                    Drag[i]:PlayAnimation{animationID = "lap_01"}
                end
            end
        end
        GAMEOBJ:DeleteObject(self)
    end

end
