require('o_mis')

--CONSTANTS = {}
--CONSTANTS["Springy"] = 5852

local platformsections = {}
local currentPlaceIndex = 1
local spawns = {5852,5852}
local offset = 0
local currentLoadIndex = 1


function onUse(self, msg)

    --print "Click a green section to move pieces, click again to reset"

    Deletespawns()
    
    Loadnextspawn()
    
end

function Loadnextspawn()

    if currentLoadIndex <= #spawns then
            RESMGR:LoadObject {objectTemplate = spawns[currentLoadIndex],
                               x = -161 + offset,
                               y = 186.32, -- 184
                               z = -483,
                               owner = self}
        end

        offset = offset + 5
        currentLoadIndex = currentLoadIndex + 1

    end

end

function Deletespawns()

    for i = 1, #platformsections do 
        GAMEOBJ:DeleteObject(platformsections[i])
    end
    platformsections = {}
    offset = 0
    currentPlaceIndex = 1
    currentLoadIndex = 1

end

function onNotifyObject(self, msg)

    if msg.name == "padloaded" then
        platformsections[#platformsections + 1] = msg.ObjIDSender
        Loadnextspawn()

    elseif msg.name == "Selected" then
        if #platformsections >= currentPlaceIndex then

--            print "Turn off FX on Object" -- Remove effect from platformsections[currentPlaceIndex]

            platformsections[currentPlaceIndex]:SetPosition{pos=msg.ObjIDSender:GetPosition{}.pos}
            platformsections[currentPlaceIndex]:StopFXEffect{name = "hot"}
            currentPlaceIndex = currentPlaceIndex + 1

--            print "Turn on FX on Object" -- Add effect from platformsections[currentPlaceIndex]

            if currentPlaceIndex <= #platformsections then
                platformsections[currentPlaceIndex]:PlayFXEffect{name = "hot", effectType = "select"}
            end
        end
    end
        
end
