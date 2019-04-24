function onStartup(self)

    self:SetVar("Collider", 0)
    local HeroShip = self:GetObjectsInGroup{ group = "GoodShip"}.objects

    for i = 1, table.maxn (HeroShip) do
        if HeroShip[i]:GetLOT().objtemplate == 9529 then
            HeroShip[i]:PreloadAnimation{animationID = "lap_02", respondObjID = self}
            break
        elseif HeroShip[i]:GetLOT().objtemplate == 10002 then
            HeroShip[i]:PreloadAnimation{animationID = "lap_02", respondObjID = self}
            break
        end
    end

end

function onCollisionPhantom(self, msg)

    local player = msg.objectID
    local lap = player:VehicleGetCurrentLap{}.uiCurLap

	if player:GetID() == GAMEOBJ:GetControlledID():GetID() and self:GetVar("Collider") == 0 then
        if lap == 2 then
            self:SetVar("Collider", 1)
            local HeroShip = self:GetObjectsInGroup{ group = "GoodShip"}.objects

            for i = 1, table.maxn (HeroShip) do
                if HeroShip[i]:GetLOT().objtemplate == 9529 then
                    local Ship = HeroShip[i]
                    Ship:PlayAnimation{animationID = "lap_02"}
                    Ship:SetOffscreenAnimation{bAnimateOffscreen = true}
                    break
                elseif HeroShip[i]:GetLOT().objtemplate == 10002 then
                    local Ship = HeroShip[i]
                    Ship:PlayAnimation{animationID = "lap_02"}
                    Ship:SetOffscreenAnimation{bAnimateOffscreen = true}
                    -- Adding timer to remove chevron at appropriate time during animation
                    --print "Start timer"
                    GAMEOBJ:GetTimer():AddTimerWithCancel( 1.6, "DeleteChevron", self )
                    break
                end
            end
        end
    end

end

function onTimerDone(self, msg)

	if msg.name == "DeleteChevron" then
        --print "Not finding chevron"
        local Chevron = self:GetObjectsInGroup{ group = "GoodShip"}.objects

        for i = 1, table.maxn (Chevron) do
            if Chevron[i]:GetLOT().objtemplate == 10003 then
                local Chevy = Chevron[i]
                --print "Deleting Chevron"
                GAMEOBJ:DeleteObject(Chevy)
                break
            end
        end
    end

end 
