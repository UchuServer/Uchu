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
 
           
      if msg.objType == "Enemies" or msg.objType == "NPC" then
      
    
--//////////////////////////////////////////////////////////////////////////////////
--//////////////////////////////////////////////////////////////////////////////////
--//////////////////////////////////////////////////////////////////////////////////
        if  msg.name == "conductRadius" and msg.status == "ENTER" and IsLocalCharacter(msg.objId) then
                storeMissionTarget(self, msg.objId)
              self:FaceTarget{ target = msg.objId, degreesOff = 5, keepFacingTarget = true }

                local myMissionID = self:GetMissionForPlayer{playerID = msg.objId}.missionID

               local myMissionState = self:GetMissionForPlayer{playerID = msg.objId}.missionState
               
                if myMissionState == 1 or myMissionState == 9 then -- STATE_AV AILABLE
                local chat1Text = self:GetMissionData{missionID = myMissionID}.chat1
                if ( chat1Text and chat1Text ~= nil and chat1Text ~= "" ) then
					self:DisplayChatBubble{wsText = chat1Text }
				end
                 self:SetVar("EmoteType", "missionState1")
                 setState("MissionEmote", self) 
                end
                if myMissionState == 2  or myMissionState == 10 then-- STATE_ACTIVE
				local chat2Text = self:GetMissionData{missionID = myMissionID}.chat2
                if ( chat2Text and chat2Text ~= nil and chat2Text ~= "" ) then
					self:DisplayChatBubble{wsText = chat2Text }
				end
                 self:SetVar("EmoteType", "missionState2")
                    setState("MissionEmote", self) 
                end               
                if myMissionState == 4 or myMissionState == 12 then -- READY_TO_COMPLETE
				local chat3Text = self:GetMissionData{missionID = myMissionID}.chat3
                if ( chat3Text and chat3Text ~= nil and chat3Text ~= "" ) then
					self:DisplayChatBubble{wsText = chat3Text }
				end
                 self:SetVar("EmoteType", "missionState3")
                    setState("MissionEmote", self) 
                end              
                if myMissionState == 0 then -- STATE_COMPLETED
                -- self:DisplayChatBubble{wsText = "null" }
                -- self:SetVar("EmoteType", "I am Nil")
                  --   setState("MissionEmote", self) 
                end              
              
              
            end
        end
--//////////////////////////////////////////////////////////////////////////////////
--//////////////////////////////////////////////////////////////////////////////////
--//////////////////////////////////////////////////////////////////////////////////  

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



