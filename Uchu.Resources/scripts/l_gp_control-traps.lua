require('o_mis')

local trapsections = {}
local currentTrapPlaceIndex = 1
local trapspawns = {5917,5917,5918,5918}
local trapoffset = 0
local currentTrapLoadIndex = 1


function onUse(self, msg)

    currentTrapPlaceIndex = 1
    currentTrapLoadIndex = 1
    Deletetrapspawns()
    Loadnexttrapspawn()

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

        elseif friends[i]:GetLOT().objtemplate == 5927 then
            friends[i]:NotifyObject{name = "blockercancel", ObjIDSender = self}

        elseif friends[i]:GetLOT().objtemplate == 5855 then
            friends[i]:NotifyObject{name = "allcancel", ObjIDSender = self}
        end
    end

end

function Loadnexttrapspawn()

    if currentTrapLoadIndex <= #trapspawns then
        RESMGR:LoadObject {objectTemplate = trapspawns[currentTrapLoadIndex],
                           x = -104.31,
                           y = 185.53,
                           z = -470 + trapoffset,
                           owner = self}
        trapoffset = trapoffset + 15
        currentTrapLoadIndex = currentTrapLoadIndex + 1
    end

end

function Deletetrapspawns()

    for i = 1, #trapsections do 
        GAMEOBJ:DeleteObject(trapsections[i])
    end
    trapsections = {}
    trapoffset = 0
    currentTrapPlaceIndex = 1
    currentTrapLoadIndex = 1

end

function onNotifyObject(self, msg)

    if msg.name == "traploaded" then
        trapsections[#trapsections + 1] = msg.ObjIDSender
        Loadnexttrapspawn()

    elseif msg.name == "Selected" then
        if #trapsections >= currentTrapPlaceIndex then

            trapsections[currentTrapPlaceIndex]:SetPosition{pos=msg.ObjIDSender:GetPosition{}.pos}
--            trapsections[currentTrapPlaceIndex]:StopFXEffect{name = "hot"}
            currentTrapPlaceIndex = currentTrapPlaceIndex + 1

--            if currentTrapPlaceIndex <= #trapsections then
--                trapsections[currentTrapPlaceIndex]:PlayFXEffect{name = "hot", effectType = "select"}
--            end
        end

    elseif msg.name == "Trashed" then
        if #trapsections >= currentTrapPlaceIndex then
            trapsections[currentTrapPlaceIndex]:SetPosition{pos={x=msg.ObjIDSender:GetPosition{}.pos.x - 200, y=msg.ObjIDSender:GetPosition{}.pos.y, z=msg.ObjIDSender:GetPosition{}.pos.z}}
            currentTrapPlaceIndex = currentTrapPlaceIndex + 1
        end

    elseif msg.name == "trapcancel" then

        currentTrapPlaceIndex = 5
        currentTrapLoadIndex = 5

    elseif msg.name == "reset" then
        Deletetrapspawns()
    end
        
end
