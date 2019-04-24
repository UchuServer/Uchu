--Chat Test

-- GLOBALS --

-- define what the radius of the NPC will be

local INTERACT_RADIUS = 100
local RAND_CHAT1 = {"Get over here soldier!", "I need your help!", "Be careful soldier!"}
local RAND_CHAT2 = {"I hope nothing happens while we transmit the data.", "Where are all my soldiers?", "This crash site is kind of spooky."}
local CHAT_TIME = 10.0

-- use the game message to assign the defined radius

function onStartup(self)

   self:SetProximityRadius { radius = INTERACT_RADIUS }
   GAMEOBJ:GetTimer():AddTimerWithCancel(CHAT_TIME, "ChatTimer", self)
   
end

function onTimerDone(self, msg)
   
   if( msg.name == "ChatTimer" ) then
   
      local index = math.random(1,#RAND_CHAT2)
      self:DisplayChatBubble{wsText = RAND_CHAT2[index]}
      GAMEOBJ:GetTimer():AddTimerWithCancel(CHAT_TIME, "ChatTimer", self)
   
   end
   
end

function onProximityUpdate(self, msg)

   if (msg.status == "ENTER") and (msg.objId:GetID() == GAMEOBJ:GetLocalCharID()) then

      GAMEOBJ:GetTimer():CancelAllTimers( self )
      local index = math.random(1,#RAND_CHAT1)
      self:DisplayChatBubble{wsText = RAND_CHAT1[index]}
      GAMEOBJ:GetTimer():AddTimerWithCancel(CHAT_TIME, "ChatTimer", self)
   
   end
   
end