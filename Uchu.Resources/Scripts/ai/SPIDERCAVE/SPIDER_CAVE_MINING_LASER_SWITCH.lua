--------------------------------------------------------------------
--switch that tells the mining laser to stop firing while it's in motion
--------------------------------------------------------------------

function onFireEvent(self, msg)
   local object = self:GetObjectsInGroup{group = "MLaser", ignoreSpawners = true}.objects[1]
   if object then
      object:GoToWaypoint{iPathIndex = 1, bAllowPathingDirectionChange = false}
      object:NotifyObject{name = "StopLaser", ObjIDSender = self}
   end
end