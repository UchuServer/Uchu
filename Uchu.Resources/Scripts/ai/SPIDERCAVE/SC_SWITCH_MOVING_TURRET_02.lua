--------------------------------------------------------------------
--script on switch which explodes moving platform and destroys web
-- Updated 3/15 Darren McKinsey
--------------------------------------------------------------------

function onStartup(self)
   self:SetVar("Active", false)
end

function onNotifyObject(self, msg)
   if msg.name == "AtWaypoint" then
      self:SetVar("Active", true)
   end
end

function onFireEvent(self, msg)
	--print("got msg")
   if self:GetVar("Active") == true then
	  local object2 = self:GetObjectsInGroup{group = "moving_turret_01", ignoreSpawners = true}.objects[1]
      if object2 then
            object2:PlayFXEffect{name = "bigboomsupercharge", effectID = 580, effectType = "create"}
			object2:PlayFXEffect{name = "imaginationexplosion", effectID = 1034, effectType = "cast"}
      end
	  local object = self:GetObjectsInGroup{group = "GiantWeb", ignoreSpawners = true}.objects[1]
      if object then
		--print("got web")
         object:RequestDie()
      end
   end
end
