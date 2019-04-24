local sections = {}
local currentPlaceIndex = 1
local spawns = {5851,5852,5851,5852,5851,5852,5851,5852}
local offset = 0

function onClientUse(self)

    print "Click a green section to move pieces from left to right, click again to reset"

    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    player:PlayCinematic { pathName = "Gameplay_Setup" }
    
    Deletespawns()
    
    for i = 1, #spawns do 
        Loadspawn(i)
    end

end

function Loadspawn(yegges)

    RESMGR:LoadObject {objectTemplate = spawns[yegges],
                       x = -161 + offset,
                       y = 184.32,
                       z = -483,
                       owner = self}
    offset = offset + 5

end

function Deletespawns()

    for i = 1, #sections do 
        GAMEOBJ:DeleteObject(sections[i])
    end
    sections = {}
    offset = 0
    currentPlaceIndex = 1

end


function onNotifyObject(self, msg)
    if msg.name == "padloaded" then
        sections[#sections + 1] = msg.ObjIDSender
--        currentLoadIndex = currentLoadIndex + 1

    elseif msg.name == "Selected" then
        if #sections >= currentPlaceIndex then
            print "Turn off FX on Object" -- Remove effect from sections[currentPlaceIndex]
            sections[currentPlaceIndex]:SetPosition{pos=msg.ObjIDSender:GetPosition{}.pos}
            currentPlaceIndex = currentPlaceIndex + 1
            print "Turn on FX on Object" -- Add effect from sections[currentPlaceIndex]
        end
    end
        
end

function onGetOverridePickType(self, msg)

	msg.ePickType = 14
	return msg

end
--[[
5851 - Pad 01
5852 - Pad 02
5853 - Place Piece 01
5854 - Place Piece 02
5855 - Control Console
5856 - Cancel Button

--]]
