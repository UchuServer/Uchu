function onStartup(self)

    if GAMEOBJ:GetZoneControlID():GetVar("isMinigame") == nil or GAMEOBJ:GetZoneControlID():GetVar("isMinigameStarted") == false then
        
         player = self:GetItemOwner{}.ownerID
    
      
    end

end

