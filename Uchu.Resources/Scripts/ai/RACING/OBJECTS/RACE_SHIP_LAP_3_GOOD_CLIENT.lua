function onStartup(self)

    local HeroShip = self:GetObjectsInGroup{ group = "GoodShip"}.objects

    for i = 1, table.maxn (HeroShip) do
        if HeroShip[i]:GetLOT().objtemplate == 9529 then
            HeroShip[i]:PreloadAnimation{animationID = "lap_03", respondObjID = self}
            break
        elseif HeroShip[i]:GetLOT().objtemplate == 10001 then
            HeroShip[i]:PreloadAnimation{animationID = "lap_03", respondObjID = self}
            break
        end
    end

end

function onCollisionPhantom(self, msg)

    local player = msg.objectID
    local lap = player:VehicleGetCurrentLap{}.uiCurLap

	if player:GetID() == GAMEOBJ:GetControlledID():GetID() then
        if lap == 3 then
            local HeroShip = self:GetObjectsInGroup{ group = "GoodShip"}.objects

            for i = 1, table.maxn (HeroShip) do
                if HeroShip[i]:GetLOT().objtemplate == 9529 then
                    local Ship = HeroShip[i]
                    Ship:PlayAnimation{animationID = "lap_03"}
                    Ship:SetOffscreenAnimation{bAnimateOffscreen = true}
                    break
                elseif HeroShip[i]:GetLOT().objtemplate == 10001 then
                    local Ship = HeroShip[i]
                    Ship:PlayAnimation{animationID = "lap_03"}
                    Ship:SetOffscreenAnimation{bAnimateOffscreen = true}
                    break
                end
            end
            GAMEOBJ:DeleteObject(self)
        end
    end

end
