------------------------------------------------------
--Script for the defending team's gates
------------------------------------------------------

local interactRadius = 10

function onStartup(self)
    --print("gate starting up")
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
    --print("proximity updated")
   
   local target = msg.objId
   local isHuman = msg.objId:IsCharacter().isChar
   if (msg.status == "ENTER") and (isHuman) and (self:GetVar("RebuildComplete") == true) and (target:CheckPrecondition{PreconditionID = 28}.bPass == true) then
        --print("raising the gate!")
        self:GoToWaypoint{iPathIndex = 1, bAllowPathingDirectionChange = true}
    elseif (msg.status == "LEAVE") and (isHuman) and (self:GetVar("RebuildComplete") == true) and (target:CheckPrecondition{PreconditionID = 28}.bPass == true) then
        --print("lowering the gate!")
        self:GoToWaypoint{iPathIndex = 0, bAllowPathingDirectionChange = true}
   end
   
end