-----------------------------------------------------------------
--script which activates switch for "explosive" moving platform
-- Updated Darren McKinsey 3/15
-----------------------------------------------------------------

function onRebuildNotifyState(self, msg)
   if msg.iState == 2 then
      local object = self:GetObjectsInGroup{group = "turret_switch_01", ignoreSpawners = true}.objects[1]
      if object then
         object:NotifyObject{name = "ModuleBuilt", ObjIDSender = self}
         object:PlayFXEffect{name = "sirenlight_B", effectID = 242, effectType = "orange"}
      end
   end
end

function onArrived(self, msg)
   if msg.wayPoint == 1 then
      local object = self:GetObjectsInGroup{group = "turret_switch_02", ignoreSpawners = true}.objects[1]
      if object then
         object:NotifyObject{name = "AtWaypoint", ObjIDSender = self}
         object:PlayFXEffect{name = "sirenlight_B", effectID = 242, effectType = "orange"}
      end
	end
end