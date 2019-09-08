require('o_mis')

local allsections = {}
local currentAllPlaceIndex = 1
local allspawns = {5942,5942,5943,5943,5944,5945,5942,5942}
local alloffset = 0
local currentAllLoadIndex = 1

function onStartup(self, msg)

    self:AddObjectToGroup{ group = "GP_Control" }
    self:AddObjectToGroup{ group = "GP_ALL" }

end

function onUse(self, msg)

    currentAllPlaceIndex = 1
    currentAllLoadIndex = 1
    Deleteallspawns()
    LoadAllnextspawn()

    local friends = self:GetObjectsInGroup{ group = "GP_Control" }.objects
    for i = 1, table.maxn (friends) do 
        if friends[i]:GetLOT().objtemplate == 5899 then
            friends[i]:NotifyObject{name = "padcancel", ObjIDSender = self}

        elseif friends[i]:GetLOT().objtemplate == 5900 then
            friends[i]:NotifyObject{name = "bouncecancel", ObjIDSender = self}

        elseif friends[i]:GetLOT().objtemplate == 5901 then
            friends[i]:NotifyObject{name = "climbercancel", ObjIDSender = self}

        elseif friends[i]:GetLOT().objtemplate == 5925 then
            friends[i]:NotifyObject{name = "misccancel", ObjIDSender = self}

        elseif friends[i]:GetLOT().objtemplate == 5926 then
            friends[i]:NotifyObject{name = "trapcancel", ObjIDSender = self}

        elseif friends[i]:GetLOT().objtemplate == 5855 then
            friends[i]:NotifyObject{name = "blockercancel", ObjIDSender = self}
        end

    end

end

function LoadAllnextspawn()

    if currentAllLoadIndex <= #allspawns then
        RESMGR:LoadObject {objectTemplate = allspawns[currentAllLoadIndex],
                           x = -177 + alloffset,
                           y = 184,
                           z = -482,
                           owner = self}
        alloffset = alloffset + 5
        currentAllLoadIndex = currentAllLoadIndex + 1
    end

end

function Deleteallspawns()

    for i = 1, #allsections do 
        GAMEOBJ:DeleteObject(allsections[i])
    end
    allsections = {}
    alloffset = 0
    currentAllPlaceIndex = 1
    currentAllLoadIndex = 1

end

function onNotifyObject(self, msg)

    if msg.name == "totalloaded" then
        allsections[#allsections + 1] = msg.ObjIDSender
        LoadAllnextspawn()

    elseif msg.name == "Selected" then
        if #allsections >= currentAllPlaceIndex then

            allsections[currentAllPlaceIndex]:SetPosition{pos=msg.ObjIDSender:GetPosition{}.pos}
            currentAllPlaceIndex = currentAllPlaceIndex + 1
        end

    elseif msg.name == "Trashed" then
        if #allsections >= currentAllPlaceIndex then
            allsections[currentAllPlaceIndex]:SetPosition{pos={x=msg.ObjIDSender:GetPosition{}.pos.x - 200, y=msg.ObjIDSender:GetPosition{}.pos.y, z=msg.ObjIDSender:GetPosition{}.pos.z}}
            currentAllPlaceIndex = currentAllPlaceIndex + 1
        end

    elseif msg.name == "allcancel" then
        currentAllPlaceIndex = 9
        currentAllLoadIndex = 9

    elseif msg.name == "reset" then
        Deleteallspawns()
    end

end
