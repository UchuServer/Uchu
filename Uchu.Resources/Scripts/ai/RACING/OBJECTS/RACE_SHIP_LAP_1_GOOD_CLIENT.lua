function onStartup(self)

    local HeroShip = self:GetObjectsInGroup{ group = "GoodShip"}.objects

    for i = 1, table.maxn (HeroShip) do
        if HeroShip[i]:GetLOT().objtemplate == 9529 then
            HeroShip[i]:PreloadAnimation{animationID = "lap_01", respondObjID = self}
            break
        elseif HeroShip[i]:GetLOT().objtemplate == 10002 then
            HeroShip[i]:PreloadAnimation{animationID = "lap_01", respondObjID = self}
            break
        end
    end

end

function onCollisionPhantom(self, msg)

	local player = msg.objectID
    local HeroShip = self:GetObjectsInGroup{ group = "GoodShip"}.objects

	if player:GetID() == GAMEOBJ:GetControlledID():GetID() then
        for i = 1, table.maxn (HeroShip) do
            if HeroShip[i]:GetLOT().objtemplate == 9529 then
                local Ship = HeroShip[i]
                Ship:PlayAnimation{animationID = "lap_01"}
                Ship:SetOffscreenAnimation{bAnimateOffscreen = true}
                break
            elseif HeroShip[i]:GetLOT().objtemplate == 10002 then
                local Ship = HeroShip[i]
                Ship:PlayAnimation{animationID = "lap_01"}
                Ship:SetOffscreenAnimation{bAnimateOffscreen = true}
                break
            end
        end
        GAMEOBJ:DeleteObject(self)
    end

end
