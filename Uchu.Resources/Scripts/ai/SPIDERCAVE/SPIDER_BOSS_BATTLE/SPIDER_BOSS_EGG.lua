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

   if (msg.status == "ENTER") and (isHuman) and not self:GetVar("hatching") --[[and (chanceToHatch == hatchSpider)--]] then
  
      self:SetVar("hatching", true)
      self:PlayFXEffect{name = "dropdustmedium", effectID = 52, effectType = "rebuild_medium"}
      self:CastSkill{skillID = 305}
      GAMEOBJ:GetTimer():AddTimerWithCancel(hatchTime, "hatchTime", self)
     
   end
   
end

function onOnHit(self, msg)

    if self:GetVar("hatching") == false   then
        self:SetVar("hatching", true)
        self:PlayFXEffect{name = "dropdustmedium", effectID = 52, effectType = "rebuild_medium"}
        GAMEOBJ:GetTimer():AddTimerWithCancel(hatchTime, "hatchTime", self)
        if msg.attacker:IsCharacter().isChar then
            self:CastSkill{skillID = 305}
        end
    end
end

function onTimerDone(self, msg)

	if msg.name == "hatchTime" then
	
		local ActivityObj = self:GetParentObj().objIDParent
		self:PlayFXEffect{name = "egg_puff_b", effectID = 644, effectType = "create"}
		local pos = self:GetPosition().pos
		RESMGR:LoadObject { objectTemplate = 9463  , x = pos.x , y =  pos.y , z = pos.z , owner = ActivityObj }
		GAMEOBJ:GetTimer():AddTimerWithCancel(0.2, "die", self)
   elseif msg.name == "die" then
   		GAMEOBJ:DeleteObject(self) 
   		--self:RequestDie()
   
   end
end


function onNotifyObject(self,msg)


	if msg.name == "hatch" and self:GetVar("hatching") == false  then
	
		self:SetVar("hatching", true)
		self:PlayFXEffect{name = "dropdustmedium", effectID = 52, effectType = "rebuild_medium"}
		GAMEOBJ:GetTimer():AddTimerWithCancel(hatchTime, "hatchTime", self)
	
	end


end



function onDie(self,msg)
	
	local ActivityObj = self:GetParentObj().objIDParent
	ActivityObj:NotifyObject{name = "removeEgg" ,ObjIDSender = self }
	 	
end