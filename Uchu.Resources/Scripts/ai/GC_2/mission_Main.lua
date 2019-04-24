-- Saved Vars
function  GetMissionVars(self)
    self:SetVar("ConductCoolDown", false) 
    self:SetVar("ConductTimer_Started",false)
    self:SetVar("Emote_onExitBuyActive",false)
    Emote = {emote = emote} 
end


function IsLocalCharacter(target)
    return GAMEOBJ:GetLocalCharID() == target:GetID()
end

function CreateMissionStates(self)
    if self:GetVar('Miss.OverRideConduct') then
        self:SetProximityRadius { radius = self:GetVar("Miss.conductRadius") , name = "conductRadius" }
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
        emote(self, getMyMissionTarget(self), self:GetVar("EmoteType") ) 
        -- print(debug.traceback())
    end

    MissionEmote.onArrived = function(self)
    end
    
    addState(MissionIdle, "MissionIdle", "MissionIdle", self)
    addState(MissionEmote, "MissionEmote", "MissionEmote", self)
    beginStateMachine("MissionIdle", self) 
    MissionIdle.onEnter(self)    
end

function onProximityUpdate(self, msg)
    self:SetVar("skybox","mesh/env/challenge_sky_light_2awesome.nif")
	self:SetVar("skylayer","(invalid)")
	self:SetVar("ringlayer0","(invalid)")
	self:SetVar("ringlayer1","(invalid)")
	self:SetVar("ringlayer2","(invalid)")
	self:SetVar("ringlayer3","(invalid)")
           
    if msg.objType == "Enemies" or msg.objType == "NPC" then
        if  msg.name == "conductRadius" and msg.status == "ENTER" and IsLocalCharacter(msg.objId) and msg.objId:GetFaction().faction == 1 then
        end
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
    self:SetVar("EmbeddedTime", self:GetAnimationTime{  animationID = "interact" }.time)
    self:PlayFXEffect {priority = 1.2, effectType = skillType}
end

onNotifyMission = function(self,msg)
    msg = msg
end

onTerminateInteraction = function(self, msg) 
    if foo == nil then
        print("Closed the windows") 
    end
end