require('o_mis')

function GetMissionVars(self)

    --self:SetVar("ConductCoolDown", false) 
    --self:SetVar("ConductTimer_Started",false)
    self:SetVar("Emote_onExitBuyActive",false)
    --Emote = {emote = emote} 

end

function IsLocalCharacter(target)

    return GAMEOBJ:GetLocalCharID() == target:GetID()

end

function CreateMissionStates(self)

    if self:GetVar('Miss.OverRideConduct') then
        self:SetProximityRadius { radius = "16" , name = "conductRadius" }
    end 

    self:UseStateMachine{} 

    -- Idle State

    MissionIdle = State.create()

    MissionIdle.onEnter = function(self)
    end 

    MissionIdle.onArrived = function(self)
    end  

    -- Emote State

    MissionEmote = State.create()
    MissionEmote.onEnter = function(self)
        -- self:FaceTarget{ target = myTarget, degreesOff = 5, keepFacingTarget = true }
        emote(self, getMyMissionTarget(self), self:GetVar("EmoteType")) 
    end

    MissionEmote.onArrived = function(self)
    end

    addState(MissionIdle, "MissionIdle", "MissionIdle", self)
    addState(MissionEmote, "MissionEmote", "MissionEmote", self)
    beginStateMachine("MissionIdle", self) 
    MissionIdle.onEnter(self)    

end

function onProximityUpdate(self, msg)

    if msg.objType == "NPC" then
        if msg.name == "conductRadius" and msg.status == "ENTER" and IsLocalCharacter(msg.objId) and msg.objId:GetFaction().faction == (1 or 55) then
            local myMissionID = self:GetMissionForPlayer{playerID = msg.objId}.missionID
            local myMissionState = self:GetMissionForPlayer{playerID = msg.objId}.missionState
            print ("My mission state: ".. (myMissionState))

            if myMissionState == 65 then
                print "Mission 1"
                self:SetVar("EmoteType", "missionState1")
                setState("MissionEmote", self) 
            end

            if myMissionState == 2 or myMissionState == 10 then
                print "Mission 2"
                self:SetVar("EmoteType", "missionState2")
                setState("MissionEmote", self)
            end

            if myMissionState == 3 or myMissionState == 12 or myMissionState == 4 then
                print "Mission 3"
                self:SetVar("EmoteType", "missionState3")
                setState("MissionEmote", self) 
            end              

            if myMissionState == 73 then
                print "Mission 4"
                self:SetVar("EmoteType", "missionState4")
                setState("MissionEmote", self)
            end

            if myMissionState == 0 then
                print "Mission New"
                self:SetVar("EmoteType", "missionState1")
                setState("MissionEmote", self)
            end

        end
    end

    if msg.name == "conductRadius" and msg.status ~= "ENTER" and IsLocalCharacter(msg.objId) and msg.objId:GetFaction().faction == 1 then
        print "Leave"
        self:SetVar("EmoteType", "idle")
        setState("MissionEmote", self)
    end
end

function storeMissionTarget(self, target)

    idString = target:GetID()
    finalID = "|" .. idString
    self:SetVar("myMissionTarget", finalID)

end

function getMyMissionTarget(self)

    targetID = self:GetVar("myMissionTarget")
    return GAMEOBJ:GetObjectByID(targetID)

end

function emote(self,target, skillType)

        DoObjectAction(self, "anim", skillType)

end
