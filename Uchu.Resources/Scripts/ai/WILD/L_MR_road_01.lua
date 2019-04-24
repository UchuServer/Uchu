require('o_mis')

local straight = 0
local turn = 0
local grass = 0
local threeway = 0
local fourway = 0

local straighttick = 0
local turntick = 0
local grasstick = 0
local threetick = 0
local fourtick = 0
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

    for i = 0, (threetick) do

        if getObjectByName(self, "ThreePlacer" .. i) then
            if GAMEOBJ:DeleteObject(getObjectByName(self, "ThreePlacer" .. i)) then
                GAMEOBJ:DeleteObject(getObjectByName(self, "ThreePlacer" .. i))
            end
        end
    end

    for i = 0, (fourtick) do

        if getObjectByName(self, "FourPlacer" .. i) then
            if GAMEOBJ:DeleteObject(getObjectByName(self, "FourPlacer" .. i)) then
                GAMEOBJ:DeleteObject(getObjectByName(self, "FourPlacer" .. i))
            end
        end
    end

    GAMEOBJ:DeleteObject(self)

end

function onNotifyObject(self, msg)

    if msg.name == "Straight" then
        RESMGR:LoadObject {objectTemplate = 6220,
                           x = self:GetPosition{}.pos.x + 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z + 5,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6220,
                           x = self:GetPosition{}.pos.x - 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z + 5,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6220,
                           x = self:GetPosition{}.pos.x,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6220,
                           x = self:GetPosition{}.pos.x + 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z - 5,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6220,
                           x = self:GetPosition{}.pos.x - 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z - 5,
                           owner = self}
        straight = 1
        turn = 0
        grass = 0
        threeway = 0
        fourway = 0

    elseif msg.name == "Turn" then
        RESMGR:LoadObject {objectTemplate = 6249,
                           x = self:GetPosition{}.pos.x,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z,
                           owner = self}
        straight = 0
        turn = 1
        grass = 0
        threeway = 0
        fourway = 0

    elseif msg.name == "Grass" then
        RESMGR:LoadObject {objectTemplate = 6250,
                           x = self:GetPosition{}.pos.x + 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z + 5,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6250,
                           x = self:GetPosition{}.pos.x - 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z + 5,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6250,
                           x = self:GetPosition{}.pos.x,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6250,
                           x = self:GetPosition{}.pos.x + 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z - 5,
                           owner = self}
        RESMGR:LoadObject {objectTemplate = 6250,
                           x = self:GetPosition{}.pos.x - 5,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z - 5,
                           owner = self}
        straight = 0
        turn = 0
        grass = 1
        threeway = 0
        fourway = 0

    elseif msg.name == "ThreeWay" then
        RESMGR:LoadObject {objectTemplate = 6309,
                           x = self:GetPosition{}.pos.x,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z,
                           owner = self}
        straight = 0
        turn = 0
        grass = 0
        threeway = 1
        fourway = 0

    elseif msg.name == "FourWay" then
        RESMGR:LoadObject {objectTemplate = 6310,
                           x = self:GetPosition{}.pos.x,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z,
                           owner = self}
        straight = 0
        turn = 0
        grass = 0
        threeway = 0
        fourway = 1

    elseif msg.name == "NoPlace" then
        straight = 0
        turn = 0
        grass = 0
        threeway = 0
        fourway = 0

    elseif msg.name == "delete" then
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

        for i = 0, (threetick) do

            if getObjectByName(self, "ThreePlacer" .. i) then
                if GAMEOBJ:DeleteObject(getObjectByName(self, "ThreePlacer" .. i)) then
                    GAMEOBJ:DeleteObject(getObjectByName(self, "ThreePlacer" .. i))
                end
            end
        end

        for i = 0, (fourtick) do

            if getObjectByName(self, "FourPlacer" .. i) then
                if GAMEOBJ:DeleteObject(getObjectByName(self, "FourPlacer" .. i)) then
                    GAMEOBJ:DeleteObject(getObjectByName(self, "FourPlacer" .. i))
                end
            end
        end

        straighttick = 0
        turntick = 0
        grasstick = 0
        threetick = 0
        fourtick = 0

    elseif msg.name == "up" then

        if straight > turn and straight > grass and straight > threeway and straight > fourway then
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

        if turn > straight and turn > grass and turn > threeway and turn > fourway then
            if getObjectByName(self, "TurnPlacer" .. turntick - 1) then
                if getObjectByName(self, "TurnPlacer" .. turntick - 1):GetPosition{}.pos.y < msg.ObjIDSender:GetPosition{}.pos.y + UpIncrement * 4 then
                    getObjectByName(self, "TurnPlacer" .. turntick - 1):SetPosition{pos = {x = getObjectByName(self, "TurnPlacer" .. turntick - 1):GetPosition{}.pos.x, y = getObjectByName(self, "TurnPlacer" .. turntick - 1):GetPosition{}.pos.y + UpIncrement, z = getObjectByName(self, "TurnPlacer" .. turntick - 1):GetPosition{}.pos.z}}

                elseif getObjectByName(self, "TurnPlacer" .. turntick - 1):GetPosition{}.pos.y > msg.ObjIDSender:GetPosition{}.pos.y + UpIncrement * 4 then
                    getObjectByName(self, "TurnPlacer" .. turntick - 1):SetPosition{pos = {x = getObjectByName(self, "TurnPlacer" .. turntick - 1):GetPosition{}.pos.x, y = msg.ObjIDSender:GetPosition{}.pos.y, z = getObjectByName(self, "TurnPlacer" .. turntick - 1):GetPosition{}.pos.z}}
                end
            end
        end

        if grass > straight and grass > turn and grass > threeway and grass > fourway then
            for i = (grasstick - 5), (grasstick) do
                if getObjectByName(self, "GrassPlacer" .. i) then
                    if getObjectByName(self, "GrassPlacer" .. i):GetPosition{}.pos.y < msg.ObjIDSender:GetPosition{}.pos.y + UpIncrement * 4 then
                        getObjectByName(self, "GrassPlacer" .. i):SetPosition{pos = {x = getObjectByName(self, "GrassPlacer" .. i):GetPosition{}.pos.x, y = getObjectByName(self, "GrassPlacer" .. i):GetPosition{}.pos.y + UpIncrement, z = getObjectByName(self, "GrassPlacer" .. i):GetPosition{}.pos.z}}

                    elseif getObjectByName(self, "GrassPlacer" .. i):GetPosition{}.pos.y > msg.ObjIDSender:GetPosition{}.pos.y + UpIncrement * 4 then
                        getObjectByName(self, "GrassPlacer" .. i):SetPosition{pos = {x = getObjectByName(self, "GrassPlacer" .. i):GetPosition{}.pos.x, y = msg.ObjIDSender:GetPosition{}.pos.y, z = getObjectByName(self, "GrassPlacer" .. i):GetPosition{}.pos.z}}
                    end
                end
            end
        end

        if threeway > straight and threeway > turn and threeway > grass and threeway > fourway then
            if getObjectByName(self, "ThreePlacer" .. threetick - 1) then
            print "Double BOOOYA!"
                if getObjectByName(self, "ThreePlacer" .. threetick - 1):GetPosition{}.pos.y < msg.ObjIDSender:GetPosition{}.pos.y + UpIncrement * 4 then
                    getObjectByName(self, "ThreePlacer" .. threetick - 1):SetPosition{pos = {x = getObjectByName(self, "ThreePlacer" .. threetick - 1):GetPosition{}.pos.x, y = getObjectByName(self, "ThreePlacer" .. threetick - 1):GetPosition{}.pos.y + UpIncrement, z = getObjectByName(self, "ThreePlacer" .. threetick - 1):GetPosition{}.pos.z}}

                elseif getObjectByName(self, "ThreePlacer" .. threetick - 1):GetPosition{}.pos.y > msg.ObjIDSender:GetPosition{}.pos.y + UpIncrement * 4 then
                    print "Down"
                    getObjectByName(self, "ThreePlacer" .. threetick - 1):SetPosition{pos = {x = getObjectByName(self, "ThreePlacer" .. threetick - 1):GetPosition{}.pos.x, y = msg.ObjIDSender:GetPosition{}.pos.y, z = getObjectByName(self, "ThreePlacer" .. threetick - 1):GetPosition{}.pos.z}}
                end
            end
        end

        if fourway > straight and fourway > turn and fourway > grass and fourway > threeway then
            if getObjectByName(self, "FourPlacer" .. fourtick - 1) then
                if getObjectByName(self, "FourPlacer" .. fourtick - 1):GetPosition{}.pos.y < msg.ObjIDSender:GetPosition{}.pos.y + UpIncrement * 4 then
                    getObjectByName(self, "FourPlacer" .. fourtick - 1):SetPosition{pos = {x = getObjectByName(self, "FourPlacer" .. fourtick - 1):GetPosition{}.pos.x, y = getObjectByName(self, "FourPlacer" .. fourtick - 1):GetPosition{}.pos.y + UpIncrement, z = getObjectByName(self, "FourPlacer" .. fourtick - 1):GetPosition{}.pos.z}}

                elseif getObjectByName(self, "FourPlacer" .. fourtick - 1):GetPosition{}.pos.y > msg.ObjIDSender:GetPosition{}.pos.y + UpIncrement * 4 then
                    getObjectByName(self, "FourPlacer" .. fourtick - 1):SetPosition{pos = {x = getObjectByName(self, "FourPlacer" .. fourtick - 1):GetPosition{}.pos.x, y = msg.ObjIDSender:GetPosition{}.pos.y, z = getObjectByName(self, "FourPlacer" .. fourtick - 1):GetPosition{}.pos.z}}
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

	if (msg.templateID == 6309) then
        storeObjectByName(self, "ThreePlacer" .. threetick, msg.childID)
        threetick = threetick + 1
    end

	if (msg.templateID == 6310) then
        storeObjectByName(self, "FourPlacer" .. fourtick, msg.childID)
        fourtick = fourtick + 1
    end

end