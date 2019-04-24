function onStartup(self)

    local Pillar = self:GetObjectsInGroup{ group = "pillars"}.objects

    for i = 1, table.maxn (Pillar) do
        if Pillar[i]:GetLOT().objtemplate == 11946 then
            print "Preload Pillar A"
            Pillar[i]:PreloadAnimation{animationID = "crumble", respondObjID = self}
        elseif Pillar[i]:GetLOT().objtemplate == 11947 then
            print "Preload Pillar B"
            Pillar[i]:PreloadAnimation{animationID = "crumble", respondObjID = self}
        elseif Pillar[i]:GetLOT().objtemplate == 11948 then
            print "Preload Pillar C"
            Pillar[i]:PreloadAnimation{animationID = "crumble", respondObjID = self}
        end
    end

    local Drag = self:GetObjectsInGroup{ group = "dragon"}.objects

    for i = 1, table.maxn (Drag) do
        if Drag[i]:GetLOT().objtemplate == 11898 then
            print "Preload Dragon Roar"
            Drag[i]:PreloadAnimation{animationID = "roar", respondObjID = self}
        end
    end    

end

function onCollisionPhantom(self, msg)

    local player = msg.objectID
    local lap = player:VehicleGetCurrentLap{}.uiCurLap

    if lap == 2 then
        print "collide lap 2!"

        local Pillar = self:GetObjectsInGroup{ group = "pillars"}.objects

        for i = 1, table.maxn (Pillar) do
            if Pillar[i]:GetLOT().objtemplate == 11946 then
                print "Play Pillar A"
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

        print "Start Timers"

        GAMEOBJ:GetTimer():AddTimerWithCancel( 2.5, "PillarBFall", self )
        GAMEOBJ:GetTimer():AddTimerWithCancel( 3.7, "PillarCFall", self )
        GAMEOBJ:GetTimer():AddTimerWithCancel( 3.8, "DeleteObject", self )

    end

end

function onTimerDone(self, msg)
print "Timer?"
	if msg.name == "PillarBFall" then
        local Pillarb = self:GetObjectsInGroup{ group = "pillars"}.objects

        for i = 1, table.maxn (Pillarb) do
            if Pillarb[i]:GetLOT().objtemplate == 11947 then
                print "Play Pillar B"
                Pillarb[i]:PlayAnimation{animationID = "crumble"}
            end
        end        

    elseif msg.name == "PillarCFall" then
        local Pillarc = self:GetObjectsInGroup{ group = "pillars"}.objects

        for i = 1, table.maxn (Pillarc) do
            if Pillarc[i]:GetLOT().objtemplate == 11948 then
                print "Play Pillar C"
                Pillarc[i]:PlayAnimation{animationID = "crumble"}
            end
        end        

    elseif msg.name == "DeleteObject" then
        print "Delete pillar 1 collider"
        GAMEOBJ:DeleteObject(self)
    end

end 
