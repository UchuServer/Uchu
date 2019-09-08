require('o_mis')
CONSTANTS = {}
CONSTANTS["NO_OBJECT"] = "0"

function  GetMissionVars(self)
    self:SetVar("ConductCoolDown", false) 
    self:SetVar("ConductTimer_Started",false)
    self:SetVar("Emote_onExitBuyActive",false)
    Emote = {emote = emote} 
end


function IsLocalCharacter(target)
    return GAMEOBJ:GetLocalCharID() == target:GetID()
end

--[[
function CreateMissionStates(self)
      --self:SetProximityRadius { radius = 20 ,name = "misArrows" }
      --self:SetVar("ArrowSent", nil) 
      --self:SetVar("childNUM", 1) 
     
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
--]]

function emote(self,target, skillType)
        DoObjectAction(self, "anim", skillType)
end

--------------Special code for mission arrow prototype
--[[
onChildLoaded = function(self,msg)
    if  msg.childID:GetLOT().objtemplate == 4014  then
        local v = self:GetVar("childNUM")
        local x = "Spawn_"..v
	    msg.childID:SetMovingPlatformParams{ wsPlatformPath = self:GetVar(x), iStartIndex = 1 }
	    local n = v + 1
	    self:SetVar("childNUM", n)
	end
end 


onMissionDialogueOK = function(self, msg)
    if self:GetVar("Mission_Arrow") == nil then
        --print("nil Misson")
    end
    if msg.bIsComplete == true and self:GetVar("Mission_Complete") ~= nil  then
        local foundObj = self:GetProximityObjects{ name = "misArrows" }.objects

            for i = 1, table.maxn (foundObj) do  
                if foundObj[i]:GetLOT().objtemplate == 4639 then
                    --storeObjectByName(missionObj[i], "missionArrow", self)
                    foundObj[i]:NotifyObject{ name = "removeArrow" }
                    break

                end
            end
    end    
     if self:GetVar("Mission_Arrow") ~= nil and self:GetVar("ArrowSent") == nil then

                --print("Arrows Sent") 
                local s = self:GetVar("Mission_Arrow")
                local path = split(s, '-')

                  for b = 1, table.maxn(path) do
                    self:SetVar("Spawn_"..b,path[b])       
                    local config = { {"renderDisabled", true } , {"Group_Obj", 99 } }
                    local firstWP = self:GetPosition().pos
                    RESMGR:LoadObject { objectTemplate =  4014  , x=  firstWP.x  , y=  firstWP.y , z=  firstWP.z  , owner = self, configData = config  } 
                    
                    self:SetVar("ArrowSent", 99) 
                  end              
                
               
  
    end
 

end
--]]
