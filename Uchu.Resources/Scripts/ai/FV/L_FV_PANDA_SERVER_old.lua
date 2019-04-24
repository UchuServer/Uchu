----------------------------------------------------------
--server side script for Panda
----------------------------------------------------------


--[[function onStartup(self)

    print("panda server starting up")

end--]]



function onFireEventServerSide(self, msg)

    if msg.args == "Foot_Race_Completed" then
        
        self:FireEventClientSide{args = msg.args}
        
        --print("firing serverside event")
        
        --mypos.x = mypos.x + 20
	
        --set the tamer as the player to check that only this player can tame that lion
        -- and set the group id to include the player id to make sure this player can only spawn one lion at a time
        local config = { { "tamer", player:GetID() } , { "groupID", "lion"..player:GetID()..";lions" } }
	
        RESMGR:LoadObject { objectTemplate = 5643 , x = mypos.x , y = mypos.y , z = mypos.z ,owner = self, configData = config }
    
    else return
    
    end

end