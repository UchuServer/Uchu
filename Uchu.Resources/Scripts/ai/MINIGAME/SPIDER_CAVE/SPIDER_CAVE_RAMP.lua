function onStartup(self)
   print("ramp starting up")
end

function onArrived(self, msg)

   if (msg.wayPoint == 0) then
      --self:StopPathing()
      print("at waypoint 1")
      local friends = self:GetObjectsInGroup{group = "spiderBoulder", ignoreSpawners = true}.objects
      for i, object in pairs(friends) do
         if object:GetLOT().objtemplate == 6514 then
            object:NotifyObject{name = "rampDown", ObjIDSender = self}
            print("sending message to ramp")
         end
      end
   elseif (msg.wayPoint == 1) then
      local friends = self:GetObjectsInGroup{group = "spiderBoulder", ignoreSpawners = true}.objects
      for i, object in pairs(friends) do
         if object:GetLOT().objtemplate == 6514 then
            object:NotifyObject{name = "rampUp", ObjIDSender = self}
            print("sending second message to ramp")
         end
      end
   end
end