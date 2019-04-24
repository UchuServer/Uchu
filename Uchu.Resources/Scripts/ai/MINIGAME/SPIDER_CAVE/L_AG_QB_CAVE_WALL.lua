--------------------------------------------------
--quick build to notify switch that it's ready
--------------------------------------------------


function onStartup (self)
   self:AddObjectToGroup{group = "cannonWall"}
   --print("starting up the qb wall!")
end

function onRebuildNotifyState (self, msg)
   --print(tostring(msg.iState))
   if (msg.iState == 2) then
      local friends = self:GetObjectsInGroup{group = "cannonWall", ignoreSpawners = true}.objects
      --print("wall ready!")
      for i, wall in pairs(friends) do
         if wall:GetLOT().objtemplate == 4956 then
            wall:NotifyObject{name = "wallIsBuilt", ObjIDSender = self}
            --print("notifying switch")
         end
      end
   end
end

function onDie (self, msg)
   local friends = self:GetObjectsInGroup{group = "cannonWall", ignoreSpawners = true}.objects
   --print("wall down!")
   for i, wall in pairs(friends) do
      if wall:GetLOT().objtemplate == 4956 then
         wall:NotifyObject{name = "wallDown", ObjIDSender = self}
      end
   end
end
