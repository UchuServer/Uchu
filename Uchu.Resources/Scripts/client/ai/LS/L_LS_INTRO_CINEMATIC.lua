----------------------------------------------------------
--plays a cinematic when the player enters a volume
----------------------------------------------------------


function onStartup(self)

   self:SetVar("HavePlayed", false)
   
end



function onCollisionPhantom(self, msg)

   local playerID = GAMEOBJ:GetControlledID()
   
   if (msg.objectID:GetID() == playerID:GetID()) then
      --print ("playerID triggered the collision")
    
      local player = msg.objectID
      
      if self:GetVar("HavePlayed") == false then
         
         self:SetVar("HavePlayed", true)
         player:PlayCinematic { pathName = "IntroCam_3" }
         
      end
      
   end
   
end