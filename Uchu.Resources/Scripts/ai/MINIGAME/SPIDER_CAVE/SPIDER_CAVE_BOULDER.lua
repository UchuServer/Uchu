-------------------------------------------------
--script to roll the boulder down the hill and smash the spider
-------------------------------------------------

function onStartup(self)
   self:AddObjectToGroup{group = "spiderBoulder"}
   --self:SetVar("shortPath", false)
   self:SetVar("longPath", false)
   print("starting up the boulder")
end

function onArrived(self, msg)

   if (msg.wayPoint == 2) then
      --self:StopPathing()
      --print("CRASH!")
      local friends = self:GetObjectsInGroup{group = "spiderBoulder", ignoreSpawners = true}.objects
      for i, spider in pairs(friends) do
         if spider:GetLOT().objtemplate == 6457 then
            spider:Die{killerID = self, killType = "VIOLENT"}
         end
      end
      self:Die()
   end
end

function onNotifyObject(self, msg)
    print("boulder got a message")
    if (msg.name == "rampDown") then
        self:SetVar("longPath", true)
    elseif (msg.name == "rampUp") then
        self:SetVar("longPath", false)
    end
end

function onRebuildNotifyState(self, msg)
    print("notifying rebuild state for the boulder")
    if (msg.iState == 2) and (self:GetVar("longPath") == true) then
        self:StartPathing()
    elseif (msg.iState == 2) then
    print("setting boulder to short path")
        self:Die()
    end
end