require('o_mis')

local sections = {} -- Creating empty group to name objects
local spawns = {} -- Our road spawn table
local LoadIndex = 1 -- Updates to show where in the table we are
local PlaceIndex = 1 -- Updates to show when an object in the table has been moved
local UpIndex = 0 -- Updates to show when an object can no longer be raised
local UpIncrement = 2.885 -- How high an object gets raised

function onStartup(self, msg)

    self:AddObjectToGroup{ group = "MR_Control" } -- We need to add this to a group to send messages to it

    for i=1, 500 do -- Easier than typing 6100 x amount of times
        spawns[i] = 6100
    end

end

function onUse(self, msg)

    local roads = self:GetObjectsInGroup{ group = "MR_Roads" }.objects

    for i = 1, table.maxn (roads) do 
        roads[i]:NotifyObject{name = "NoPlace", ObjIDSender = self}
    end

    if LoadIndex <= #spawns and LoadIndex <= PlaceIndex then
        LoadIndex = LoadIndex + 1
        if LoadIndex <= UpIndex then
            UpIndex = LoadIndex -- So the object can't be raised
        end
        RESMGR:LoadObject {objectTemplate = spawns[LoadIndex],
                           x = self:GetPosition{}.pos.x,
                           y = self:GetPosition{}.pos.y,
                           z = self:GetPosition{}.pos.z,
                           owner = self}
    end

    local friends = self:GetObjectsInGroup{ group = "MR_Control" }.objects

    for i = 1, table.maxn (friends) do 
        if friends[i]:GetLOT().objtemplate ~= 6104 then
            friends[i]:NotifyObject{name = "cancel", ObjIDSender = self}
        end
    end

end

function Deletespawns()

    for i = 1, #sections do 
        GAMEOBJ:DeleteObject(sections[i])
    end

    sections = {}
    PlaceIndex = 1
    LoadIndex = 1
    UpIndex = 0

end

function onNotifyObject(self, msg)

    if msg.name == "placed" then
        if #spawns >= LoadIndex and LoadIndex > PlaceIndex then
            sections[#sections]:SetPosition{pos={x = msg.ObjIDSender:GetPosition{}.pos.x, y = sections[#sections]:GetPosition{}.pos.y, z = msg.ObjIDSender:GetPosition{}.pos.z}}

            PlaceIndex = PlaceIndex + 1

            sections[#sections]:NotifyObject{name = "Straight", ObjIDSender = self}
        end

    elseif msg.name == "rotate" then
        if #spawns >= LoadIndex and LoadIndex > UpIndex and sections[#sections] then
            if sections[#sections]:GetRotation{}.y == 1 then
                sections[#sections]:SetRotation{x=0, y=0.707, z=0, w=0.707}
            elseif sections[#sections]:GetRotation{}.y > 0.7 and sections[#sections]:GetRotation{}.y < 0.8 then
                sections[#sections]:SetRotation{x=0, y=0, z=0, w=1}
            elseif sections[#sections]:GetRotation{}.y == 0 then
                sections[#sections]:SetRotation{x=0, y=-0.707, z=0, w=0.707}
            elseif sections[#sections]:GetRotation{}.y < -0.7 and sections[#sections]:GetRotation{}.y > -0.8 then
                sections[#sections]:SetRotation{x=0, y=1, z=0, w=0}
            end
        end

    elseif msg.name == "counterrotate" then
        if #spawns >= LoadIndex and LoadIndex > UpIndex and sections[#sections] then
            if sections[#sections]:GetRotation{}.y == 1 then
                sections[#sections]:SetRotation{x=0, y=-0.707, z=0, w=0.707}
            elseif sections[#sections]:GetRotation{}.y > 0.7 and sections[#sections]:GetRotation{}.y < 0.8 then
                sections[#sections]:SetRotation{x=0, y=1, z=0, w=0}
            elseif sections[#sections]:GetRotation{}.y == 0 then
                sections[#sections]:SetRotation{x=0, y=0.707, z=0, w=0.707}
            elseif sections[#sections]:GetRotation{}.y < -0.7 and sections[#sections]:GetRotation{}.y > -0.8 then
                sections[#sections]:SetRotation{x=0, y=0, z=0, w=1}
            end
        end

    elseif msg.name == "up" then

        if #spawns >= LoadIndex and LoadIndex > UpIndex and sections[#sections] then
            if sections[#sections]:GetPosition{}.pos.y < msg.ObjIDSender:GetPosition{}.pos.y + UpIncrement * 5 then
                sections[#sections]:SetPosition{pos = {x=sections[#sections]:GetPosition{}.pos.x, y=sections[#sections]:GetPosition{}.pos.y + UpIncrement, z=sections[#sections]:GetPosition{}.pos.z}}
            end
            if sections[#sections]:GetPosition{}.pos.y > msg.ObjIDSender:GetPosition{}.pos.y + UpIncrement * 5 then
                sections[#sections]:SetPosition{pos = {x=sections[#sections]:GetPosition{}.pos.x, y=msg.ObjIDSender:GetPosition{}.pos.y, z=sections[#sections]:GetPosition{}.pos.z}}
            end
        end

    elseif msg.name == "label" then
        if LoadIndex > PlaceIndex then
            sections[#sections + 1] = msg.ObjIDSender
        end

    elseif msg.name == "cancel" then
        if PlaceIndex < LoadIndex then

            for i=LoadIndex - 1, #sections do
                if sections[i] then
                    GAMEOBJ:DeleteObject(sections[i])
                end
            end

            LoadIndex = LoadIndex - 1
        end

        if LoadIndex > UpIndex then
            UpIndex = LoadIndex
        end

    elseif msg.name == "reset" then
        Deletespawns()
    end

end
