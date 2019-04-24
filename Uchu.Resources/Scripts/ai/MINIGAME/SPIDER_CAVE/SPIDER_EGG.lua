------------------------------------------------------
--Spider Egg script
------------------------------------------------------

local interactRadius = 15
local hatchTime = 1.0

function onStartup(self)

   self:SetProximityRadius { radius = interactRadius }
   --print("Egg Startup!")
end

function onProximityUpdate(self, msg)
   
   local isHuman = msg.objId:IsCharacter().isChar
   local hatchSpider = 1
   local chanceToHatch = math.random(3)
   if (msg.status == "ENTER") and (isHuman) and (chanceToHatch == hatchSpider) then
      
      --print("Look out!")
      self:PlayFXEffect{name = "dropdustmedium", effectID = 52, effectType = "rebuild_medium"}
      GAMEOBJ:GetTimer():AddTimerWithCancel(hatchTime, "hatchTime", self)
      --local pos = self:GetPosition().pos
      --self:Die()
      --RESMGR:LoadObject { objectTemplate = 6444  , x = pos.x , y =  pos.y , z = pos.z , owner = self }
   --elseif (chanceToHatch ~= hatchSpider) then
   
      --print("I'm a dud!")
   end
   
end

function onTimerDone(self, msg)
   --print("timer done!")
    self:PlayFXEffect{name = "bubbleSprayAttack", effectID = 212, effectType = "cast"}
    local pos = self:GetPosition().pos
    self:Die()
    RESMGR:LoadObject { objectTemplate = 6444  , x = pos.x , y =  pos.y , z = pos.z , owner = self }
   
end