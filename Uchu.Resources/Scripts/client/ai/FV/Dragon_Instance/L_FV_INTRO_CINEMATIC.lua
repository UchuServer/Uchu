----------------------------------------------------------
--Script by Steve Y -- 7/22/10
--plays a cinematic when the player enters the instance
----------------------------------------------------------

function onPlayerAddedToWorldLocal(self, msg)
    
    local player = msg.playerID
   
    player:PlayCinematic { pathName = "IntroCam" }

end