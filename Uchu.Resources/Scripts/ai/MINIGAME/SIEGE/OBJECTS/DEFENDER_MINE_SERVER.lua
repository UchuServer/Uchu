------------------------------------------------------
--Script for the mines
------------------------------------------------------

local interactRadius = 20

function onStartup(self)
    --print("mine starting up")
    self:SetVar("RebuildComplete", false)
    self:SetProximityRadius { radius = interactRadius }
end

function onRebuildNotifyState(self, msg)
    if (msg.iState == 2) then
        --print("Rebuild complete")
        self:SetVar("RebuildComplete", true)
    end
end

function onProximityUpdate(self, msg)
   
   local target = msg.objId
   local isHuman = msg.objId:IsCharacter().isChar
   if (msg.status == "ENTER") and (isHuman) and (self:GetVar("RebuildComplete") == true) and (target:CheckPrecondition{PreconditionID = 29}.bPass == true) then
        --print("Mine exploded!")
        self:CastSkill{skillID = 163, optionalTargetID = msg.objId}
        self:PlayFXEffect{name = "cannonbig", effectID = 71, effectType = "onfire_large"}
        self:Die()
    else
        --print("Wrong team!")
   end
   
end