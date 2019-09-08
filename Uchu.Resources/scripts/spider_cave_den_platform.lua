require('State')
require('o_StateCreate')
require('o_mis')
require('o_Main')

function onStartup(self)
   self:AddObjectToGroup{group = "SpiderDen"}
   self:SetVar("turret1", false)
   self:SetVar("turret2", false)
   self:SetVar("platformBuilt", false)
   self:SetVar("TickTime", 2)
end

function onNotifyObject(self, msg)
   print("platform got a message")
   if (msg.name == "killQBTurret1") or (msg.name == "killQBTurret2") then
      if (msg.name == "killQBTurret1") then
         self:SetVar("turret1", true)
         print("setting turret1 to true")
      else
         self:SetVar("turret2", true)
      end
   elseif (msg.name == "dontShootQBTurret1") or (msg.name == "dontShootQBTurret2") then
      if (msg.name == "dontShootQBTurret1") then
         self:SetVar("turret1", false)
         print("setting turret1 to false")
      else
         self:SetVar("turret2", false)
      end
   end
   if (self:GetVar("turret1") == false) and (self:GetVar("platformBuilt") == true) or (self:GetVar("turret2") == false) and (self:GetVar("platformBuilt") == true) then
      GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("TickTime")  , "TickTime", self )
   end
end

function onRebuildNotifyState(self, msg)
    
	if (msg.iState == 2) then
        --self:SetFaction{faction = 108}
        --print("platform set to faction 108")
        self:SetVar("platformBuilt", true)
	    local friends = self:GetObjectsInGroup{group = "SpiderDen", ignoreSpawners = true}.objects
        for i, object in pairs(friends) do
            if (object:GetLOT().objtemplate == 6537) then
                object:NotifyObject{name = "killPlatform", ObjIDSender = self}
            end
        end
         if (self:GetVar("turret1") == false) and (self:GetVar("turret2") == false) then
         GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("TickTime")  , "TickTime", self )
      end
    end
end

function onTimerDone(self, msg)
   self:Die()
   print("spider destroyed the platform!")
end

function onDie(self, msg)

   print("platform down!")
   self:SetVar("platformBuilt", false)
    local friends = self:GetObjectsInGroup{group = "SpiderDen", ignoreSpawners = true}.objects
    for i, object in pairs(friends) do
        if (object:GetLOT().objtemplate == 6537) then
            object:NotifyObject{name = "platformDead", ObjIDSender = self}
        end
    end
end
