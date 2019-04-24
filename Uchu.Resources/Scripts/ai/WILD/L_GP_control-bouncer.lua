require('o_mis')

local bouncesections = {}
local currentBouncePlaceIndex = 1
local bouncespawns = {5852,5852,5861,5861,5875,5875}
local bounceoffset = 0
local currentBounceLoadIndex = 1


function onUse(self, msg)

    currentBouncePlaceIndex = 1
    currentBounceLoadIndex = 1
    Deletebouncespawns()
    Loadnextbouncespawn()

    local friends = self:GetObjectsInGroup{ group = "GP_Control" }.objects
    for i = 1, table.maxn (friends) do   
        if friends[i]:GetLOT().objtemplate == 5899 then
            friends[i]:NotifyObject{name = "padcancel", ObjIDSender = self}

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

function Loadnextbouncespawn()

    if currentBounceLoadIndex <= #bouncespawns then
        RESMGR:LoadObject {objectTemplate = bouncespawns[currentBounceLoadIndex],
                           x = -111.31,
                           y = 185.23,
                           z = -470 + bounceoffset,
                           owner = self}
        bounceoffset = bounceoffset + 10
        currentBounceLoadIndex = currentBounceLoadIndex + 1

    end

end

function Deletebouncespawns()

    for i = 1, #bouncesections do 
        GAMEOBJ:DeleteObject(bouncesections[i])
    end
    bouncesections = {}
    bounceoffset = 0
    currentBouncePlaceIndex = 1
    currentBounceLoadIndex = 1

end

function onNotifyObject(self, msg)

    if msg.name == "bounceloaded" then
        bouncesections[#bouncesections + 1] = msg.ObjIDSender
        Loadnextbouncespawn()

    elseif msg.name == "Selected" then
        if #bouncesections >= currentBouncePlaceIndex then

            bouncesections[currentBouncePlaceIndex]:SetPosition{pos=msg.ObjIDSender:GetPosition{}.pos}
--            bouncesections[currentBouncePlaceIndex]:StopFXEffect{name = "hot"}
            currentBouncePlaceIndex = currentBouncePlaceIndex + 1

--            if currentBouncePlaceIndex <= #bouncesections then
--                bouncesections[currentBouncePlaceIndex]:PlayFXEffect{name = "hot", effectType = "select"}
--            end
        end

    elseif msg.name == "Trashed" then
        if #bouncesections >= currentBouncePlaceIndex then
            bouncesections[currentBouncePlaceIndex]:SetPosition{pos={x=msg.ObjIDSender:GetPosition{}.pos.x - 200, y=msg.ObjIDSender:GetPosition{}.pos.y, z=msg.ObjIDSender:GetPosition{}.pos.z}}
            currentBouncePlaceIndex = currentBouncePlaceIndex + 1
        end

    elseif msg.name == "bouncecancel" then

        currentBouncePlaceIndex = 7
        currentBounceLoadIndex = 7

    elseif msg.name == "reset" then
        Deletebouncespawns()
    end
    
end
