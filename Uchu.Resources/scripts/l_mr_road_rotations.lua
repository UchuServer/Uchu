require('o_mis')

local straight = 0
local turn = 0
local grass = 0
local straighttick = 0
local turntick = 0
local grasstick = 0
local stuff = 0
local UpIncrement = 3.04

function onStartup(self, msg)

    self:AddObjectToGroup{ group = "MR_Roads" }

    local friends = self:GetObjectsInGroup{ group = "MR_Control"}.objects

    for i = 1, table.maxn (friends) do
        friends[i]:NotifyObject{name = "label", ObjIDSender = self}
    end

end

function onUse(self)

    local friends = self:GetObjectsInGroup{ group = "MR_Control" }.objects

    for i = 1, table.maxn (friends) do 
        friends[i]:NotifyObject{name = "removed", ObjIDSender = self}
    end

    for i = 0, (straighttick) do

        if getObjectByName(self, "Placer" .. i) then
            if GAMEOBJ:DeleteObject(getObjectByName(self, "Placer" .. i)) then
                GAMEOBJ:DeleteObject(getObjectByName(self, "Placer" .. i))
            end
        end
    end

    for i = 0, (turntick) do

        if getObjectByName(self, "TurnPlacer" .. i) then
            if GAMEOBJ:DeleteObject(getObjectByName(self, "TurnPlacer" .. i)) then
                GAMEOBJ:DeleteObject(getObjectByName(self, "TurnPlacer" .. i))
            end
        end
    end

    for i = 0, (grasstick) do

        if getObjectByName(self, "GrassPlacer" .. i) then
            if GAMEOBJ:DeleteObject(getObjectByName(self, "GrassPlacer" .. i)) then
                GAMEOBJ:DeleteObject(getObjectByName(self, "GrassPlacer" .. i))
            end
        end
    end

    GAMEOBJ:DeleteObject(self)

end

function onNotifyObject(self, msg)

    if msg.name == "NorthStraight" then
        RESMGR:LoadObject {objectTemplate = 6220,
                           x = self:GetPosition{}.pos.x + 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z + 5,
                           rw = 0,
                           rx = 0,
                           ry = 1,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6220,
                           x = self:GetPosition{}.pos.x - 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z + 5,
                           rw = 0,
                           rx = 0,
                           ry = 1,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6220,
                           x = self:GetPosition{}.pos.x,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z,
                           rw = 0,
                           rx = 0,
                           ry = 1,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6220,
                           x = self:GetPosition{}.pos.x + 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z - 5,
                           rw = 0,
                           rx = 0,
                           ry = 1,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6220,
                           x = self:GetPosition{}.pos.x - 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z - 5,
                           rw = 0,
                           rx = 0,
                           ry = 1,
                           rz = 0,
                           owner = self}
        straight = 1
        turn = 0
        grass = 0

    elseif msg.name == "EastStraight" then
        RESMGR:LoadObject {objectTemplate = 6220,
                           x = self:GetPosition{}.pos.x + 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z - 5,
                           rw = 0.707,
                           rx = 0,
                           ry = 0.707,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6220,
                           x = self:GetPosition{}.pos.x + 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z + 5,
                           rw = 0.707,
                           rx = 0,
                           ry = 0.707,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6220,
                           x = self:GetPosition{}.pos.x,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z,
                           rw = 0.707,
                           rx = 0,
                           ry = 0.707,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6220,
                           x = self:GetPosition{}.pos.x - 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z - 5,
                           rw = 0.707,
                           rx = 0,
                           ry = 0.707,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6220,
                           x = self:GetPosition{}.pos.x - 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z + 5,
                           rw = 0.707,
                           rx = 0,
                           ry = 0.707,
                           rz = 0,
                           owner = self}
        straight = 1
        turn = 0
        grass = 0

    elseif msg.name == "SouthStraight" then
        RESMGR:LoadObject {objectTemplate = 6220,
                           x = self:GetPosition{}.pos.x + 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z + 5,
                           rw = 0,
                           rx = 0,
                           ry = 1,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6220,
                           x = self:GetPosition{}.pos.x - 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z + 5,
                           rw = 0,
                           rx = 0,
                           ry = 1,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6220,
                           x = self:GetPosition{}.pos.x,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z,
                           rw = 0,
                           rx = 0,
                           ry = 1,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6220,
                           x = self:GetPosition{}.pos.x + 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z - 5,
                           rw = 0,
                           rx = 0,
                           ry = 1,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6220,
                           x = self:GetPosition{}.pos.x - 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z - 5,
                           rw = 0,
                           rx = 0,
                           ry = 1,
                           rz = 0,
                           owner = self}
        straight = 1
        turn = 0
        grass = 0

    elseif msg.name == "WestStraight" then
        RESMGR:LoadObject {objectTemplate = 6220,
                           x = self:GetPosition{}.pos.x + 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z - 5,
                           rw = 0.707,
                           rx = 0,
                           ry = 0.707,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6220,
                           x = self:GetPosition{}.pos.x + 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z + 5,
                           rw = 0.707,
                           rx = 0,
                           ry = 0.707,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6220,
                           x = self:GetPosition{}.pos.x,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z,
                           rw = 0.707,
                           rx = 0,
                           ry = 0.707,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6220,
                           x = self:GetPosition{}.pos.x - 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z - 5,
                           rw = 0.707,
                           rx = 0,
                           ry = 0.707,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6220,
                           x = self:GetPosition{}.pos.x - 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z + 5,
                           rw = 0.707,
                           rx = 0,
                           ry = 0.707,
                           rz = 0,
                           owner = self}
        straight = 1
        turn = 0
        grass = 0

    elseif msg.name == "NorthTurn" then
        RESMGR:LoadObject {objectTemplate = 6249,
                           x = self:GetPosition{}.pos.x,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z,
                           rw = 0,
                           rx = 0,
                           ry = 1,
                           rz = 0,
                           owner = self}
        straight = 0
        turn = 1
        grass = 0

    elseif msg.name == "EastTurn" then
        RESMGR:LoadObject {objectTemplate = 6249,
                           x = self:GetPosition{}.pos.x,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z,
                           rw = 0.707,
                           rx = 0,
                           ry = 0.707,
                           rz = 0,
                           owner = self}
        straight = 0
        turn = 1
        grass = 0

    elseif msg.name == "SouthTurn" then
        RESMGR:LoadObject {objectTemplate = 6249,
                           x = self:GetPosition{}.pos.x,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z,
                           rw = 0,
                           rx = 0,
                           ry = 1,
                           rz = 0,
                           owner = self}
        straight = 0
        turn = 1
        grass = 0

    elseif msg.name == "WestTurn" then
        RESMGR:LoadObject {objectTemplate = 6249,
                           x = self:GetPosition{}.pos.x,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z,
                           rw = 0.707,
                           rx = 0,
                           ry = 0.707,
                           rz = 0,
                           owner = self}
        straight = 0
        turn = 1
        grass = 0

    elseif msg.name == "NorthGrass" then
        RESMGR:LoadObject {objectTemplate = 6250,
                           x = self:GetPosition{}.pos.x + 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z + 5,
                           rw = 0,
                           rx = 0,
                           ry = 1,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6250,
                           x = self:GetPosition{}.pos.x - 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z + 5,
                           rw = 0,
                           rx = 0,
                           ry = 1,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6250,
                           x = self:GetPosition{}.pos.x,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z,
                           rw = 0,
                           rx = 0,
                           ry = 1,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6250,
                           x = self:GetPosition{}.pos.x + 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z - 5,
                           rw = 0,
                           rx = 0,
                           ry = 1,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6250,
                           x = self:GetPosition{}.pos.x - 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z - 5,
                           rw = 0,
                           rx = 0,
                           ry = 1,
                           rz = 0,
                           owner = self}
        straight = 0
        turn = 0
        grass = 1

    elseif msg.name == "EastGrass" then
        RESMGR:LoadObject {objectTemplate = 6250,
                           x = self:GetPosition{}.pos.x + 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z - 5,
                           rw = 0.707,
                           rx = 0,
                           ry = 0.707,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6250,
                           x = self:GetPosition{}.pos.x + 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z + 5,
                           rw = 0.707,
                           rx = 0,
                           ry = 0.707,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6250,
                           x = self:GetPosition{}.pos.x,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z,
                           rw = 0.707,
                           rx = 0,
                           ry = 0.707,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6250,
                           x = self:GetPosition{}.pos.x - 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z - 5,
                           rw = 0.707,
                           rx = 0,
                           ry = 0.707,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6250,
                           x = self:GetPosition{}.pos.x - 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z + 5,
                           rw = 0.707,
                           rx = 0,
                           ry = 0.707,
                           rz = 0,
                           owner = self}
        straight = 0
        turn = 0
        grass = 1

    elseif msg.name == "SouthGrass" then
        RESMGR:LoadObject {objectTemplate = 6250,
                           x = self:GetPosition{}.pos.x + 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z + 5,
                           rw = 0,
                           rx = 0,
                           ry = 1,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6250,
                           x = self:GetPosition{}.pos.x - 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z + 5,
                           rw = 0,
                           rx = 0,
                           ry = 1,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6250,
                           x = self:GetPosition{}.pos.x,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z,
                           rw = 0,
                           rx = 0,
                           ry = 1,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6250,
                           x = self:GetPosition{}.pos.x + 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z - 5,
                           rw = 0,
                           rx = 0,
                           ry = 1,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6250,
                           x = self:GetPosition{}.pos.x - 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z - 5,
                           rw = 0,
                           rx = 0,
                           ry = 1,
                           rz = 0,
                           owner = self}
        straight = 0
        turn = 0
        grass = 1

    elseif msg.name == "WestGrass" then
        RESMGR:LoadObject {objectTemplate = 6250,
                           x = self:GetPosition{}.pos.x + 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z - 5,
                           rw = 0.707,
                           rx = 0,
                           ry = 0.707,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6250,
                           x = self:GetPosition{}.pos.x + 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z + 5,
                           rw = 0.707,
                           rx = 0,
                           ry = 0.707,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6250,
                           x = self:GetPosition{}.pos.x,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z,
                           rw = 0.707,
                           rx = 0,
                           ry = 0.707,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6250,
                           x = self:GetPosition{}.pos.x - 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z - 5,
                           rw = 0.707,
                           rx = 0,
                           ry = 0.707,
                           rz = 0,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6250,
                           x = self:GetPosition{}.pos.x - 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z + 5,
                           rw = 0.707,
                           rx = 0,
                           ry = 0.707,
                           rz = 0,
                           owner = self}
        straight = 0
        turn = 0
        grass = 1

    elseif msg.name == "NoPlace" then
        straight = 0
        turn = 0
        grass = 0

    elseif msg.name == "delete" then
        for i = 0, (straighttick) do
            if getObjectByName(self, "Placer" .. i) then
                GAMEOBJ:DeleteObject(getObjectByName(self, "Placer" .. i))
            end
        end

        for i = 0, (turntick) do
            if getObjectByName(self, "TurnPlacer" .. i) then
                GAMEOBJ:DeleteObject(getObjectByName(self, "TurnPlacer" .. i))
            end
        end

        for i = 0, (grasstick) do
            if getObjectByName(self, "GrassPlacer" .. i) then
                GAMEOBJ:DeleteObject(getObjectByName(self, "GrassPlacer" .. i))
            end
        end

    elseif msg.name == "up" then

        if straight > turn and straight > grass then
            for i = (straighttick - 5), (straighttick) do
                if getObjectByName(self, "Placer" .. i) then
                    if getObjectByName(self, "Placer" .. i):GetPosition{}.pos.y < msg.ObjIDSender:GetPosition{}.pos.y + UpIncrement * 4 then
                        getObjectByName(self, "Placer" .. i):SetPosition{pos = {x = getObjectByName(self, "Placer" .. i):GetPosition{}.pos.x, y = getObjectByName(self, "Placer" .. i):GetPosition{}.pos.y + UpIncrement, z = getObjectByName(self, "Placer" .. i):GetPosition{}.pos.z}}

                    elseif getObjectByName(self, "Placer" .. i):GetPosition{}.pos.y > msg.ObjIDSender:GetPosition{}.pos.y + UpIncrement * 4 then
                        getObjectByName(self, "Placer" .. i):SetPosition{pos = {x = getObjectByName(self, "Placer" .. i):GetPosition{}.pos.x, y = msg.ObjIDSender:GetPosition{}.pos.y, z = getObjectByName(self, "Placer" .. i):GetPosition{}.pos.z}}
                    end
                end
            end
        end

        if turn > straight and turn > grass then
            if getObjectByName(self, "TurnPlacer" .. turntick - 1) then
                if getObjectByName(self, "TurnPlacer" .. turntick - 1):GetPosition{}.pos.y < msg.ObjIDSender:GetPosition{}.pos.y + UpIncrement * 4 then
                    getObjectByName(self, "TurnPlacer" .. turntick - 1):SetPosition{pos = {x = getObjectByName(self, "TurnPlacer" .. turntick - 1):GetPosition{}.pos.x, y = getObjectByName(self, "TurnPlacer" .. turntick - 1):GetPosition{}.pos.y + UpIncrement, z = getObjectByName(self, "TurnPlacer" .. turntick - 1):GetPosition{}.pos.z}}

                elseif getObjectByName(self, "TurnPlacer" .. turntick - 1):GetPosition{}.pos.y > msg.ObjIDSender:GetPosition{}.pos.y + UpIncrement * 4 then
                    getObjectByName(self, "TurnPlacer" .. turntick - 1):SetPosition{pos = {x = getObjectByName(self, "TurnPlacer" .. turntick - 1):GetPosition{}.pos.x, y = msg.ObjIDSender:GetPosition{}.pos.y, z = getObjectByName(self, "TurnPlacer" .. turntick - 1):GetPosition{}.pos.z}}
                end
            end
        end

        if grass > straight and grass > turn then
            if getObjectByName(self, "GrassPlacer" .. grasstick - 1) then
                if getObjectByName(self, "GrassPlacer" .. grasstick - 1):GetPosition{}.pos.y < msg.ObjIDSender:GetPosition{}.pos.y + UpIncrement * 4 then
                    getObjectByName(self, "GrassPlacer" .. grasstick - 1):SetPosition{pos = {x = getObjectByName(self, "GrassPlacer" .. grasstick - 1):GetPosition{}.pos.x, y = getObjectByName(self, "GrassPlacer" .. grasstick - 1):GetPosition{}.pos.y + UpIncrement, z = getObjectByName(self, "GrassPlacer" .. grasstick - 1):GetPosition{}.pos.z}}

                elseif getObjectByName(self, "GrassPlacer" .. i):GetPosition{}.pos.y > msg.ObjIDSender:GetPosition{}.pos.y + UpIncrement * 4 then
                    getObjectByName(self, "GrassPlacer" .. i):SetPosition{pos = {x = getObjectByName(self, "GrassPlacer" .. grasstick - 1):GetPosition{}.pos.x, y = msg.ObjIDSender:GetPosition{}.pos.y, z = getObjectByName(self, "GrassPlacer" .. grasstick - 1):GetPosition{}.pos.z}}
                end
            end
        end
    end

end

function onChildLoaded(self, msg)

	if (msg.templateID == 6220) then
        storeObjectByName(self, "Placer" .. straighttick, msg.childID)
        straighttick = straighttick + 1
    end

	if (msg.templateID == 6249) then
        storeObjectByName(self, "TurnPlacer" .. turntick, msg.childID)
        turntick = turntick + 1
    end

	if (msg.templateID == 6250) then
        storeObjectByName(self, "GrassPlacer" .. grasstick, msg.childID)
        grasstick = grasstick + 1
    end
end
