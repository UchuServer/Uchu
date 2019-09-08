require('o_mis')

local blockersections = {}
local currentBlockerPlaceIndex = 1
local blockerspawns = {5913,5913,5913,5913,5913,5928,5928,5928,5928,5928}
local blockeroffset = 0
local currentBlockerLoadIndex = 1


function onUse(self, msg)

    currentBlockerPlaceIndex = 1
    currentBlockerLoadIndex = 1
    Deleteblockerspawns()
    Loadnextblockerspawn()

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
            friends[i]:NotifyObject{name = "allcancel", ObjIDSender = self}
        end

    end

end

function Loadnextblockerspawn()

    if currentBlockerLoadIndex <= #blockerspawns then
        RESMGR:LoadObject {objectTemplate = blockerspawns[currentBlockerLoadIndex],
                           x = -118.31,
                           y = 185.83,
                           z = -470 + blockeroffset,
                           owner = self}
        blockeroffset = blockeroffset + 5
        currentBlockerLoadIndex = currentBlockerLoadIndex + 1
    end

end

function Deleteblockerspawns()

    for i = 1, #blockersections do 
        GAMEOBJ:DeleteObject(blockersections[i])
    end
    blockersections = {}
    blockeroffset = 0
    currentBlockerPlaceIndex = 1
    currentBlockerLoadIndex = 1

end

function onNotifyObject(self, msg)

    if msg.name == "blockerloaded" then
        blockersections[#blockersections + 1] = msg.ObjIDSender
        Loadnextblockerspawn()

    elseif msg.name == "Selected" then
        if #blockersections >= currentBlockerPlaceIndex then

            blockersections[currentBlockerPlaceIndex]:SetPosition{pos=msg.ObjIDSender:GetPosition{}.pos}
            currentBlockerPlaceIndex = currentBlockerPlaceIndex + 1
        end

    elseif msg.name == "Trashed" then
        if #blockersections >= currentBlockerPlaceIndex then
            blockersections[currentBlockerPlaceIndex]:SetPosition{pos={x=msg.ObjIDSender:GetPosition{}.pos.x - 200, y=msg.ObjIDSender:GetPosition{}.pos.y, z=msg.ObjIDSender:GetPosition{}.pos.z}}
            currentBlockerPlaceIndex = currentBlockerPlaceIndex + 1
        end

    elseif msg.name == "blockercancel" then
        currentBlockerPlaceIndex = 11
        currentBlockerLoadIndex = 11

    elseif msg.name == "reset" then
        Deleteblockerspawns()
    end
    
end
