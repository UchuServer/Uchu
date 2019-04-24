require('o_mis')

CONSTANTS = {}
CONSTANTS["ClimbSide"] = 5915
CONSTANTS["ClimbTop"] = 5914
CONSTANTS["ClimbWall"] = 5916

local climbsections = {}
local currentClimberPlaceIndex = 1
local climberspawns = {5915,5915,5915,5915,5914,5914,5914,5916,5916,5916,5916}
local climberoffset = 0
local currentClimberLoadIndex = 1

function onGetOverridePickType(self, msg)
    msg.ePickType = 14
	return msg
end

function onClientUse(self, msg)

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
        end
    end

end

function Loadnextclimberspawn()
    if currentClimberLoadIndex <= #climberspawns then
        if climberspawns[currentClimberLoadIndex] == CONSTANTS["ClimbSide"] then
            local config = { {"climbable", "ladder"} }
            RESMGR:LoadObject { objectTemplate = CONSTANTS["ClimbSide"],
                                x = -116.86 + climberoffset,
                                y = 184.23,
                                z = -506.23,
                                configData = config }

        elseif climberspawns[currentClimberLoadIndex] == CONSTANTS["ClimbTop"] then
            local config = { {"climbable", "wall"} }
            RESMGR:LoadObject { objectTemplate = CONSTANTS["ClimbTop"],
                                x = -116.86 + climberoffset,
                                y = 184.23,
                                z = -506.23,
                                configData = config }

        elseif climberspawns[currentClimberLoadIndex] == CONSTANTS["ClimbWall"] then
            local config = { {"climbable", "wall"} }
            RESMGR:LoadObject { objectTemplate = CONSTANTS["ClimbWall"],
                                x = -116.86 + climberoffset,
                                y = 184.23,
                                z = -506.23,
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

    elseif msg.name == "climbercancel" then
        currentClimberPlaceIndex = 12
        currentClimberLoadIndex = 12
    end
        
end
