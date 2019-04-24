require('o_mis')

CONSTANTS = {}
CONSTANTS["BlueTele"] = 5895
CONSTANTS["OrangeTele"] = 5896

local miscsections = {}
local currentMiscPlaceIndex = 1
local miscspawns = {5884,5884,5884,5895,5896}
local miscoffset = 0
local currentMiscLoadIndex = 1

function onUse(self, msg)

    currentMiscPlaceIndex = 1
    currentMiscLoadIndex = 1
    Deletemiscspawns()
    Loadnextmiscspawn()

    local friends = self:GetObjectsInGroup{ group = "GP_Control" }.objects
    for i = 1, table.maxn (friends) do 
        if friends[i]:GetLOT().objtemplate == 5899 then
            friends[i]:NotifyObject{name = "padcancel", ObjIDSender = self}

        elseif friends[i]:GetLOT().objtemplate == 5900 then
            friends[i]:NotifyObject{name = "bouncecancel", ObjIDSender = self}

        elseif friends[i]:GetLOT().objtemplate == 5901 then
            friends[i]:NotifyObject{name = "climbercancel", ObjIDSender = self}

        elseif friends[i]:GetLOT().objtemplate == 5926 then
            friends[i]:NotifyObject{name = "trapcancel", ObjIDSender = self}

        elseif friends[i]:GetLOT().objtemplate == 5927 then
            friends[i]:NotifyObject{name = "blockercancel", ObjIDSender = self}

        elseif friends[i]:GetLOT().objtemplate == 5855 then
            friends[i]:NotifyObject{name = "allcancel", ObjIDSender = self}

        end
    end

end

function Loadnextmiscspawn()

    if currentMiscLoadIndex <= #miscspawns then

        RESMGR:LoadObject {objectTemplate = miscspawns[currentMiscLoadIndex],
                           x = -97.31,
                           y = 185.38,
                           z = -470 + miscoffset,
                           owner = self}
        miscoffset = miscoffset + 15
        currentMiscLoadIndex = currentMiscLoadIndex + 1
    end

end

function Deletemiscspawns()

    for i = 1, #miscsections do 
        GAMEOBJ:DeleteObject(miscsections[i])
    end
    miscsections = {}
    miscoffset = 0
    currentMiscPlaceIndex = 1
    currentMiscLoadIndex = 1

end

function onNotifyObject(self, msg)

    if msg.name == "miscloaded" then
        miscsections[#miscsections + 1] = msg.ObjIDSender
        Loadnextmiscspawn()

    elseif msg.name == "Selected" then
        if #miscsections >= currentMiscPlaceIndex then

            miscsections[currentMiscPlaceIndex]:SetPosition{pos=msg.ObjIDSender:GetPosition{}.pos}
--            miscsections[currentMiscPlaceIndex]:StopFXEffect{name = "hot"}
            currentMiscPlaceIndex = currentMiscPlaceIndex + 1

--            if currentMiscPlaceIndex <= #miscsections then
--                miscsections[currentMiscPlaceIndex]:PlayFXEffect{name = "hot", effectType = "select"}
--            end
        end

    elseif msg.name == "Trashed" then
        if #miscsections >= currentMiscPlaceIndex then
            miscsections[currentMiscPlaceIndex]:SetPosition{pos={x=msg.ObjIDSender:GetPosition{}.pos.x - 200, y=msg.ObjIDSender:GetPosition{}.pos.y, z=msg.ObjIDSender:GetPosition{}.pos.z}}
            currentMiscPlaceIndex = currentMiscPlaceIndex + 1
        end

    elseif msg.name == "misccancel" then
        currentMiscPlaceIndex = 6
        currentMiscLoadIndex = 6

    elseif msg.name == "reset" then
        Deletemiscspawns()
    end
        
end
