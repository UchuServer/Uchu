---------------------------------------------------------------------------
--tells the client to play a cinematic
---------------------------------------------------------------------------

function onStartup(self)
   self:SetVar("haveplayedCine", false)
end

function onCollisionPhantom(self, msg)
   if msg.objectID:IsCharacter().isChar and self:GetVar("haveplayedCine") == false then
      self:SetVar("haveplayedCine", true)
      local playerAsID = GAMEOBJ:GetLocalCharID()
      local player = GAMEOBJ:GetObjectByID(playerAsID)
      player:PlayCinematic{pathName = "SpiderCine1"}
   end
end