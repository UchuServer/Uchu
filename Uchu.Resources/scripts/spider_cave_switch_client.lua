require('o_mis')
------------------------------------------------------------------------------------------------
--client side script to play a cinematic when the cannon and quick build are both activated
------------------------------------------------------------------------------------------------

function onStartup(self)
   --print("ready!")
end

function onNotifyClientObject(self, msg)
   if (msg.name == "playCine") then
      local playerID = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
      playerID:PlayCinematic{pathName = "SpiderRetreat1"}
   end
end

--[[function onFireEvent(self, msg)
   print("event fired!")
   local playerID = GAMEOBJ:GetLocalCharID()
   if (msg.objectID:GetID() == playerID) then
      local player =msg.objectID
      player:PlayCinematic{pathName = "SpiderRetreat1"}
   end
end--]]
