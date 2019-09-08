require('o_mis')

local roadsections = {}
local currentRoadPlaceIndex = 1
local roadspawns = {}
local offsetx = 0
local LoadIndex = 1

local placeposx = ""
local placeposy = ""
local placeposz = ""

local spacing = 25.6
local gridsize = 10
local gridBool = true
local vertTick = 1

function onStartup(self, msg)
    
    placeposx = self:GetPosition{}.pos.x
    placeposy = self:GetPosition{}.pos.y
    placeposz = self:GetPosition{}.pos.z

    self:AddObjectToGroup{ group = "MR_Control" }

    for i=1, gridsize * gridsize do
        roadspawns[i] = 6087
    end
end

function onUse(self, msg)

    local friends = self:GetObjectsInGroup{ group = "MR_Control"}.objects
        for i = 1, table.maxn (friends) do
            friends[i]:NotifyObject{name = "reset", ObjIDSender = self}
        end

    local roads = self:GetObjectsInGroup{ group = "MR_Roads" }.objects
        for i = 1, table.maxn (roads) do 
            roads[i]:NotifyObject{name = "delete", ObjIDSender = self}
        end

    Deleteroadspawns()
    currentRoadPlaceIndex = 1
    LoadIndex = 1

    gridBool = true
    gridsize = 10
    vertTick = 1
    offsetx = 0
    gridBool = false    
    LoadRoadnextspawn()

end

function LoadRoadnextspawn()

    if LoadIndex <= #roadspawns  then
        if gridBool then
            RESMGR:LoadObject {objectTemplate = roadspawns[LoadIndex],
                               x = placeposx + offsetx,
                               y = placeposy,
                               z = placeposz + vertTick * spacing,
                               owner = self}
            offsetx = offsetx + spacing

        else
            RESMGR:LoadObject {objectTemplate = roadspawns[LoadIndex],
                                x = placeposx + offsetx,
                                y = placeposy,
                                z = placeposz + vertTick * spacing,
                                owner = self}
            offsetx = offsetx - spacing
        end

        if LoadIndex == gridsize * vertTick then
            
            vertTick = vertTick + 1
            if gridBool then 
                gridBool = false
            else  
                gridBoll = true 
            end
            offsetx = 0
        end    

    LoadIndex = LoadIndex + 1

    end

end

function Deleteroadspawns()

    for i = 1, #roadsections do 
        GAMEOBJ:DeleteObject(roadsections[i])
    end

    roadsections = {}
    roadoffset = 0
    currentRoadPlaceIndex = 1
    LoadIndex = 1

end

function onNotifyObject(self, msg)

    if msg.name == "roadloaded" then
        roadsections[#roadsections + 1] = msg.ObjIDSender
        LoadRoadnextspawn()

    elseif msg.name == "buttonreset" then

    local friends = self:GetObjectsInGroup{ group = "MR_Control"}.objects
        for i = 1, table.maxn (friends) do
            friends[i]:NotifyObject{name = "reset", ObjIDSender = self}
        end

    local roads = self:GetObjectsInGroup{ group = "MR_Roads" }.objects
        for i = 1, table.maxn (roads) do 
            roads[i]:NotifyObject{name = "delete", ObjIDSender = self}
        end

        Deleteroadspawns()
        currentRoadPlaceIndex = 1
        LoadIndex = 1
        gridBool = true
        gridsize = 10
        vertTick = 1
        offsetx = 0
        gridBool = false    
        LoadRoadnextspawn()
    end

end
