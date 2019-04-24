require('o_mis')

local sections = {}
local currentPlaceIndex = 1
local spawns = {5851,5851,5851,5851,5851,5851,5851,5851,5851,5851,5851}
local offset = 0
local currentLoadIndex = 1


function onUse(self, msg)

    currentPlaceIndex = 1
    currentLoadIndex = 1
    Deletespawns()
    Loadnextspawn()

    local friends = self:GetObjectsInGroup{ group = "GP_Control" }.objects
    for i = 1, table.maxn (friends) do 
        if friends[i]:GetLOT().objtemplate == 5900 then
            friends[i]:NotifyObject{name = "bouncecancel", ObjIDSender = self}

        elseif friends[i]:GetLOT().objtemplate == 5901 then
            friends[i]:NotifyObject{name = "climbercancel", ObjIDSender = self}

        elseif friends[i]:GetLOT().objtemplate == 5925 then
            friends[i]:NotifyObject{name = "misccancel", ObjIDSender = self}

        elseif friends[i]:GetLOT().objtemplate == 5926 then
            friends[i]:NotifyObject{name = "trapcancel", ObjIDSender = self}

        elseif friends[i]:GetLOT().objtemplate == 5927 then
            friends[i]:NotifyObject{name = "blockercancel", ObjIDSender = self}

        elseif friends[i]:GetLOT().objtemplate == 5855 then
            friends[i]:NotifyObject{name = "allcancel", ObjIDSender = self}
        end
    end

end

function Loadnextspawn()

    if currentLoadIndex <= #spawns then
        RESMGR:LoadObject {objectTemplate = spawns[currentLoadIndex],
                           x = -125.31,
                           y = 185.51,
                           z = -470 + offset,
                           owner = self}
        offset = offset + 5
        currentLoadIndex = currentLoadIndex + 1

    end

end

function Deletespawns()

    for i = 1, #sections do 
        GAMEOBJ:DeleteObject(sections[i])
    end
    sections = {}
    offset = 0
    currentPlaceIndex = 1
    currentLoadIndex = 1

end

function onNotifyObject(self, msg)

    if msg.name == "padloaded" then
        sections[#sections + 1] = msg.ObjIDSender
        Loadnextspawn()

    elseif msg.name == "Selected" then
        if #sections >= currentPlaceIndex then

            sections[currentPlaceIndex]:SetPosition{pos=msg.ObjIDSender:GetPosition{}.pos}
--            sections[currentPlaceIndex]:StopFXEffect{name = "hot"}
            currentPlaceIndex = currentPlaceIndex + 1

--            if currentPlaceIndex <= #sections then
--                sections[currentPlaceIndex]:PlayFXEffect{name = "hot", effectType = "select"}
--            end
        end

    elseif msg.name == "Trashed" then
        if #sections >= currentPlaceIndex then
            sections[currentPlaceIndex]:SetPosition{pos={x=msg.ObjIDSender:GetPosition{}.pos.x - 200, y=msg.ObjIDSender:GetPosition{}.pos.y, z=msg.ObjIDSender:GetPosition{}.pos.z}}
            currentPlaceIndex = currentPlaceIndex + 1
        end

    elseif msg.name == "padcancel" then
        currentPlaceIndex = 12
        currentLoadIndex = 12

    elseif msg.name == "reset" then
        Deletespawns()
    end

end
