------------------------------------------------------
--Spider Egg script
------------------------------------------------------

local interactRadius = 15
local hatchTime = 2.0

function onStartup(self)
    self:SetVar("hatching", false)
    self:SetProximityRadius { radius = interactRadius }
end

function onProximityUpdate(self, msg)
   local isHuman = msg.objId:IsCharacter().isChar
   
   if (msg.status == "ENTER") and (isHuman) and self:GetVar("hatching") == false then
      
      --print("Look out!")
      self:SetVar("hatching", true)
      self:PlayFXEffect{name = "dropdustmedium", effectID = 52, effectType = "rebuild_medium"}
      self:CastSkill{skillID = 305}
      GAMEOBJ:GetTimer():AddTimerWithCancel(hatchTime, "hatchTime", self)
   end   
end

function onOnHit(self, msg)
    --print("egg hit!")
    if self:GetVar("hatching") == false then
        self:SetVar("hatching", true)
        self:PlayFXEffect{name = "dropdustmedium", effectID = 52, effectType = "rebuild_medium"}
        GAMEOBJ:GetTimer():AddTimerWithCancel(hatchTime, "hatchTime", self)
        
        if msg.attacker:GetLOT().objtemplate ~= 6598 then
            self:CastSkill{skillID = 305}
        end
    end
end

function onTimerDone(self, msg)
   --print("timer done!")
    self:PlayFXEffect{name = "egg_puff_b", effectID = 644, effectType = "create"}
    local pos = self:GetPosition().pos
    
    self:Die()
    RESMGR:LoadObject { objectTemplate = 6444  , x = pos.x , y =  pos.y , z = pos.z , owner = self }   
end

function onNotifyObject(self,msg)
	if msg.name == "hatch" and self:GetVar("hatching") == false  then	
		self:SetVar("hatching", true)
		self:PlayFXEffect{name = "dropdustmedium", effectID = 52, effectType = "rebuild_medium"}
		self:CastSkill{skillID = 305}
		GAMEOBJ:GetTimer():AddTimerWithCancel(hatchTime, "hatchTime", self)	
	end
end

function onDie(self,msg)
    if msg.killerID ~= self then
        GAMEOBJ:GetZoneControlID():NotifyObject{name = "EggDestroyed" ,ObjIDSender = msg.killerID}
    end
end 