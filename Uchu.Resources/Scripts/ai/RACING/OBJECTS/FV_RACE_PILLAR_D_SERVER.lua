function onStartup(self)

    local Pillar = self:GetObjectsInGroup{ group = "pillars"}.objects

    for i = 1, table.maxn (Pillar) do
        if Pillar[i]:GetLOT().objtemplate == 11949 then
            print "Preload Pillar D"
            Pillar[i]:PreloadAnimation{animationID = "crumble", respondObjID = self}
        end
    end

end

function onCollisionPhantom(self, msg)

    local player = msg.objectID
    local lap = player:VehicleGetCurrentLap{}.uiCurLap

    if lap == 3 then
        print "collide lap 3!"
        local Pillar = self:GetObjectsInGroup{ group = "pillars"}.objects

        for i = 1, table.maxn (Pillar) do
            if Pillar[i]:GetLOT().objtemplate == 11949 then
                print "Play Pillar D"
                Pillar[i]:PlayAnimation{animationID = "crumble"}
            end
        end

        local Drag = self:GetObjectsInGroup{ group = "dragon"}.objects

        for i = 1, table.maxn (Drag) do
            if Drag[i]:GetLOT().objtemplate == 11898 then
                print "Play Dragon Roar"
                Drag[i]:PlayAnimation{animationID = "roar"}
            end
        end
        print "Delete pillar 2 collider"
        GAMEOBJ:DeleteObject(self)
    end

end
