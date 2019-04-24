require('o_mis')

CONSTANTS = {}
CONSTANTS["ClimbSide"] = 5915
CONSTANTS["ClimbTop"] = 5914
CONSTANTS["ClimbWall"] = 5916

local climbsections = {}
local currentClimberPlaceIndex = 1
local climberspawns = {5915,5915,5915,5915,5915,5915,5916,5916,5916,5916,5916,5916,5914,5914,5914,5914}
local climberoffset = 0
local currentClimberLoadIndex = 1


function onUse(self, msg)

    currentClimberPlaceIndex = 1
    currentClimberLoadIndex = 1
    Deleteclimberspawns()
    Loadnextclimberspawn()

    local friends = self:GetObjectsInGroup{ group = "GP_Control" }.objects
    for i = 1, table.maxn (friends) do 
        if friends[i]:GetLOT().objtemplate == 5899 then
            friends[i]:NotifyObject{name = "padcancel", ObjIDSender = self}

        elseif friends[i]:GetLOT().objtemplate == 5900 then
            friends[i]:NotifyObject{name = "bouncecancel", ObjIDSender = self}

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

function Loadnextclimberspawn()
    if currentClimberLoadIndex <= #climberspawns then
        if climberspawns[currentClimberLoadIndex] == CONSTANTS["ClimbSide"] then
            local config = { {"climbable", "ladder"} }
            RESMGR:LoadObject { objectTemplate = CONSTANTS["ClimbSide"],
                                x = -90.31,
                                y = 185.94,
                                z = -470 + climberoffset,
                                configData = config }

        elseif climberspawns[currentClimberLoadIndex] == CONSTANTS["ClimbTop"] then
            local config = { {"climbable", "wall"} }
            RESMGR:LoadObject { objectTemplate = CONSTANTS["ClimbTop"],
                                x = -90.31,
                                y = 185.94,
                                z = -470 + climberoffset,
                                configData = config }

        elseif climberspawns[currentClimberLoadIndex] == CONSTANTS["ClimbWall"] then
            local config = { {"climbable", "wall"} }
            RESMGR:LoadObject { objectTemplate = CONSTANTS["ClimbWall"],
                                x = -90.31,
                                y = 185.94,
                                z = -470 + climberoffset,
                                configData = config }

        end

        climberoffset = climberoffset + 5
        currentClimberLoadIndex = currentClimberLoadIndex + 1

    end

end

function Deleteclimberspawns()

    for i = 1, #climbsections do 
        GAMEOBJ:DeleteObject(climbsections[i])
    end
    climbsections = {}
    climberoffset = 0
    currentClimberPlaceIndex = 1
    currentClimberLoadIndex = 1

end

function onNotifyObject(self, msg)

    if msg.name == "climberloaded" then
        climbsections[#climbsections + 1] = msg.ObjIDSender
        Loadnextclimberspawn()

    elseif msg.name == "Selected" then
        if #climbsections >= currentClimberPlaceIndex then

            climbsections[currentClimberPlaceIndex]:SetPosition{pos=msg.ObjIDSender:GetPosition{}.pos}
  --          climbsections[currentClimberPlaceIndex]:StopFXEffect{name = "hot"}
            currentClimberPlaceIndex = currentClimberPlaceIndex + 1

--            if currentClimberPlaceIndex <= #climbsections then
--                climbsections[currentClimberPlaceIndex]:PlayFXEffect{name = "hot", effectType = "select"}
--            end
        end

    elseif msg.name == "Trashed" then
        if #climbsections >= currentClimberPlaceIndex then
            climbsections[currentClimberPlaceIndex]:SetPosition{pos={x=msg.ObjIDSender:GetPosition{}.pos.x - 200, y=msg.ObjIDSender:GetPosition{}.pos.y, z=msg.ObjIDSender:GetPosition{}.pos.z}}
            currentClimberPlaceIndex = currentClimberPlaceIndex + 1
        end

    elseif msg.name == "climbercancel" then
        currentClimberPlaceIndex = 17
        currentClimberLoadIndex = 17

    elseif msg.name == "reset" then
        Deleteclimberspawns()
    end
        
end
