function onStartup(self)
   self:AddObjectToGroup{group = "spiderDrill"}
   --self:SetVar("killSpider", false)
   print("starting up the drill")
end

function onArrived(self, msg)
   if (msg.wayPoint == 1) then
      print("on waypoint " .. msg.wayPoint)
      local friends = self:GetObjectsInGroup{group = "spiderDrill", ignoreSpawners = true}.objects
        for i, object in pairs(friends) do
            if (object:GetLOT().objtemplate == 4956) then
                object:NotifyObject{name = "drillDown", ObjIDSender = self}
            end
        end
   elseif (msg.wayPoint == 0) then
      print("on waypoint " .. msg.wayPoint)
      local friends = self:GetObjectsInGroup{group = "spiderDrill", ignoreSpawners = true}.objects
        for i, object in pairs(friends) do
            if (object:GetLOT().objtemplate == 4956) then
                object:NotifyObject{name = "drillUp", ObjIDSender = self}
            end
        end
   --[[elseif (msg.wayPoint == 3) then
      self:Die()
      print("on waypoint " .. msg.wayPoint)
      local friends = self:GetObjectsInGroup{group = "spiderDrill", ignoreSpawners = true}.objects
        for i, object in pairs(friends) do
            if (object:GetLOT().objtemplate == 6457) then
               print("killing the spider")
                object:Die{killerID = self, killType = "VIOLENT"}
            elseif (object:GetLOT().objtemplate == 5651) then
               print("toggling the trigger")
               object:ToggleTrigger{enable = false}
            end
        end--]]
   end
end

function onRebuildNotifyState(self, msg)
   if (msg.iState == 2) then
      local friends = self:GetObjectsInGroup{group = "spiderDrill", ignoreSpawners = true}.objects
      for i, object in pairs(friends) do
         if (object:GetLOT().objtemplate == 4956) then
            object:NotifyObject{name = "readyToMove", ObjIDSender = self}
         end
      end
   elseif (msg.iState == 0) then
      local friends = self:GetObjectsInGroup{group = "spiderDrill", ignoreSpawners = true}.objects
      for i, object in pairs(friends) do
         if (object:GetLOT().objtemplate == 4956) then
            object:NotifyObject{name = "notReadyToMove", ObjIDSender = self}
         end
      end
   end
end

--[[function onNotifyObject(self, msg)
   print("drill got a message")
   if msg.name == "killTheSpider" then
      print("got message to kill the spider")
      self:GoToWaypoint{iPathIndex = 3}
   elseif msg.name == "missTheSpider" then
      print("got message to miss the spider")
      self:GoToWaypoint{iPathIndex = 0}
   end
end--]]