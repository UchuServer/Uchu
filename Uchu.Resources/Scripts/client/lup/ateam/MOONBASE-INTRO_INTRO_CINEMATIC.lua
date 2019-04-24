----------------------------------------------------------
--Script by Brian K -- 8/26/10
--plays a cinematic when the player enters the instance
----------------------------------------------------------

function onPlayerAddedToWorldLocal(self, msg)
    
    local player = msg.playerID
   
    player:PlayCinematic { pathName = "Fly in 3" }

end