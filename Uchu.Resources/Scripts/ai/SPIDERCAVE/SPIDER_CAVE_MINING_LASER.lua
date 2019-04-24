-----------------------------------------------------------
--script to fire the laser at the maelstrom infected rocks
-----------------------------------------------------------

function onStartup(self)
   self:SetVar("effectTime", 2)
   self:PlayFXEffect{name = "moviespotlight", effectID = 503, effectType = "create"}
end

function onNotifyObject(self, msg)
   if msg.name == "StopLaser" then
      self:StopFXEffect{name = "moviespotlight"}
   end
end

function onArrived(self, msg)
   print("arrived at waypoint: " .. msg.wayPoint)
   if msg.wayPoint == 1 then
      self:PlayFXEffect{name = "moviespotlight", effectID = 503, effectType = "create"}
      GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("effectTime")  , "effectTime", self )
   end
end

function onTimerDone(self, msg)
   local object = self:GetObjectsInGroup{group = "MaelstromRock", ignoreSpawners = true}.objects[1]
   if object then
      object:Die()
   end
   self:StopFXEffect{name = "moviespotlight"}
end